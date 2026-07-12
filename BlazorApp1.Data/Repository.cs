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

    public IQueryable<T> GetAll()
    {
        return _ctx.Set<T>().AsNoTracking();
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        return await _ctx.Set<T>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _ctx.Set<T>().AsNoTracking().ToListAsync();
    }
}
