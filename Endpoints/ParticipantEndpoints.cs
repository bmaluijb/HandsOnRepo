using NNPensionPlanner.Models;
using NNPensionPlanner.Services;

namespace NNPensionPlanner.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/participants").WithTags("Participants");

        group.MapGet("/", (ParticipantService service) =>
            Results.Ok(service.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, ParticipantService service) =>
        {
            var participant = service.GetById(id);
            return participant is not null ? Results.Ok(participant) : Results.NotFound();
        });

        group.MapPost("/", (Participant participant, ParticipantService service) =>
        {
            var created = service.Create(participant);
            return Results.Created($"/api/participants/{created.Id}", created);
        });

        group.MapPut("/{id:guid}", (Guid id, Participant participant, ParticipantService service) =>
        {
            var updated = service.Update(id, participant);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        });

        group.MapDelete("/{id:guid}", (Guid id, ParticipantService service) =>
            service.Delete(id) ? Results.NoContent() : Results.NotFound());
    }
}
