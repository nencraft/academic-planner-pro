using AcademicPlanner.Services;

namespace AcademicPlanner.Views;

public partial class LoginPage : ContentPage
{
    private readonly AuthenticationService _authService;

    public LoginPage(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnSignInClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? string.Empty;
        string password = PasswordEntry.Text ?? string.Empty;

        var result = await _authService.LoginAsync(username, password);

        if (!result.Success)
        {
            await DisplayAlert("Sign In Failed", result.ErrorMessage, "OK");
            return;
        }

        Application.Current!.Windows[0].Page = new AppShell();
    }
}