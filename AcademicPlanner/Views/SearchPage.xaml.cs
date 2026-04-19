using AcademicPlanner.Models;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class SearchPage : ContentPage
{
    private readonly SearchService _searchService;

    public SearchPage(SearchService searchService)
    {
        InitializeComponent();
        _searchService = searchService;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string query = SearchEntry.Text?.Trim() ?? string.Empty;
        ResultsCollectionView.ItemsSource = await _searchService.SearchAsync(query);
    }

    private async void OnOpenResultClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not string route)
            return;

        await Shell.Current.GoToAsync(route);
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