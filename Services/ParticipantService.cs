using NNPensionPlanner.Data;
using NNPensionPlanner.Models;

namespace NNPensionPlanner.Services;

public class ParticipantService
{
    private readonly IRepository<Participant> _repository;
    private readonly ILogger<ParticipantService> _logger;

    public ParticipantService(IRepository<Participant> repository, ILogger<ParticipantService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IEnumerable<Participant> GetAll() => _repository.GetAll();

    public Participant? GetById(Guid id) => _repository.GetById(id);

    public Participant Create(Participant participant)
    {
        Validate(participant);
        var created = _repository.Add(participant);
        _logger.LogInformation("Created participant {Name} ({Id})", created.FullName, created.Id);
        return created;
    }

    public Participant? Update(Guid id, Participant updated)
    {
        var existing = _repository.GetById(id);
        if (existing is null) return null;

        existing.FirstName = updated.FirstName;
        existing.LastName = updated.LastName;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.Email = updated.Email;
        existing.EmployerName = updated.EmployerName;
        existing.AnnualSalary = updated.AnnualSalary;

        Validate(existing);
        _repository.Update(existing);
        _logger.LogInformation("Updated participant {Id}", id);
        return existing;
    }

    public bool Delete(Guid id)
    {
        var result = _repository.Delete(id);
        if (result) _logger.LogInformation("Deleted participant {Id}", id);
        return result;
    }

    private static void Validate(Participant participant)
    {
        if (string.IsNullOrWhiteSpace(participant.FirstName))
            throw new ArgumentException("First name is required.");
        if (string.IsNullOrWhiteSpace(participant.LastName))
            throw new ArgumentException("Last name is required.");
        if (participant.DateOfBirth > DateTime.UtcNow.AddYears(-18))
            throw new ArgumentException("Participant must be at least 18 years old.");
        if (participant.AnnualSalary < 0)
            throw new ArgumentException("Annual salary cannot be negative.");
    }
}
