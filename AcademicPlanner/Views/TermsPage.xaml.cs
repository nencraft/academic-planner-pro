using AcademicPlanner.Data;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class TermsPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private readonly SeedDataService _seedDataService;
    private bool _initialized;

    public TermsPage(AcademicPlannerDatabase database, SeedDataService seedDataService)
    {
        InitializeComponent();
        _database = database;
        _seedDataService = seedDataService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_initialized)
        {
            _initialized = true;
            await _seedDataService.SeedAsync();
        }

        await LoadTermsAsync();
    }

    private async Task LoadTermsAsync()
    {
        TermsCollectionView.ItemsSource = await _database.GetTermsAsync();
    }

    private async void OnAddTermClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TermEditPage));
    }

    private async void OnTermSelectedClicked(object? sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is not int termId)
            return;

        await Shell.Current.GoToAsync($"{nameof(TermOverviewPage)}?termId={termId}");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Task.CompletedTask;
    }

    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}