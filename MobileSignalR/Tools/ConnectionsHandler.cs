using System.Collections.Concurrent;

namespace MobileSignalR.Tools;

public class ConnectionsHandler
{
    //UserId (1) - (m) connectionID
    private readonly ConcurrentDictionary<Guid, List<string>> _users = [];

    public void AddConnection(Guid userId, string connectionId)
    {
        _ = _users.AddOrUpdate(userId, [connectionId], (guid, list) => {
            if (!list.Contains(connectionId))
                list.Add(connectionId);
            return list;
        });
    }

    public bool RemoveConnection(Guid userId, string connectionId)
    {
        if (!_users.TryGetValue(userId, out var list)) return false;
        
        list.Remove(connectionId);
        if (list.Count == 0)
            _users.TryRemove(userId, out _);
        return true;
    }

    public Guid? GetUserId(string connectionId) =>
        _users.FirstOrDefault(u => u.Value.Contains(connectionId)).Key;
    public List<string>? GetConnections(Guid userId)
    {
        _users.TryGetValue(userId, out var list);
        return list?.ToList();
    }
}