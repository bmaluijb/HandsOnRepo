using System.Collections.Concurrent;
using System.Reflection;

namespace NNPensionPlanner.Data;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    private readonly ConcurrentDictionary<Guid, T> _store = new();

    public IEnumerable<T> GetAll() => _store.Values.ToList();

    public T? GetById(Guid id) => _store.GetValueOrDefault(id);

    public T Add(T entity)
    {
        var id = GetId(entity);
        if (id == Guid.Empty)
        {
            SetId(entity, Guid.NewGuid());
            id = GetId(entity);
        }
        _store[id] = entity;
        return entity;
    }

    public T Update(T entity)
    {
        var id = GetId(entity);
        _store[id] = entity;
        return entity;
    }

    public bool Delete(Guid id) => _store.TryRemove(id, out _);

    public IEnumerable<T> Find(Func<T, bool> predicate) => _store.Values.Where(predicate).ToList();

    public void Seed(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            var id = GetId(entity);
            _store[id] = entity;
        }
    }

    private static Guid GetId(T entity)
    {
        var prop = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        return prop?.GetValue(entity) is Guid guid ? guid : Guid.Empty;
    }

    private static void SetId(T entity, Guid id)
    {
        var prop = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
        prop?.SetValue(entity, id);
    }
}
