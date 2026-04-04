using AcademicPlanner.Data;
using AcademicPlanner.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.ApplicationModel;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseOverviewPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private int _courseId;
    private Course? _course;


    public string CourseId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _courseId = id;
                _ = LoadCourseAsync();
            }
        }
    }

    public CourseOverviewPage(AcademicPlannerDatabase database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_courseId != 0)
            await LoadCourseAsync();
    }

    private async Task LoadCourseAsync()
    {
        _course = await _database.GetCourseAsync(_courseId);
        if (_course is null)
            return;

        BannerTitleLabel.Text = $"{_course.Title} Overview";
        CourseStatusLabel.Text = _course.Status;
        CourseStartDateLabel.Text = _course.StartDate.ToString("MM/dd/yyyy");
        CourseEndDateLabel.Text = _course.EndDate.ToString("MM/dd/yyyy");
        InstructorNameLabel.Text = _course.InstructorName;
        InstructorPhoneLabel.Text = _course.InstructorPhone;
        InstructorEmailLabel.Text = _course.InstructorEmail;
        NotesLabel.Text = string.IsNullOrWhiteSpace(_course.Notes)
            ? "No notes added."
            : _course.Notes;

        BannerBellImage.Source = _course.AlertSetting == "None"
            ? "icon_bell_inactive_white.png"
            : "icon_bell_active_white.png";

        AssessmentsCollectionView.ItemsSource = await _database.GetAssessmentsByCourseAsync(_courseId);
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnEditCourseClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(CourseEditPage)}?courseId={_courseId}");
    }

    private async void OnDeleteCourseClicked(object? sender, EventArgs e)
    {
        if (_course is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Course",
            $"Delete '{_course.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteCourseCascadeAsync(_courseId);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnShareNotesClicked(object? sender, EventArgs e)
    {
        if (_course is null || string.IsNullOrWhiteSpace(_course.Notes))
        {
            await DisplayAlert("Share Notes", "There are no notes to share.", "OK");
            return;
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = _course.Notes,
            Title = "Course Notes"
        });
    }

    private async void OnAddAssessmentClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?courseId={_courseId}");
    }

    private async void OnEditAssessmentClicked(object? sender, EventArgs e)
    {
        if (sender is not ImageButton button || button.CommandParameter is not int assessmentId)
            return;

        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?assessmentId={assessmentId}");
    }
    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
    private async void OnCallInstructorClicked(object? sender, EventArgs e)
    {
        if (_course is null || string.IsNullOrWhiteSpace(_course.InstructorPhone))
            return;

        await Launcher.Default.OpenAsync(new Uri($"tel:{_course.InstructorPhone}"));
    }

    private async void OnEmailInstructorClicked(object? sender, EventArgs e)
    {
        if (_course is null || string.IsNullOrWhiteSpace(_course.InstructorEmail))
            return;

        await Launcher.Default.OpenAsync(new Uri($"mailto:{_course.InstructorEmail}"));
    }
}