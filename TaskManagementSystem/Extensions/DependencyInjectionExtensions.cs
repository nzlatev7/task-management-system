using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.ExceptionHandlers;
using TaskManagementSystem.Checkers;
using TaskManagementSystem.Features.Tasks;
using TaskManagementSystem.Features.Categories;
using TaskManagementSystem.Features.Tasks.DeleteTask.Strategy;
using TaskManagementSystem.Features.Tasks.DeleteTask.Interfaces;

namespace TaskManagementSystem.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        var dbConnectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<TaskManagementSystemDbContext>(opt =>
                opt.UseLazyLoadingProxies()
                    .UseNpgsql(dbConnectionString));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryChecker, CategoryChecker>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => {
            c.RegisterServicesFromAssemblyContaining<Program>();
            c.AddBehavior<CreateTaskLoggingHandler>();
            c.AddBehavior<DeleteTaskLoggingHandler>();
            c.AddBehavior<UpdateTaskLoggingHandler>();
            c.AddBehavior<UnlockTaskLoggingHandler>();
            c.AddBehavior<CreateCategoryLoggingHandler>();
            c.AddBehavior<UpdateCategoryLoggingHandler>();
            c.AddBehavior<DeleteCategoryLoggingHandler>();
        });

        services.AddScoped<ITaskDeleteOrchestrator, TaskDeleteOrchestrator>();
        services.AddScoped<ITaskDeleteStrategy, TaskRemovingDeleteStrategy>();
        services.AddScoped<ITaskDeleteStrategy, TaskMovingDeleteStrategy>();
        services.AddScoped<ITaskDeleteStrategy, TaskLockingDeleteStrategy>();

        return services;
    }

    public static IServiceCollection AddErrorHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<NotFoundExceptionHandler>();
        services.AddExceptionHandler<BadRequestExceptionHandler>();
        services.AddExceptionHandler<ConflictExceptionHanlder>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
