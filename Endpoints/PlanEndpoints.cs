using NNPensionPlanner.Data;
using NNPensionPlanner.Models;
using NNPensionPlanner.Services;

namespace NNPensionPlanner.Endpoints;

public static class PlanEndpoints
{
    public static void MapPlanEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/plans").WithTags("Pension Plans");

        group.MapGet("/", (IRepository<PensionPlan> repo) =>
            Results.Ok(repo.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, IRepository<PensionPlan> repo) =>
        {
            var plan = repo.GetById(id);
            return plan is not null ? Results.Ok(plan) : Results.NotFound();
        });

        group.MapGet("/{id:guid}/enrollments", (Guid id, EnrollmentService enrollmentService) =>
        {
            var enrollments = enrollmentService.GetAll()
                .Where(e => e.PlanId == id);
            return Results.Ok(enrollments);
        });
    }
}
