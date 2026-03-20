using NNPensionPlanner.Services;

namespace NNPensionPlanner.Endpoints;

public static class EnrollmentEndpoints
{
    public static void MapEnrollmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/enrollments").WithTags("Enrollments");

        group.MapGet("/", (EnrollmentService service) =>
            Results.Ok(service.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, EnrollmentService service) =>
        {
            var enrollment = service.GetById(id);
            return enrollment is not null ? Results.Ok(enrollment) : Results.NotFound();
        });

        group.MapGet("/participant/{participantId:guid}", (Guid participantId, EnrollmentService service) =>
            Results.Ok(service.GetByParticipant(participantId)));

        group.MapPost("/", (NNPensionPlanner.Models.Enrollment enrollment, EnrollmentService service) =>
        {
            var created = service.Create(enrollment);
            return Results.Created($"/api/enrollments/{created.Id}", created);
        });

        group.MapPut("/{id:guid}/status", (Guid id, UpdateStatusRequest request, EnrollmentService service) =>
        {
            var updated = service.UpdateStatus(id, request.Status);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        });
    }
}

public record UpdateStatusRequest(NNPensionPlanner.Models.EnrollmentStatus Status);
