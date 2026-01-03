using FamilyPlacement.Abstractions;
using FamilyPlacement.Services;
using FamilyPlacement.ViewModels;
using FamilyPlacement.Views;
using Microsoft.Extensions.DependencyInjection;
using RxBim.Di;
using UIFramework;

namespace FamalyPlacment
{
    internal class Config : ICommandConfiguration
    {
        public void Configure(IServiceCollection services)
        {
            services.AddSingleton<IPlacementService, PlacementService>();
            services.AddSingleton<MainWindowViewModel, MainWindowViewModel>();
            services.AddSingleton<MainWindow, MainWindow>();
        }
    }
}
