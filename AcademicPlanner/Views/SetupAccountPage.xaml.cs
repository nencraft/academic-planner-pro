using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class SetupAccountPage : ContentPage
{
    private readonly AuthenticationService _authService;

    public SetupAccountPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;
        string confirmPassword = ConfirmPasswordEntry.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username))
        {
            await DisplayAlert("Validation Error", "Username is required.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validation Error", "Password is required.", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Validation Error", "Passwords do not match.", "OK");
            return;
        }

        var result = await _authService.CreateAccountAsync(username, password);

        if (!result.Success)
        {
            await DisplayAlert("Account Error", result.ErrorMessage, "OK");
            return;
        }

        Application.Current!.Windows[0].Page = new AppShell();
    }
}