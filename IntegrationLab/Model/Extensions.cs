using System.Collections.Generic;
using Avalonia.Layout;
using BaseLibrary.Model;

namespace IntegrationLab.Model;

public static class Extensions
{
    /*
    public static HorizontalAlignment GetMessageHorizontalAlignment(this Message message)
        => message.SenderId == App.CurrentDriver.UserId
            ? HorizontalAlignment.Right
            : HorizontalAlignment.Left;
    */

    public static void InsertInsteadOf<T>(this IList<T> collection, T oldItem, T newItem)
    {
        var index = collection.IndexOf(oldItem);
        collection.RemoveAt(index);
        collection.Insert(index, newItem);
    }

    // public static async void InsertInsteadOfAsync<T>(this List<T> collection, T oldItem, T newItem)
    // {
    //     lock (collection)
    //     {
    //         
    //     }
    // }
    
    
    extension(Message message)
    {
        public HorizontalAlignment GetMessageHorizontalAlignment =>
            message.SenderId == App.CurrentDriver.UserId
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
    }

    extension(Message)
    {
        public static HorizontalAlignment GetHorizAlignment =>
            HorizontalAlignment.Right;
    }
    
}