using AcademicPlanner.Views;

namespace AcademicPlanner;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TermEditPage), typeof(TermEditPage));
        Routing.RegisterRoute(nameof(TermOverviewPage), typeof(TermOverviewPage));
        Routing.RegisterRoute(nameof(CourseEditPage), typeof(CourseEditPage));
        Routing.RegisterRoute(nameof(CourseOverviewPage), typeof(CourseOverviewPage));
        Routing.RegisterRoute(nameof(AssessmentEditPage), typeof(AssessmentEditPage));

    }
}
