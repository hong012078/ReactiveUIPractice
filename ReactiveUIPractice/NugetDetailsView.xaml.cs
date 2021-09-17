using System.Reactive.Disposables;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace ReactiveUIPractice
{
    public partial class NugetDetailsView : ReactiveUserControl<NugetDetailsViewModel>
    {
        public NugetDetailsView()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.IconUrl,
                        v => v.iconImage.Source,
                        url => url == null ? null : new BitmapImage(url))
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        vm => vm.Title,
                        v => v.titleRun.Text)
                    .DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel,
                        vm => vm.Description,
                        v => v.descriptionRun.Text)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        vm => vm.OpenPage,
                        v => v.openButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}