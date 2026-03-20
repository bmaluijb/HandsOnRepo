namespace NNPensionPlanner.Models;

public enum ContributionType
{
    Regular,
    Voluntary,
    CatchUp
}

public class Contribution
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnrollmentId { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public decimal EmployeeAmount { get; set; }
    public decimal EmployerAmount { get; set; }
    public ContributionType Type { get; set; } = ContributionType.Regular;

    public decimal TotalAmount => EmployeeAmount + EmployerAmount;
}
