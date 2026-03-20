namespace NNPensionPlanner.Models;

public enum EnrollmentStatus
{
    Active,
    Suspended,
    Closed
}

public class Enrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ParticipantId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public decimal ContributionPercentage { get; set; }
}
