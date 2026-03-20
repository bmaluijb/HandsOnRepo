using NNPensionPlanner.Data;
using NNPensionPlanner.Events;
using NNPensionPlanner.Models;

namespace NNPensionPlanner.Services;

public class ContributionService
{
    private readonly IRepository<Contribution> _contributionRepo;
    private readonly IRepository<Enrollment> _enrollmentRepo;
    private readonly IRepository<Participant> _participantRepo;
    private readonly IRepository<PensionPlan> _planRepo;
    private readonly EventBus _eventBus;
    private readonly ILogger<ContributionService> _logger;

    public ContributionService(
        IRepository<Contribution> contributionRepo,
        IRepository<Enrollment> enrollmentRepo,
        IRepository<Participant> participantRepo,
        IRepository<PensionPlan> planRepo,
        EventBus eventBus,
        ILogger<ContributionService> logger)
    {
        _contributionRepo = contributionRepo;
        _enrollmentRepo = enrollmentRepo;
        _participantRepo = participantRepo;
        _planRepo = planRepo;
        _eventBus = eventBus;
        _logger = logger;
    }

    public IEnumerable<Contribution> GetAll() => _contributionRepo.GetAll();

    public IEnumerable<Contribution> GetByEnrollment(Guid enrollmentId) =>
        _contributionRepo.Find(c => c.EnrollmentId == enrollmentId);

    public Contribution AddContribution(Guid enrollmentId, decimal employeeAmount, ContributionType type = ContributionType.Regular)
    {
        var enrollment = _enrollmentRepo.GetById(enrollmentId)
            ?? throw new ArgumentException($"Enrollment {enrollmentId} not found.");

        if (enrollment.Status != EnrollmentStatus.Active)
            throw new ArgumentException("Cannot add contributions to a non-active enrollment.");

        var plan = _planRepo.GetById(enrollment.PlanId)
            ?? throw new ArgumentException("Associated plan not found.");

        // Calculate employer match
        var participant = _participantRepo.GetById(enrollment.ParticipantId)!;
        var monthlyGross = participant.AnnualSalary / 12;
        var employeePercentage = employeeAmount / monthlyGross * 100;
        var matchPercentage = Math.Min(employeePercentage, plan.EmployerMatchPercentage);
        var employerAmount = Math.Round(monthlyGross * matchPercentage / 100, 2);

        var contribution = new Contribution
        {
            EnrollmentId = enrollmentId,
            Date = DateTime.UtcNow,
            EmployeeAmount = Math.Round(employeeAmount, 2),
            EmployerAmount = employerAmount,
            Type = type
        };

        var created = _contributionRepo.Add(contribution);
        _logger.LogInformation(
            "Added contribution {Id}: employee €{Employee}, employer €{Employer} for enrollment {EnrollmentId}",
            created.Id, created.EmployeeAmount, created.EmployerAmount, enrollmentId);

        _eventBus.Publish(new ContributionAdded(created.Id, enrollmentId, created.TotalAmount, DateTime.UtcNow));

        return created;
    }

    public decimal GetTotalBalance(Guid enrollmentId)
    {
        return _contributionRepo
            .Find(c => c.EnrollmentId == enrollmentId)
            .Sum(c => c.TotalAmount);
    }

    public IEnumerable<object> GetMonthlySummary(Guid enrollmentId)
    {
        return _contributionRepo
            .Find(c => c.EnrollmentId == enrollmentId)
            .GroupBy(c => new { c.Date.Year, c.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                EmployeeTotal = g.Sum(c => c.EmployeeAmount),
                EmployerTotal = g.Sum(c => c.EmployerAmount),
                Total = g.Sum(c => c.TotalAmount),
                Count = g.Count()
            });
    }
}
