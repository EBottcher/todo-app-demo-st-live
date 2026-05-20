using System.Text.Json.Serialization;
using TodoApp.Application;
using TodoApp.Application.Dtos;
using TodoApp.Application.Services;
using TodoApp.Domain.Enums;
using TodoApp.Domain.ValueObjects;
using TodoApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsPolicy = "spa";
builder.Services.AddCors(opts =>
{
    opts.AddPolicy(CorsPolicy, p => p
        .WithOrigins("http://localhost:5173", "http://localhost:4173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services
    .AddInfrastructure()
    .AddApplication();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsPolicy);
app.MapControllers();

// Optional seed for dev
if (app.Environment.IsDevelopment() &&
    string.Equals(app.Configuration["Seed"], "true", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var projects = scope.ServiceProvider.GetRequiredService<IProjectService>();
    var labels = scope.ServiceProvider.GetRequiredService<ILabelService>();
    var tasks = scope.ServiceProvider.GetRequiredService<ITaskService>();

    var inbox = projects.Create(new CreateProjectDto("Inbox", "#3b82f6"));
    var work = projects.Create(new CreateProjectDto("Work", "#ef4444"));
    var urgent = labels.Create(new CreateLabelDto("urgent", "#dc2626"));

    var today = DateTime.UtcNow.Date.AddHours(12);
    tasks.Create(new CreateTaskDto(
        "Welcome to TodoApp",
        "Try editing this task or creating a new one.",
        TodoPriority.Medium,
        inbox.Id,
        Array.Empty<Guid>(),
        today));
    tasks.Create(new CreateTaskDto(
        "Daily standup",
        "Recurring sample",
        TodoPriority.High,
        work.Id,
        new[] { urgent.Id },
        today.AddHours(-3),
        today.AddHours(-3).AddMinutes(-15),
        new RecurrenceRule(RecurrenceFrequency.Daily, 1)));
    tasks.Create(new CreateTaskDto(
        "Overdue example",
        null,
        TodoPriority.Low,
        inbox.Id,
        Array.Empty<Guid>(),
        DateTime.UtcNow.Date.AddDays(-2)));
}

app.Run();

public partial class Program;
