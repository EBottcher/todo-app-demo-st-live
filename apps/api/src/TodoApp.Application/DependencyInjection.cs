using Microsoft.Extensions.DependencyInjection;

namespace TodoApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<Services.IActivityService, Services.ActivityService>();
        services.AddSingleton<Services.ITaskService, Services.TaskService>();
        services.AddSingleton<Services.IProjectService, Services.ProjectService>();
        services.AddSingleton<Services.ILabelService, Services.LabelService>();
        services.AddSingleton<Services.ISmartFilterService, Services.SmartFilterService>();
        return services;
    }
}
