using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

namespace DynamicDataTreeDemo.Items;

public sealed class TreeNodeViewModel
    : ReactiveObject,
        IDisposable
{
    public TreeNodeViewModel(
        Random                                  random,
        ViewModelFactory                        viewModelFactory,
        ISourceCache<Model, int>                itemsSource,
        Model                                   item,
        IObservableCache<Node<Model, int>, int> children)
    {
        _item = item;

        _randomizeDataCommand = ReactiveCommand.Create(() => itemsSource.AddOrUpdate(
            itemsSource.Lookup(item.Id).Value with
            {
                DataValue1 = random.NextInt64().ToString(),
                DataValue2 = random.NextInt64().ToString(),
                DataValue3 = random.NextInt64().ToString(),
                DataValue4 = random.NextInt64().ToString(),
                DataValue5 = random.NextInt64().ToString(),
                DataValue6 = random.NextInt64().ToString(),
                DataValue7 = random.NextInt64().ToString(),
                DataValue8 = random.NextInt64().ToString()
            }));

        _subscription = children.Connect()
            .TransformWithInlineUpdate(
                transformFactory:   node => viewModelFactory.CreateTreeNode(
                    itemsSource:    itemsSource,
                    item:           node.Item,
                    children:       node.Children),
                updateAction:       static (item, node) => item.Update(node.Item))
            .ObserveOn(RxApp.MainThreadScheduler)
            .SortAndBind(
                readOnlyObservableCollection:   out _children,
                comparer:                       SortExpressionComparer<TreeNodeViewModel>.Ascending(static item => item.Name))
            .Subscribe();
    }

    public ReadOnlyObservableCollection<TreeNodeViewModel> Children
        => _children;

    public string DataValue1
        => _item.DataValue1;

    public string DataValue2
        => _item.DataValue2;

    public string DataValue3
        => _item.DataValue3;

    public string DataValue4
        => _item.DataValue4;

    public string DataValue5
        => _item.DataValue5;

    public string DataValue6
        => _item.DataValue6;

    public string DataValue7
        => _item.DataValue7;

    public string DataValue8
        => _item.DataValue8;

    public string Name
        => _item.Name;

    public int? ParentId
        => _item.ParentId;

    public ReactiveCommand<Unit, Unit> RandomizeDataCommand
        => _randomizeDataCommand;

    public void Dispose()
    {
        _subscription           .Dispose();
        _randomizeDataCommand   .Dispose();
    }

    public void Update(Model item)
    {
        var oldItem = _item;
        _item = item;

        if (_item.DataValue1 != oldItem.DataValue1)
            this.RaisePropertyChanged(nameof(DataValue1));

        if (_item.DataValue2 != oldItem.DataValue2)
            this.RaisePropertyChanged(nameof(DataValue2));

        if (_item.DataValue3 != oldItem.DataValue3)
            this.RaisePropertyChanged(nameof(DataValue3));

        if (_item.DataValue4 != oldItem.DataValue4)
            this.RaisePropertyChanged(nameof(DataValue4));

        if (_item.DataValue5 != oldItem.DataValue5)
            this.RaisePropertyChanged(nameof(DataValue5));

        if (_item.DataValue6 != oldItem.DataValue6)
            this.RaisePropertyChanged(nameof(DataValue6));

        if (_item.DataValue7 != oldItem.DataValue7)
            this.RaisePropertyChanged(nameof(DataValue7));

        if (_item.DataValue8 != oldItem.DataValue8)
            this.RaisePropertyChanged(nameof(DataValue8));

        if (_item.Name != oldItem.Name)
            this.RaisePropertyChanged(nameof(Name));
    }

    private readonly ReadOnlyObservableCollection<TreeNodeViewModel>    _children;
    private readonly ReactiveCommand<Unit, Unit>                        _randomizeDataCommand;
    private readonly IDisposable                                        _subscription;

    private Model _item;
}
