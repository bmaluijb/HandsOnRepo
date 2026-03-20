using NNPensionPlanner.Data;
using NNPensionPlanner.Events;
using NNPensionPlanner.Models;

namespace NNPensionPlanner.Services;

public class EnrollmentService
{
    private readonly IRepository<Enrollment> _enrollmentRepo;
    private readonly IRepository<Participant> _participantRepo;
    private readonly IRepository<PensionPlan> _planRepo;
    private readonly EventBus _eventBus;
    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(
        IRepository<Enrollment> enrollmentRepo,
        IRepository<Participant> participantRepo,
        IRepository<PensionPlan> planRepo,
        EventBus eventBus,
        ILogger<EnrollmentService> logger)
    {
        _enrollmentRepo = enrollmentRepo;
        _participantRepo = participantRepo;
        _planRepo = planRepo;
        _eventBus = eventBus;
        _logger = logger;
    }

    public IEnumerable<Enrollment> GetAll() => _enrollmentRepo.GetAll();

    public Enrollment? GetById(Guid id) => _enrollmentRepo.GetById(id);

    public IEnumerable<Enrollment> GetByParticipant(Guid participantId) =>
        _enrollmentRepo.Find(e => e.ParticipantId == participantId);

    public Enrollment Create(Enrollment enrollment)
    {
        var participant = _participantRepo.GetById(enrollment.ParticipantId)
            ?? throw new ArgumentException($"Participant {enrollment.ParticipantId} not found.");

        var plan = _planRepo.GetById(enrollment.PlanId)
            ?? throw new ArgumentException($"Plan {enrollment.PlanId} not found.");

        if (!plan.IsActive)
            throw new ArgumentException($"Plan '{plan.Name}' is not currently active.");

        if (participant.AnnualSalary < plan.MinimumAnnualSalary)
            throw new ArgumentException($"Participant salary €{participant.AnnualSalary:N0} is below the minimum €{plan.MinimumAnnualSalary:N0} for plan '{plan.Name}'.");

        if (enrollment.ContributionPercentage > plan.MaxContributionPercentage)
            throw new ArgumentException($"Contribution percentage {enrollment.ContributionPercentage}% exceeds the plan maximum of {plan.MaxContributionPercentage}%.");

        // Check for existing active enrollment in same plan
        var existing = _enrollmentRepo.Find(e =>
            e.ParticipantId == enrollment.ParticipantId &&
            e.PlanId == enrollment.PlanId &&
            e.Status == EnrollmentStatus.Active);

        if (existing.Any())
            throw new ArgumentException("Participant already has an active enrollment in this plan.");

        var created = _enrollmentRepo.Add(enrollment);
        _logger.LogInformation("Created enrollment {Id} for participant {ParticipantId} in plan {PlanId}",
            created.Id, created.ParticipantId, created.PlanId);

        _eventBus.Publish(new EnrollmentCreated(created.Id, created.ParticipantId, created.PlanId, DateTime.UtcNow));

        return created;
    }

    public Enrollment? UpdateStatus(Guid id, EnrollmentStatus newStatus)
    {
        var enrollment = _enrollmentRepo.GetById(id);
        if (enrollment is null) return null;

        var oldStatus = enrollment.Status;
        enrollment.Status = newStatus;

        if (newStatus == EnrollmentStatus.Closed)
            enrollment.EndDate = DateTime.UtcNow;

        _enrollmentRepo.Update(enrollment);
        _logger.LogInformation("Enrollment {Id} status changed from {Old} to {New}", id, oldStatus, newStatus);

        _eventBus.Publish(new EnrollmentStatusChanged(id, oldStatus.ToString(), newStatus.ToString(), DateTime.UtcNow));

        return enrollment;
    }
}
