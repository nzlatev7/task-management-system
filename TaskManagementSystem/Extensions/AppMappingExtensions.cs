using Scalar.AspNetCore;

namespace TaskManagementSystem.Extensions;

public static class AppMappingExtensions
{
    public static void MapOpenApiDisplayer(this WebApplication app)
    {
        // used to display OpenAPI documentation
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Task Management System")
                .WithTheme(ScalarTheme.BluePlanet);
        });
    }
}
