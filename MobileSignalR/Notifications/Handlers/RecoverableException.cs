namespace MobileSignalR.Notifications.Handlers;

// Простая обёртка для временных ошибок, чтобы ре-куеить сообщение
public class RecoverableException(string message, Exception? inner = null) : Exception(message, inner);