using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(TermId), "termId")]
public partial class TermEditPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private int _termId;

    public string TermId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _termId = id;
                _ = LoadTermAsync();
            }
        }
    }

    public TermEditPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddMonths(6);
    }

    private async Task LoadTermAsync()
    {
        Term? term = await _database.GetTermAsync(_termId);
        if (term is null)
            return;

        BannerTitleLabel.Text = "Edit Term";
        TitleEntry.Text = term.Title;
        StartDatePicker.Date = term.StartDate;
        EndDatePicker.Date = term.EndDate;
        DeleteButton.IsVisible = true;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Validation Error", "Term title is required.", "OK");
            return;
        }

        if (StartDatePicker.Date > EndDatePicker.Date)
        {
            await DisplayAlert("Validation Error", "Start date cannot be after end date.", "OK");
            return;
        }

        var overlappingTerm = await _database.GetOverlappingTermAsync(
            StartDatePicker.Date,
            EndDatePicker.Date,
            _termId);

        if (overlappingTerm is not null)
        {
            await DisplayAlert(
                "Validation Error",
                $"This term overlaps '{overlappingTerm.Title}'. Please choose a different date range.",
                "OK");
            return;
        }

        Term term = new()
        {
            Id = _termId,
            Title = title,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date
        };

        await _database.SaveTermAsync(term);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_termId == 0)
            return;

        Term? term = await _database.GetTermAsync(_termId);
        if (term is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Term",
            $"Delete '{term.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteTermAsync(term);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnAddNewClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TermEditPage));
    }
    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}