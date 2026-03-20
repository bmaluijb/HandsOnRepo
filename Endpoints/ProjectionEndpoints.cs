using NNPensionPlanner.Models;
using NNPensionPlanner.Services;

namespace NNPensionPlanner.Endpoints;

public static class ProjectionEndpoints
{
    public static void MapProjectionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/projections").WithTags("Projections");

        group.MapGet("/enrollment/{enrollmentId:guid}", (Guid enrollmentId, ProjectionService service) =>
            Results.Ok(service.GetByEnrollment(enrollmentId)));

        group.MapGet("/enrollment/{enrollmentId:guid}/latest", (Guid enrollmentId, ProjectionService service) =>
        {
            var projection = service.GetLatest(enrollmentId);
            return projection is not null ? Results.Ok(projection) : Results.NotFound();
        });

        group.MapPost("/calculate", (CalculateProjectionRequest request, ProjectionService service) =>
        {
            var projection = service.Calculate(request.EnrollmentId, request.RetirementAge);
            return Results.Ok(projection);
        });
    }
}

public record CalculateProjectionRequest(Guid EnrollmentId, int? RetirementAge = null);
