using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazorApp1.Data;

public interface IUnitOfWork
{
    void Create<TEntity>(TEntity entity) where TEntity : class;
    void Remove<TEntity>(TEntity entity) where TEntity : class;
    IQueryable<TEntity> Any<TEntity>() where TEntity : class;
    Task<int> CheckStatusAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "");
}

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        ApplicationDbContext context,
        ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Create<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public IQueryable<TEntity> Any<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>().AsQueryable();
    }

    public async Task<int> CheckStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            var connectionString = _context.Database.GetConnectionString();
            var builder = new SqlConnectionStringBuilder(connectionString);
            _logger.LogError(e, "Database health check failed for {DataSource}/{Database}",
                builder.DataSource, builder.InitialCatalog);
            throw;
            //throw new SqlStatusCheckException(builder.DataSource, builder.InitialCatalog, e);
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
    {
        //using var trace = _metrics.MethodDuration(MetricsGroups.Sql, nameof(UnitOfWork), $"{nameof(SaveChangesAsync)} << {Path.GetFileNameWithoutExtension(sourceFilePath)}:{memberName}");
        _logger.LogDebug("SaveChangesAsync called from {File}:{Member}",
            Path.GetFileNameWithoutExtension(sourceFilePath), memberName);
        var affected = await _context.SaveChangesAsync(cancellationToken);
        return affected;
    }
}