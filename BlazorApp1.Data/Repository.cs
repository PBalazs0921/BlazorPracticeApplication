using BlazorApp1.Entities.Helper;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Data;

public class Repository<T> where T : class, IIdEntity
{
    private readonly ApplicationDbContext ctx;

    public Repository(ApplicationDbContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task CreateAsync(T entity)
    {
        ctx.Set<T>().Add(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task CreateManyAsync(IEnumerable<T> entities)
    {
        ctx.Set<T>().AddRange(entities);
        await ctx.SaveChangesAsync();
    }
    public IQueryable<T> GetAll()
    {
        return ctx.Set<T>();
    }

    public async Task<T?> FindByIdAsync(int id)
    {
        return await ctx.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await ctx.Set<T>().ToListAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        ctx.Set<T>().Remove(entity);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteByIdAsync(int id)
    {
        var entity = await FindByIdAsync(id);
        if (entity != null)
        {
            ctx.Set<T>().Remove(entity);
            await ctx.SaveChangesAsync();
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
            ctx.Set<T>().Update(old);
            await ctx.SaveChangesAsync();
        }
    }
}
