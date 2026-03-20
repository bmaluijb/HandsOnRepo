using System.Text.Json;
using System.Text.Json.Serialization;
using NNPensionPlanner.Data;
using NNPensionPlanner.Endpoints;
using NNPensionPlanner.Events;
using NNPensionPlanner.Middleware;
using NNPensionPlanner.Models;
using NNPensionPlanner.Services;

var builder = WebApplication.CreateBuilder(args);

// JSON serialization with enums as strings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// Data repositories (singleton — in-memory store)
builder.Services.AddSingleton<InMemoryRepository<Participant>>();
builder.Services.AddSingleton<IRepository<Participant>>(sp => sp.GetRequiredService<InMemoryRepository<Participant>>());

builder.Services.AddSingleton<InMemoryRepository<PensionPlan>>();
builder.Services.AddSingleton<IRepository<PensionPlan>>(sp => sp.GetRequiredService<InMemoryRepository<PensionPlan>>());

builder.Services.AddSingleton<InMemoryRepository<Enrollment>>();
builder.Services.AddSingleton<IRepository<Enrollment>>(sp => sp.GetRequiredService<InMemoryRepository<Enrollment>>());

builder.Services.AddSingleton<InMemoryRepository<Contribution>>();
builder.Services.AddSingleton<IRepository<Contribution>>(sp => sp.GetRequiredService<InMemoryRepository<Contribution>>());

builder.Services.AddSingleton<InMemoryRepository<Projection>>();
builder.Services.AddSingleton<IRepository<Projection>>(sp => sp.GetRequiredService<InMemoryRepository<Projection>>());

// Event bus
builder.Services.AddSingleton<EventBus>();

// Services
builder.Services.AddSingleton<ParticipantService>();
builder.Services.AddSingleton<EnrollmentService>();
builder.Services.AddSingleton<ContributionService>();
builder.Services.AddSingleton<ProjectionService>();

var app = builder.Build();

// Middleware pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseDefaultFiles();
app.UseStaticFiles();

// API endpoints
app.MapParticipantEndpoints();
app.MapPlanEndpoints();
app.MapEnrollmentEndpoints();
app.MapContributionEndpoints();
app.MapProjectionEndpoints();

// Seed data on startup
SeedData.Initialize(
    app.Services.GetRequiredService<InMemoryRepository<Participant>>(),
    app.Services.GetRequiredService<InMemoryRepository<PensionPlan>>(),
    app.Services.GetRequiredService<InMemoryRepository<Enrollment>>(),
    app.Services.GetRequiredService<InMemoryRepository<Contribution>>());

app.Run();
