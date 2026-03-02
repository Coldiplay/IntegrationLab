using Avalonia.Layout;
using Models.Model;

namespace IntegrationLab.Model;

public static class Extensions
{
    /*
    public static HorizontalAlignment GetMessageHorizontalAlignment(this Message message)
        => message.SenderId == App.CurrentDriver.UserId
            ? HorizontalAlignment.Right
            : HorizontalAlignment.Left;
    */
    
    
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