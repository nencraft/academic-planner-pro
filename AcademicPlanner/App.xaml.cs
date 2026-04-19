using AcademicPlanner.Services;
using AcademicPlanner.Views;

namespace AcademicPlanner;

public partial class App : Application
{
    private readonly AuthenticationService _authService;

    public App(AuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Page startupPage = new ContentPage();
        var window = new Window(startupPage);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool hasAccount = await _authService.HasAccountAsync();

            if (!hasAccount)
            {
                window.Page = new SetupAccountPage(_authService);
                return;
            }

            bool isPinEnabled = await _authService.IsPinEnabledAsync();

            window.Page = isPinEnabled
                ? new PinUnlockPage(_authService)
                : new LoginPage(_authService);
        });

        return window;
    }
}