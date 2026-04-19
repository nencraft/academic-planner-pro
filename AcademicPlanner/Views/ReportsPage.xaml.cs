using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class ReportsPage : ContentPage
{
    private readonly ReportService _reportService;

    public ReportsPage(ReportService reportService)
    {
        InitializeComponent();
        _reportService = reportService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        TimestampLabel.Text = $"Generated on: {DateTime.Now:MM/dd/yyyy hh:mm tt}";
        ReportCollectionView.ItemsSource = await _reportService.GetUpcomingAcademicActivityReportAsync();
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}