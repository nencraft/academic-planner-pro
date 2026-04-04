using Microsoft.Extensions.Logging;
using AcademicPlanner.Data;
using AcademicPlanner.Views;
using AcademicPlanner.Services;

namespace AcademicPlanner;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<AcademicPlannerDatabase>();
        builder.Services.AddSingleton<INotificationManagerService, AcademicPlanner.Platforms.Android.NotificationManagerService>();
        builder.Services.AddSingleton<SeedDataService>();

        builder.Services.AddTransient<TermsPage>();
        builder.Services.AddTransient<TermEditPage>();
        builder.Services.AddTransient<TermOverviewPage>();
        builder.Services.AddTransient<CourseEditPage>();
        builder.Services.AddTransient<CourseOverviewPage>();
        builder.Services.AddTransient<AssessmentEditPage>();


        return builder.Build();
    }
}