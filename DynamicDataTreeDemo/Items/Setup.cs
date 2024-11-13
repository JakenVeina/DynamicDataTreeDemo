using Microsoft.Extensions.DependencyInjection;

namespace DynamicDataTreeDemo.Items;

public static class Setup
{
    public static IServiceCollection AddItems(this IServiceCollection services)
        => services
            .AddSingleton<ViewModelFactory>()
            .AddTransient<TreeViewModel>();
}
