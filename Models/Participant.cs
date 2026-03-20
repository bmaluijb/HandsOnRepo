namespace NNPensionPlanner.Models;

public class Participant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; } = string.Empty;
    public string EmployerName { get; set; } = string.Empty;
    public decimal AnnualSalary { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;

    public string FullName => $"{FirstName} {LastName}";
    public int Age => (int)((DateTime.UtcNow - DateOfBirth).TotalDays / 365.25);
}
