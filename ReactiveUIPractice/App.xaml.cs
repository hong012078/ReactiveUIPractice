using System.Reflection;
using System.Windows;
using Autofac;
using ReactiveUI;
using Splat;
using Splat.Autofac;

namespace ReactiveUIPractice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<MainWindow>()
                .As<IViewFor<AppViewModel>>()
                .AsSelf();
            builder.RegisterType<NugetDetailsView>()
                .As<IViewFor<NugetDetailsViewModel>>();
            builder.RegisterType<AppViewModel>();
            var autofacResolver = builder.UseAutofacDependencyResolver();
            builder.RegisterInstance(autofacResolver);
            autofacResolver.InitializeReactiveUI();
            IContainer container = builder.Build();
            var resolver = container.Resolve<AutofacDependencyResolver>();
            resolver.SetLifetimeScope(container);
            var window = container.Resolve<MainWindow>();
            window.Show();
        }
    }
}