namespace NNPensionPlanner.Events;

public record EnrollmentCreated(Guid EnrollmentId, Guid ParticipantId, Guid PlanId, DateTime Timestamp);
public record ContributionAdded(Guid ContributionId, Guid EnrollmentId, decimal TotalAmount, DateTime Timestamp);
public record ProjectionRequested(Guid EnrollmentId, DateTime Timestamp);
public record EnrollmentStatusChanged(Guid EnrollmentId, string OldStatus, string NewStatus, DateTime Timestamp);
