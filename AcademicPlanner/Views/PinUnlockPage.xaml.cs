using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class PinUnlockPage : ContentPage
{
    private readonly AuthenticationService _authService;

    public PinUnlockPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnUnlockClicked(object sender, EventArgs e)
    {
        string pin = PinEntry.Text?.Trim() ?? string.Empty;

        var result = await _authService.VerifyPinAsync(pin);

        if (!result.Success)
        {
            await DisplayAlert("Unlock Failed", result.ErrorMessage, "OK");
            return;
        }

        Application.Current!.Windows[0].Page = new AppShell();
    }

    private void OnUsePasswordClicked(object sender, EventArgs e)
    {
        Application.Current!.Windows[0].Page = new LoginPage(_authService);
    }
}