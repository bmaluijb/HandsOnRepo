using NNPensionPlanner.Data;
using NNPensionPlanner.Events;
using NNPensionPlanner.Models;

namespace NNPensionPlanner.Services;

public class ContributionService
{
    private const int MonthsPerYear = 12;
    private const int PercentageBase = 100;
    private const int CurrencyDecimalPlaces = 2;

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

    /// <summary>
    /// Retrieves all contributions for a specific enrollment.
    /// </summary>
    /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
    /// <returns>An enumerable of contributions for the enrollment.</returns>
    /// <exception cref="ArgumentException">Thrown when enrollmentId is empty.</exception>
    public IEnumerable<Contribution> GetByEnrollment(Guid enrollmentId)
    {
        ValidateEnrollmentId(enrollmentId);
        return _contributionRepo.Find(c => c.EnrollmentId == enrollmentId);
    }

    /// <summary>
    /// Records a new contribution to an enrollment's pension fund.
    /// Validates enrollment status, calculates employer match based on plan rules, and publishes a domain event.
    /// </summary>
    /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
    /// <param name="employeeAmount">The employee contribution amount in euros (must be positive).</param>
    /// <param name="type">The type of contribution (defaults to Regular).</param>
    /// <returns>The created contribution object.</returns>
    /// <exception cref="ArgumentException">Thrown when validation fails (invalid ID, invalid enrollment state, exceeds plan limits).</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the contribution amount is not positive.</exception>
    public Contribution AddContribution(Guid enrollmentId, decimal employeeAmount, ContributionType type = ContributionType.Regular)
    {
        ValidateEnrollmentId(enrollmentId);
        ValidateContributionAmount(employeeAmount);
        ValidateContributionType(type);

        var enrollment = _enrollmentRepo.GetById(enrollmentId)
            ?? throw new ArgumentException($"Enrollment {enrollmentId} not found.");

        var plan = _planRepo.GetById(enrollment.PlanId)
            ?? throw new ArgumentException("Associated plan not found.");

        var participant = _participantRepo.GetById(enrollment.ParticipantId)
            ?? throw new InvalidOperationException($"Participant for enrollment {enrollmentId} not found.");

        ValidateEnrollmentAndParticipant(enrollment, participant);

        // Calculate employer match
        var monthlyGross = participant.AnnualSalary / MonthsPerYear;
        var employeePercentage = employeeAmount / monthlyGross * PercentageBase;

        ValidateContributionLimits(plan, employeePercentage);

        var employerAmount = CalculateEmployerMatch(employeePercentage, monthlyGross, plan.EmployerMatchPercentage);

        var contribution = new Contribution
        {
            EnrollmentId = enrollmentId,
            Date = DateTime.UtcNow,
            EmployeeAmount = Math.Round(employeeAmount, CurrencyDecimalPlaces),
            EmployerAmount = employerAmount,
            Type = type
        };

        new Exception().Data["EnrollmentId"] = enrollmentId;
        // Log the contribution details before saving



        var created = _contributionRepo.Add(contribution);
        _logger.LogInformation(
            "Added contribution {Id}: employee €{Employee}, employer €{Employer} for enrollment {EnrollmentId}",
            created.Id, created.EmployeeAmount, created.EmployerAmount, enrollmentId);

        _eventBus.Publish(new ContributionAdded(created.Id, enrollmentId, created.TotalAmount, DateTime.UtcNow));

        return created;
    }


    public void HelperFDunctionForSecurityTesting()
    {
        // This function is intentionally left blank to serve as a target for security testing.
        // It can be used to verify that security scanning tools are properly configured and can detect potential vulnerabilities.
        //TODO: implement security testing logic here if needed in the future.
        // Placeholder for future security testing implementation

        

    }

    /// <summary>
    /// Calculates the total accumulated balance (employee + employer contributions) for an enrollment.
    /// </summary>
    /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
    /// <returns>The total balance in euros.</returns>
    /// <exception cref="ArgumentException">Thrown when enrollmentId is empty.</exception>
    public decimal GetTotalBalance(Guid enrollmentId)
    {
        ValidateEnrollmentId(enrollmentId);
        return _contributionRepo
            .Find(c => c.EnrollmentId == enrollmentId)
            .Sum(c => c.TotalAmount);
    }

    /// <summary>
    /// Retrieves a monthly summary of contributions for an enrollment, grouped by month and year.
    /// </summary>
    /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
    /// <returns>An enumerable of monthly summaries sorted chronologically.</returns>
    /// <exception cref="ArgumentException">Thrown when enrollmentId is empty.</exception>
    public IEnumerable<MonthlySummary> GetMonthlySummary(Guid enrollmentId)
    {
        ValidateEnrollmentId(enrollmentId);
        return _contributionRepo
            .Find(c => c.EnrollmentId == enrollmentId)
            .GroupBy(c => new { c.Date.Year, c.Date.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new MonthlySummary(
                g.Key.Year,
                g.Key.Month,
                g.Sum(c => c.EmployeeAmount),
                g.Sum(c => c.EmployerAmount),
                g.Sum(c => c.TotalAmount),
                g.Count()));
    }

    private static decimal CalculateEmployerMatch(decimal employeePercentage, decimal monthlyGross, decimal employerMatchPercentage)
    {
        var matchPercentage = Math.Min(employeePercentage, employerMatchPercentage);
        return Math.Round(monthlyGross * matchPercentage / PercentageBase, CurrencyDecimalPlaces);
    }

    private static void ValidateEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty)
            throw new ArgumentException("Enrollment ID must not be empty.", nameof(enrollmentId));
    }

    private static void ValidateContributionAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Employee contribution amount must be greater than zero.");
    }

    private static void ValidateContributionType(ContributionType type)
    {
        if (!Enum.IsDefined(typeof(ContributionType), type))
            throw new ArgumentException("Invalid contribution type.", nameof(type));
    }

    private void ValidateEnrollmentAndParticipant(Enrollment enrollment, Participant participant)
    {
        if (enrollment.Status != EnrollmentStatus.Active)
        {
            _logger.LogWarning("Contribution rejected: enrollment {EnrollmentId} is not active (status: {Status})",
                enrollment.Id, enrollment.Status);
            throw new ArgumentException("Cannot add contributions to a non-active enrollment.");
        }

        if (participant.AnnualSalary <= 0)
        {
            _logger.LogWarning("Contribution rejected: participant {ParticipantId} has invalid salary {Salary}",
                participant.Id, participant.AnnualSalary);
            throw new InvalidOperationException("Participant has an invalid annual salary.");
        }
    }

    private void ValidateContributionLimits(PensionPlan plan, decimal employeePercentage)
    {
        if (plan.MaxContributionPercentage > 0 && employeePercentage > plan.MaxContributionPercentage)
        {
            _logger.LogWarning("Contribution rejected: employee percentage {EmployeePercentage}% exceeds plan maximum {MaxPercentage}%",
                employeePercentage, plan.MaxContributionPercentage);
            throw new ArgumentException(
                $"Employee contribution exceeds the plan maximum of {plan.MaxContributionPercentage}%.");
        }
    }
}

public record MonthlySummary(
    int Year,
    int Month,
    decimal EmployeeTotal,
    decimal EmployerTotal,
    decimal Total,
    int Count);
