using Microsoft.Extensions.DependencyInjection;
using TodoApp.Application.Abstractions;
using TodoApp.Domain.Abstractions;
using TodoApp.Infrastructure.Hosted;
using TodoApp.Infrastructure.Persistence;
using TodoApp.Infrastructure.Time;

namespace TodoApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        services.AddSingleton<IProjectRepository, InMemoryProjectRepository>();
        services.AddSingleton<ILabelRepository, InMemoryLabelRepository>();
        services.AddSingleton<IActivityRepository, InMemoryActivityRepository>();
        services.AddHostedService<ReminderBackgroundService>();
        return services;
    }
}
