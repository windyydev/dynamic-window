using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DynamicIsland.UI;
using DynamicIsland.Core.Interfaces;
using DynamicIsland.Platform.Windows.Windowing;

namespace DynamicIsland.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IOverlayWindowService, OverlayWindowService>();
                services.AddSingleton<IMediaSessionService, DynamicIsland.Platform.Windows.Media.MediaSessionService>();
                services.AddSingleton<DynamicIsland.UI.ViewModels.ActivityManagerViewModel>();
                services.AddSingleton<CoreWindow>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupWindow = AppHost.Services.GetRequiredService<CoreWindow>();
        startupWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();
        base.OnExit(e);
    }
}
