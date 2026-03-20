using NNPensionPlanner.Data;
using NNPensionPlanner.Events;
using NNPensionPlanner.Models;

namespace NNPensionPlanner.Services;

public class ProjectionService
{
    private readonly IRepository<Projection> _projectionRepo;
    private readonly IRepository<Enrollment> _enrollmentRepo;
    private readonly IRepository<Participant> _participantRepo;
    private readonly IRepository<PensionPlan> _planRepo;
    private readonly ContributionService _contributionService;
    private readonly EventBus _eventBus;
    private readonly ILogger<ProjectionService> _logger;

    public ProjectionService(
        IRepository<Projection> projectionRepo,
        IRepository<Enrollment> enrollmentRepo,
        IRepository<Participant> participantRepo,
        IRepository<PensionPlan> planRepo,
        ContributionService contributionService,
        EventBus eventBus,
        ILogger<ProjectionService> logger)
    {
        _projectionRepo = projectionRepo;
        _enrollmentRepo = enrollmentRepo;
        _participantRepo = participantRepo;
        _planRepo = planRepo;
        _contributionService = contributionService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public IEnumerable<Projection> GetByEnrollment(Guid enrollmentId) =>
        _projectionRepo.Find(p => p.EnrollmentId == enrollmentId);

    public Projection? GetLatest(Guid enrollmentId) =>
        _projectionRepo.Find(p => p.EnrollmentId == enrollmentId)
            .OrderByDescending(p => p.CalculatedAt)
            .FirstOrDefault();

    public Projection Calculate(Guid enrollmentId, int? retirementAge = null)
    {
        var enrollment = _enrollmentRepo.GetById(enrollmentId)
            ?? throw new ArgumentException($"Enrollment {enrollmentId} not found.");

        var participant = _participantRepo.GetById(enrollment.ParticipantId)
            ?? throw new ArgumentException("Participant not found.");

        var plan = _planRepo.GetById(enrollment.PlanId)
            ?? throw new ArgumentException("Plan not found.");

        var targetRetirementAge = retirementAge ?? 67;
        var currentAge = participant.Age;
        var yearsToRetirement = Math.Max(0, targetRetirementAge - currentAge);

        // Current accumulated balance
        var currentBalance = _contributionService.GetTotalBalance(enrollmentId);

        // Monthly contribution (employee + employer match)
        var monthlyGross = participant.AnnualSalary / 12;
        var employeeMonthly = monthlyGross * (enrollment.ContributionPercentage / 100);
        var employerMonthly = monthlyGross * (Math.Min(enrollment.ContributionPercentage, plan.EmployerMatchPercentage) / 100);
        var totalMonthlyContribution = employeeMonthly + employerMonthly;

        // Calculate three scenarios
        var scenarios = new List<ProjectionScenario>
        {
            CalculateScenario("Conservative", 0.03m, currentBalance, totalMonthlyContribution, yearsToRetirement),
            CalculateScenario("Expected", 0.06m, currentBalance, totalMonthlyContribution, yearsToRetirement),
            CalculateScenario("Optimistic", 0.08m, currentBalance, totalMonthlyContribution, yearsToRetirement)
        };

        var projection = new Projection
        {
            EnrollmentId = enrollmentId,
            RetirementAge = targetRetirementAge,
            YearsToRetirement = yearsToRetirement,
            CurrentBalance = currentBalance,
            MonthlyContribution = totalMonthlyContribution,
            Scenarios = scenarios,
            CalculatedAt = DateTime.UtcNow
        };

        var saved = _projectionRepo.Add(projection);
        _logger.LogInformation("Calculated projection {Id} for enrollment {EnrollmentId}: {Years} years to retirement",
            saved.Id, enrollmentId, yearsToRetirement);

        _eventBus.Publish(new ProjectionRequested(enrollmentId, DateTime.UtcNow));

        return saved;
    }

    private static ProjectionScenario CalculateScenario(
        string name,
        decimal annualReturnRate,
        decimal currentBalance,
        decimal monthlyContribution,
        int yearsToRetirement)
    {
        // Future value of current balance: FV = PV * (1 + r)^n
        var monthlyRate = annualReturnRate / 12;
        var totalMonths = yearsToRetirement * 12;

        var futureValueOfBalance = currentBalance * (decimal)Math.Pow((double)(1 + monthlyRate), totalMonths);

        // Future value of monthly contributions: FV = PMT * [((1+r)^n - 1) / r]
        decimal futureValueOfContributions;
        if (monthlyRate > 0)
        {
            futureValueOfContributions = monthlyContribution *
                (decimal)((Math.Pow((double)(1 + monthlyRate), totalMonths) - 1) / (double)monthlyRate);
        }
        else
        {
            futureValueOfContributions = monthlyContribution * totalMonths;
        }

        var estimatedLumpSum = Math.Round(futureValueOfBalance + futureValueOfContributions, 2);

        // Convert lump sum to monthly pension (assuming 20-year payout period)
        var payoutMonths = 20 * 12;
        var estimatedMonthlyPension = Math.Round(estimatedLumpSum / payoutMonths, 2);

        return new ProjectionScenario
        {
            Name = name,
            AnnualReturnRate = annualReturnRate,
            EstimatedLumpSum = estimatedLumpSum,
            EstimatedMonthlyPension = estimatedMonthlyPension
        };
    }
}
