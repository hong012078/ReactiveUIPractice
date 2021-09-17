using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUIPractice
{
    public class AppViewModel : ReactiveObject
    {
        [Reactive]
        public string SearchTerm { get; set; }

        private readonly ObservableAsPropertyHelper<IEnumerable<NugetDetailsViewModel>> searchResults;
        public IEnumerable<NugetDetailsViewModel> SearchResults => searchResults.Value;
        private readonly ObservableAsPropertyHelper<bool> isAvailable;
        public bool IsAvailable => isAvailable.Value;

        public AppViewModel()
        {
            searchResults = this.WhenAnyValue(x => x.SearchTerm)
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Select(term => term?.Trim())
                .DistinctUntilChanged()
                .Where(term => !string.IsNullOrWhiteSpace(term))
                .SelectMany(SearchNuGetPackages)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.SearchResults);
            searchResults.ThrownExceptions
                .Subscribe(Console.WriteLine);
            isAvailable = this.WhenAnyValue(x => x.SearchResults)
                .Select(results => results != null)
                .ToProperty(this, x => x.IsAvailable);
            Interaction = new Interaction<Unit, Unit>();
            OpenDialog = ReactiveCommand.Create(() =>
            {
                Interaction.Handle(Unit.Default)
                    .Subscribe();
            });
        }
        
        private async Task<IEnumerable<NugetDetailsViewModel>> SearchNuGetPackages(
            string term, CancellationToken token)
        {
            var providers = new List<Lazy<INuGetResourceProvider>>();
            providers.AddRange(Repository.Provider.GetCoreV3()); // Add v3 API support
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var source = new SourceRepository(packageSource, providers);
            ILogger logger = NullLogger.Instance;

            var filter = new SearchFilter(false);
            var resource = await source.GetResourceAsync<PackageSearchResource>().ConfigureAwait(false);
            var metadata = await resource.SearchAsync(term, filter, 0, 10, logger, token).ConfigureAwait(false);
            return metadata.Select(x => new NugetDetailsViewModel(x));
        }
        
        public ReactiveCommand<Unit, Unit> OpenDialog { get; }
        public Interaction<Unit, Unit> Interaction { get; }
    }

    public class NugetDetailsViewModel : ReactiveObject
    {
        private readonly IPackageSearchMetadata metadata;
        private readonly Uri defaultUrl;

        public NugetDetailsViewModel(IPackageSearchMetadata metadata)
        {
            this.metadata = metadata;
            defaultUrl = new Uri("https://git.io/fAlfh");
            OpenPage = ReactiveCommand.Create(() =>
            {
                Process.Start(new ProcessStartInfo(this.ProjectUrl.ToString())
                {
                    UseShellExecute = true
                });
            });
        }

        public Uri IconUrl => metadata.IconUrl ?? defaultUrl;
        public string Description => metadata.Description;
        public Uri ProjectUrl => metadata.ProjectUrl;
        public string Title => metadata.Title;
        public ReactiveCommand<Unit, Unit> OpenPage { get; }
    }
}