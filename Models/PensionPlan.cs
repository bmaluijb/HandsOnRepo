namespace NNPensionPlanner.Models;

public enum PlanType
{
    DefinedBenefit,
    DefinedContribution
}

public class PensionPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PlanType Type { get; set; }
    public decimal EmployerMatchPercentage { get; set; }
    public decimal MaxContributionPercentage { get; set; }
    public int VestingPeriodYears { get; set; }
    public decimal MinimumAnnualSalary { get; set; }
    public bool IsActive { get; set; } = true;
}
