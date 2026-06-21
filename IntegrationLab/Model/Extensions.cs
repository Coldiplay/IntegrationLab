using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BaseLibrary.Model.Classes;

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

    public static bool RemoveChat(this ConcurrentDictionary<Chat, (ObservableCollection<User> members, ObservableCollection<Message> messages)> collection, ulong chatId)
    {
        var pair = collection.FirstOrDefault(c => c.Key.Id == chatId).Key;
        return collection.Remove(pair, out _);
    }   

    public static Chat? GetChat(this ConcurrentDictionary<Chat, (ObservableCollection<User> members, ObservableCollection<Message> messages)> collection, ulong chatId)
    {
        return collection.FirstOrDefault(c => c.Key.Id == chatId).Key;
    }
    
    public static (ObservableCollection<User> members, ObservableCollection<Message> messages)? GetChatData(this ConcurrentDictionary<Chat, (ObservableCollection<User> members, ObservableCollection<Message> messages)> collection, ulong chatId)
        => collection.FirstOrDefault(c => c.Key.Id == chatId).Value;
}