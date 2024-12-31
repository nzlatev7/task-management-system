using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Constants;
using TaskManagementSystem.Database;
using Testcontainers.PostgreSql;

namespace TaskManagementSystem.Tests.Fixtures;

public sealed class TestDatabaseFixture : IAsyncLifetime
{
    private TaskManagementSystemDbContext? _dbContext;
    private readonly PostgreSqlContainer _container;

    public TestDatabaseFixture()
    {
        _container = new PostgreSqlBuilder()
            .Build();
    }

    public TaskManagementSystemDbContext DbContext 
    {
        get => _dbContext ?? throw new InvalidOperationException(ErrorMessageConstants.DbContextNotInitialized);
        private set => _dbContext = value;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        InitializeDbContext();

        await DbContext!.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.StopAsync();
    }

    private void InitializeDbContext()
    {
        var options = new DbContextOptionsBuilder<TaskManagementSystemDbContext>()
            .UseNpgsql(_container.GetConnectionString());

        DbContext = new TaskManagementSystemDbContext(options.Options);
    }
}
