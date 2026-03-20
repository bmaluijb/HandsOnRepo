namespace NNPensionPlanner.Models;

public class ProjectionScenario
{
    public string Name { get; set; } = string.Empty;
    public decimal AnnualReturnRate { get; set; }
    public decimal EstimatedMonthlyPension { get; set; }
    public decimal EstimatedLumpSum { get; set; }
}

public class Projection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnrollmentId { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public int RetirementAge { get; set; } = 67;
    public int YearsToRetirement { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal MonthlyContribution { get; set; }
    public List<ProjectionScenario> Scenarios { get; set; } = new();
}
