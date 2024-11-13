using System;

using DynamicData;

namespace DynamicDataTreeDemo.Items;

public class ViewModelFactory
{
    public ViewModelFactory(Random random)
        => _random = random;

    public TreeNodeViewModel CreateTreeNode(
            ISourceCache<Model, int>                itemsSource,
            Model                                   item,
            IObservableCache<Node<Model, int>, int> children)
        => new(
            random:             _random,
            viewModelFactory:   this,
            itemsSource:        itemsSource,
            item:               item,
            children:           children);

    private readonly Random _random;
}
