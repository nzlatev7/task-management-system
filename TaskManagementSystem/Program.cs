using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskManagementSystem;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// configure db provider
var dbConnectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<TaskManagementSystemDbContext>(opt => opt.UseNpgsql(dbConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Task Management System")
        .WithTheme(ScalarTheme.BluePlanet);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
