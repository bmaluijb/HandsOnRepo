namespace NNPensionPlanner.Data;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    T Add(T entity);
    T Update(T entity);
    bool Delete(Guid id);
    IEnumerable<T> Find(Func<T, bool> predicate);
}
