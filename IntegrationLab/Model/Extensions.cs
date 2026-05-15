using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IntegrationLab.Model;

public static class Extensions
{
    public static void InsertInsteadOf<T>(this IList<T> collection, T oldItem, T newItem)
    {
        var index = collection.IndexOf(oldItem);
        collection.RemoveAt(index);
        collection.Insert(index, newItem);
    }

    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> newItems)
    {
        foreach (var newItem in newItems) collection.Add(newItem);
    }
}