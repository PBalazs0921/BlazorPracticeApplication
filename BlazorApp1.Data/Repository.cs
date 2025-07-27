using BlazorApp1.Entities.Helper;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Data;

public class Repository<T> where T : class, IIdEntity
{
    private readonly ApplicationDbContext _ctx;

    public Repository(ApplicationDbContext ctx)
    {
        this._ctx = ctx;
    }

    public async Task CreateAsync(T entity)
    {
        _ctx.Set<T>().Add(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task CreateManyAsync(IEnumerable<T> entities)
    {
        _ctx.Set<T>().AddRange(entities);
        await _ctx.SaveChangesAsync();
    }
    public IQueryable<T> GetAll()
    {
        return _ctx.Set<T>();
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        return await _ctx.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _ctx.Set<T>().ToListAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _ctx.Set<T>().Remove(entity);
        await _ctx.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null)
        {
            _ctx.Set<T>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(T entity)
    {
        var old = await FindByIdAsync(entity.Id);
        if (old != null)
        {
            foreach (var prop in typeof(T).GetProperties())
            {
                prop.SetValue(old, prop.GetValue(entity));
            }
            _ctx.Set<T>().Update(old);
            await _ctx.SaveChangesAsync();
        }
    }
}
