using AcademicPlanner.Data;
using AcademicPlanner.Helpers;
using AcademicPlanner.Models;
using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

[QueryProperty(nameof(TermId), "termId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseEditPage : ContentPage
{
    private readonly AcademicPlannerDatabase _database;
    private readonly INotificationManagerService _notificationService;
    private bool _optionsLoaded;

    private int _termId;
    private int _courseId;

    public string TermId
    {
        set
        {
            if (int.TryParse(Uri.UnescapeDataString(value), out int id))
            {
                _termId = id;
            }
        }
    }

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

    public CourseEditPage(AcademicPlannerDatabase database, INotificationManagerService notificationService)
    {
        InitializeComponent();
        _database = database;
        _notificationService = notificationService;

        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today.AddMonths(1);

        StatusPicker.SelectedIndex = -1;
        AlertSettingPicker.SelectedIndex = 0;
    }

    private async Task LoadCourseAsync()
    {
        await EnsurePickerOptionsLoadedAsync();

        var course = await _database.GetCourseAsync(_courseId);
        if (course is null)
            return;

        _termId = course.TermId;

        BannerTitleLabel.Text = "Edit Course";

        TitleEntry.Text = course.Title;
        StartDatePicker.Date = course.StartDate;
        EndDatePicker.Date = course.EndDate;

        if (!string.IsNullOrWhiteSpace(course.Status))
            StatusPicker.SelectedItem = course.Status;

        InstructorNameEntry.Text = course.InstructorName;
        InstructorPhoneEntry.Text = course.InstructorPhone;
        InstructorEmailEntry.Text = course.InstructorEmail;
        NotesEditor.Text = course.Notes;

        if (!string.IsNullOrWhiteSpace(course.AlertSetting))
            AlertSettingPicker.SelectedItem = course.AlertSetting;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string title = TitleEntry.Text?.Trim() ?? string.Empty;
        string instructorName = InstructorNameEntry.Text?.Trim() ?? string.Empty;
        string instructorPhone = InstructorPhoneEntry.Text?.Trim() ?? string.Empty;
        string instructorEmail = InstructorEmailEntry.Text?.Trim() ?? string.Empty;
        string notes = NotesEditor.Text?.Trim() ?? string.Empty;
        string status = StatusPicker.SelectedItem?.ToString() ?? string.Empty;
        string alertSetting = AlertSettingPicker.SelectedItem?.ToString() ?? "None";

        if (!ValidationHelper.IsRequired(title))
        {
            await DisplayAlert("Validation Error", "Course title is required.", "OK");
            return;
        }

        if (!ValidationHelper.DatesAreValid(StartDatePicker.Date, EndDatePicker.Date))
        {
            await DisplayAlert("Validation Error", "Start date cannot be after end date.", "OK");
            return;
        }

        if (!ValidationHelper.IsRequired(status))
        {
            await DisplayAlert("Validation Error", "Course status is required.", "OK");
            return;
        }

        if (!ValidationHelper.IsRequired(instructorName))
        {
            await DisplayAlert("Validation Error", "Instructor name is required.", "OK");
            return;
        }

        if (!ValidationHelper.IsRequired(instructorPhone))
        {
            await DisplayAlert("Validation Error", "Instructor phone is required.", "OK");
            return;
        }

        if (!ValidationHelper.IsRequired(instructorEmail))
        {
            await DisplayAlert("Validation Error", "Instructor email is required.", "OK");
            return;
        }

        if (!ValidationHelper.IsValidEmail(instructorEmail))
        {
            await DisplayAlert("Validation Error", "Enter a valid instructor email.", "OK");
            return;
        }

        Course course = new()
        {
            Id = _courseId,
            TermId = _termId,
            Title = title,
            StartDate = StartDatePicker.Date,
            EndDate = EndDatePicker.Date,
            Status = status,
            InstructorName = instructorName,
            InstructorPhone = instructorPhone,
            InstructorEmail = instructorEmail,
            Notes = notes,
            AlertSetting = alertSetting
        };

        if (course.AlertSetting != "None")
        {
#if ANDROID
            var permission = await Permissions.RequestAsync<AcademicPlanner.Platforms.Android.NotificationPermission>();
            if (permission != PermissionStatus.Granted)
            {
                await DisplayAlert("Notifications", "Notification permission was not granted.", "OK");
            }
            else
            {
                if (course.AlertSetting == "Start" || course.AlertSetting == "Start and End")
                {
                    _notificationService.SendNotification(
                        "Course Start Reminder",
                        $"{course.Title} starts today.",
                        course.StartDate.Date.AddHours(9));
                }

                if (course.AlertSetting == "End" || course.AlertSetting == "Start and End")
                {
                    _notificationService.SendNotification(
                        "Course End Reminder",
                        $"{course.Title} ends today.",
                        course.EndDate.Date.AddHours(9));
                }
            }
#endif
        }

        await _database.SaveCourseAsync(course);
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
        await Shell.Current.GoToAsync($"{nameof(CourseEditPage)}?termId={_termId}");
    }
    private async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TermsPage");
    }

    private async Task EnsurePickerOptionsLoadedAsync()
    {
        if (_optionsLoaded)
            return;

        var statusOptions = await _database.GetCourseStatusOptionsAsync();
        StatusPicker.ItemsSource = statusOptions.Select(o => o.Name).ToList();

        var alertOptions = await _database.GetAlertOptionsAsync();
        AlertSettingPicker.ItemsSource = alertOptions.Select(o => o.Name).ToList();

        _optionsLoaded = true;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsurePickerOptionsLoadedAsync();
    }
}