using System;

using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using DynamicDataTreeDemo.Items;

using Microsoft.Extensions.DependencyInjection;

namespace DynamicDataTreeDemo;

public partial class Application
    : Avalonia.Application
{
    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var rootServiceProvider = new ServiceCollection()
                .AddSingleton(new Random(1234567))
                .AddItems()
                .BuildServiceProvider(new ServiceProviderOptions()
                {
                    ValidateOnBuild = true,
                    ValidateScopes  = true
                });

            desktop.MainWindow = new Window()
            {
                Content = new Items.TreeView()
                {
                    DataContext = rootServiceProvider.GetRequiredService<Items.TreeViewModel>()
                },
                Title   = "Dynamic Data Tree Demo"
            };

            desktop.MainWindow.Closing += (_, _) => rootServiceProvider.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
