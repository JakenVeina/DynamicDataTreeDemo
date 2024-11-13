using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace DynamicDataTreeDemo.Items;

public sealed class TreeViewModel
    : ReactiveObject,
        IDisposable
{
    public TreeViewModel(
        Random              random,
        ViewModelFactory    viewModelFactory)
    {
        _itemsSource = new SourceCache<Model, int>(static item => item.Id);
        
        _loadCommand = ReactiveCommand.Create(static () => { });

        _subscriptions = new()
        {
            _itemsSource.Connect()
                .Filter(
                    predicateChanged:   this.WhenAnyValue(static item => item.SearchPattern)
                        .Throttle(TimeSpan.FromSeconds(1))
                        .Prepend(null)
                        .Do(_ => IsLoading = true)
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .DistinctUntilChanged()
                        .Select(static searchPattern => new Func<Model, bool>(item => 
                                string.IsNullOrWhiteSpace(searchPattern)
                            ||  item.Id.ToString().Contains(searchPattern, StringComparison.OrdinalIgnoreCase) 
                            ||  (item.ParentId?.ToString().Contains(searchPattern, StringComparison.OrdinalIgnoreCase) ?? false)
                            ||  item.Name.Contains(searchPattern, StringComparison.OrdinalIgnoreCase))))
                .TransformToTree(static item => item.ParentId ?? 0)
                .TransformWithInlineUpdate(
                    transformFactory:   node => viewModelFactory.CreateTreeNode(
                        itemsSource:    _itemsSource,
                        item:           node.Item,
                        children:       node.Children),
                    updateAction:       static (item, node) => item.Update(node.Item))
                .ObserveOn(RxApp.MainThreadScheduler)
                .SortAndBind(
                    readOnlyObservableCollection:   out var filteredItems,
                    comparer:                       SortExpressionComparer<TreeNodeViewModel>.Ascending(static item => item.Name))
                .Do(_ => IsLoading = false)
                .Subscribe(),

            _loadCommand
                .Do(_ => IsLoading = true)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Delay(TimeSpan.FromSeconds(1))
                .Do(_ =>
                {
                    using var suspension = _itemsSource.SuspendNotifications();

                    _itemsSource.Clear();

                    _itemsSource.AddOrUpdate(Enumerable.Range(1, 65_000)
                        .Select(id => 
                        {
                            int? parentId = ((id % 1000) == 0)
                                ? null
                                : random.Next(0, id);

                            return new Model()
                            {
                                Id          = id,
                                ParentId    = (parentId is 0)
                                    ? null
                                    : parentId,
                                Name        = $"Item #{id}",
                                DataValue1  = random.NextInt64().ToString(),
                                DataValue2  = random.NextInt64().ToString(),
                                DataValue3  = random.NextInt64().ToString(),
                                DataValue4  = random.NextInt64().ToString(),
                                DataValue5  = random.NextInt64().ToString(),
                                DataValue6  = random.NextInt64().ToString(),
                                DataValue7  = random.NextInt64().ToString(),
                                DataValue8  = random.NextInt64().ToString()
                            };
                        }));
                })
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => IsLoading = false),

            _loadCommand
                .Execute()
                .Subscribe()
        };

        _items = new(filteredItems)
        {
            Columns =
            {
                new HierarchicalExpanderColumn<TreeNodeViewModel>(
                    inner:          new TextColumn<TreeNodeViewModel, string>(
                        header: "Name",
                        getter: item => item.Name,
                        width:  new GridLength(3, GridUnitType.Star)),
                    childSelector:  item => item.Children),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #1",
                    getter: item => item.DataValue1,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #2",
                    getter: item => item.DataValue2,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #3",
                    getter: item => item.DataValue3,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #4",
                    getter: item => item.DataValue4,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #5",
                    getter: item => item.DataValue5,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #6",
                    getter: item => item.DataValue6,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #7",
                    getter: item => item.DataValue7,
                    width:  new GridLength(1, GridUnitType.Star)),
                new TextColumn<TreeNodeViewModel, string>(
                    header: "Data Value #8",
                    getter: item => item.DataValue8,
                    width:  new GridLength(1, GridUnitType.Star))
            }
        };
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public HierarchicalTreeDataGridSource<TreeNodeViewModel> Items
        => _items;

    public ReactiveCommand<Unit, Unit> LoadCommand
        => _loadCommand;

    public string? SearchPattern
    {
        get => _searchPattern;
        set => this.RaiseAndSetIfChanged(ref _searchPattern, value);
    }

    public void Dispose()
    {
        _loadCommand    .Dispose();
        _items          .Dispose();
        _itemsSource    .Dispose();
        _subscriptions  .Dispose();
    }

    private readonly ReactiveCommand<Unit, Unit>                _loadCommand;
    private readonly HierarchicalTreeDataGridSource<TreeNodeViewModel>  _items;
    private readonly SourceCache<Model, int>                    _itemsSource;
    private readonly CompositeDisposable                        _subscriptions;

    private bool    _isLoading;
    private string? _searchPattern;
}
