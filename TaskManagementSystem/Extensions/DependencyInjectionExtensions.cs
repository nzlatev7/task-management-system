using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Database;
using TaskManagementSystem.ExceptionHandlers;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Interfaces;
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

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITasksService, TasksService>();
        services.AddScoped<ICategoriesService, CategoriesSerivce>();

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
