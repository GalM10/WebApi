using System;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Mdb.Desktop.ViewModels;
using Mdb.Desktop.Views;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;

namespace Mdb.Desktop;

public partial class App : Application
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton<HttpClient>(sp =>
            new HttpClient { BaseAddress = new Uri("http://localhost:5101") });
        serviceCollection.AddSingleton(_ => new MainWindow());
        serviceCollection.AddSingleton<Window>(sp => sp.GetRequiredService<MainWindow>());
        serviceCollection.AddSingleton<MainWindowViewModel>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            serviceCollection.AddSingleton(desktopLifetime);
        }
        
        ServiceProvider = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
    }
}