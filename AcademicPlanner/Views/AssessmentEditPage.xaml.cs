using AcademicPlanner.Data;
using AcademicPlanner.Helpers;
using AcademicPlanner.Models;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssessmentId), "assessmentId")]
public partial class AssessmentEditPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private readonly INotificationManagerService _notificationService;

    private int _courseId;
    private int _assessmentId;

    public string CourseId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _courseId = id;
            }
        }
    }

    public string AssessmentId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _assessmentId = id;
                _ = LoadAssessmentAsync();
            }
        }
    }

    public AssessmentEditPage(
        AcademicPlannerDatabase database,
        INotificationManagerService notificationService)
    {
        InitializeComponent();
        _database = database;
        _notificationService = notificationService;

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddMonths(1);

        AssessmentTypePicker.SelectedIndex = -1;
        AlertSettingPicker.SelectedIndex = 0; 
    }

    private async Task LoadAssessmentAsync()
    {
        var assessment = await _database.GetAssessmentAsync(_assessmentId);
        if (assessment is null)
            return;

        _courseId = assessment.CourseId;

        BannerTitleLabel.Text = "Edit Assessment";
        TitleEntry.Text = assessment.Title;
        StartDatePicker.Date = assessment.StartDate;
        EndDatePicker.Date = assessment.EndDate;

        if (!string.IsNullOrWhiteSpace(assessment.Type))
            AssessmentTypePicker.SelectedItem = assessment.Type;

        if (!string.IsNullOrWhiteSpace(assessment.AlertSetting))
            AlertSettingPicker.SelectedItem = assessment.AlertSetting;

        DeleteButton.IsVisible = true;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        string type = AssessmentTypePicker.SelectedItem?.ToString() ?? string.Empty;
        string alertSetting = AlertSettingPicker.SelectedItem?.ToString() ?? "None";

        if (!ValidationHelper.IsRequired(title))
        {
            await DisplayAlert("Validation Error", "Assessment title is required.", "OK");
            return;
        }

        if (!ValidationHelper.IsRequired(type))
        {
            await DisplayAlert("Validation Error", "Assessment type is required.", "OK");
            return;
        }

        if (!ValidationHelper.DatesAreValid(StartDatePicker.Date, EndDatePicker.Date))
        {
            await DisplayAlert("Validation Error", "Start date cannot be after end date.", "OK");
            return;
        }

        var existingAssessments = await _database.GetAssessmentsByCourseAsync(_courseId);

        if (_assessmentId == 0 && !ValidationHelper.CanAddAnotherAssessment(existingAssessments))
        {
            await DisplayAlert(
                "Assessment Limit",
                "Each course can only have two assessments: one Objective and one Performance.",
                "OK");
            return;
        }

        if (ValidationHelper.HasDuplicateAssessmentType(existingAssessments, type, _assessmentId))
        {
            await DisplayAlert(
                "Duplicate Assessment Type",
                $"This course already has a {type} assessment.",
                "OK");
            return;
        }

        Assessment assessment = new()
        {
            Id = _assessmentId,
            CourseId = _courseId,
            Title = title,
            Type = type,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date,
            AlertSetting = alertSetting
        };

        await _database.SaveAssessmentAsync(assessment);

#if ANDROID
        if (assessment.AlertSetting != "None")
        {
            var permission = await Permissions.RequestAsync<AcademicPlanner.Platforms.Android.NotificationPermission>();
            if (permission == PermissionStatus.Granted)
            {
                if (assessment.AlertSetting == "Start" || assessment.AlertSetting == "Start and End")
                {
                    _notificationService.SendNotification(
                        "Assessment Start Reminder",
                        $"{assessment.Title} starts today.",
                        assessment.StartDate.Date.AddHours(9));
                }

                if (assessment.AlertSetting == "End" || assessment.AlertSetting == "Start and End")
                {
                    _notificationService.SendNotification(
                        "Assessment Due Reminder",
                        $"{assessment.Title} is due today.",
                        assessment.EndDate.Date.AddHours(9));
                }
            }
        }
#endif

        await Shell.Current.GoToAsync("..");
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_assessmentId == 0)
            return;

        var assessment = await _database.GetAssessmentAsync(_assessmentId);
        if (assessment is null)
            return;

        bool confirm = await DisplayAlert(
            "Delete Assessment",
            $"Delete '{assessment.Title}'?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _database.DeleteAssessmentAsync(assessment);
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
        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?courseId={_courseId}");
    }
    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }
}