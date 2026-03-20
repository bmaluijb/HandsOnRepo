using NNPensionPlanner.Models;
using NNPensionPlanner.Services;

namespace NNPensionPlanner.Endpoints;

public static class ContributionEndpoints
{
    public static void MapContributionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/contributions").WithTags("Contributions");

        group.MapGet("/enrollment/{enrollmentId:guid}", (Guid enrollmentId, ContributionService service) =>
            Results.Ok(service.GetByEnrollment(enrollmentId)));

        group.MapGet("/enrollment/{enrollmentId:guid}/balance", (Guid enrollmentId, ContributionService service) =>
            Results.Ok(new { enrollmentId, balance = service.GetTotalBalance(enrollmentId) }));

        group.MapGet("/enrollment/{enrollmentId:guid}/summary", (Guid enrollmentId, ContributionService service) =>
            Results.Ok(service.GetMonthlySummary(enrollmentId)));

        group.MapPost("/", (AddContributionRequest request, ContributionService service) =>
        {
            var contribution = service.AddContribution(
                request.EnrollmentId,
                request.EmployeeAmount,
                request.Type);
            return Results.Created($"/api/contributions/{contribution.Id}", contribution);
        });
    }
}

public record AddContributionRequest(Guid EnrollmentId, decimal EmployeeAmount, ContributionType Type = ContributionType.Regular);
