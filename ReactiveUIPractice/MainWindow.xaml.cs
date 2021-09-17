using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autofac;
using ReactiveUI;

namespace ReactiveUIPractice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<AppViewModel>
    {
        public MainWindow(AppViewModel viewModel, ILifetimeScope container)
        {
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.IsAvailable,
                        v => v.searchResultsListBox.Visibility)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        vm => vm.SearchResults,
                        v => v.searchResultsListBox.ItemsSource)
                    .DisposeWith(disposableRegistration);
                this.Bind(ViewModel,
                        vm => vm.SearchTerm,
                        v => v.searchTextBox.Text)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        vm => vm.OpenDialog,
                        v => v.OpenButton)
                    .DisposeWith(disposableRegistration);
                disposableRegistration.Add(ViewModel.Interaction.RegisterHandler(handler =>
                {
                    MainWindow window = container.Resolve<MainWindow>();
                    window.Owner = this;
                    window.Show();
                    handler.SetOutput(Unit.Default);
                }));
            });
            
        }
    }
}