using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class SettingsPage : ContentPage
{
    private readonly AuthenticationService _authService;

    public SettingsPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshUiAsync();
    }

    private async Task RefreshUiAsync()
    {
        bool isEnabled = await _authService.IsPinEnabledAsync();

        PinStatusLabel.Text = isEnabled
            ? "PIN unlock is currently enabled."
            : "PIN unlock is currently disabled.";

        SavePinButton.Text = isEnabled
            ? "Save New PIN"
            : "Enable PIN Unlock";


        DisablePinButton.IsVisible = isEnabled;
    }

    private async void OnSavePinClicked(object sender, EventArgs e)
    {
        string pin = PinEntry.Text?.Trim() ?? string.Empty;
        bool wasEnabled = await _authService.IsPinEnabledAsync();

        var result = await _authService.SetPinAsync(pin);

        if (!result.Success)
        {
            await DisplayAlert("PIN Error", result.ErrorMessage, "OK");
            return;
        }

        PinEntry.Text = string.Empty;

        await DisplayAlert(
            "Success",
            wasEnabled ? "A new PIN has been saved." : "PIN unlock has been enabled.",
            "OK");

        await RefreshUiAsync();
    }

    private async void OnDisablePinClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Disable PIN",
            "Are you sure you want to disable PIN unlock?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await _authService.DisablePinAsync();
        await DisplayAlert("Success", "PIN unlock has been disabled.", "OK");
        await RefreshUiAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}