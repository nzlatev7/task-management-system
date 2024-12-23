using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.ExceptionHandlers;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.TaskDeleteStategy;
using TaskManagementSystem.Interfaces;
using TaskManagementSystem.LoggingDecorators;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Services;

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
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITasksService, TasksService>();
        services.Decorate<ITasksService, TasksServiceLoggingDecorator>();

        services.AddScoped<ICategoriesService, CategoriesSerivce>();
        services.Decorate<ICategoriesService, CategoriesServiceLoggingDecorator>();

        services.AddScoped<IReportsService, ReportsService>();
        services.AddScoped<ITaskDeleteContext, TaskDeleteContext>();

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
