using System.Collections.Concurrent;

namespace MobileSignalR.Tools;

public class ConnectionsHandler
{
    private readonly ConcurrentDictionary<ulong, string> _users = [];

    public void AddConnection(ulong userId, string connectionId)
    {
        if (_users.ContainsKey(userId))
        {
            _users[userId] = connectionId;
        }
        else
        {
            _users.TryAdd(userId, connectionId);
        }
    }

    public void RemoveConnection(ulong userId) => _users.TryRemove(userId, out _);
    
    public void RemoveConnection(string connectionId)
    {
        var pair = _users.FirstOrDefault(x => x.Value == connectionId);
        if (pair.Value == null) return;
        _users.TryRemove(pair.Key, out _);
    }

    public string? GetConnection(ulong userId)
    {
        _users.TryGetValue(userId, out var connection);
        return connection;
    }

    public ulong? GetUserId(string connectionId) => 
        _users.FirstOrDefault(x => x.Value == connectionId).Key;

    
    /*
    private readonly ConcurrentDictionary<ulong, List<string>> _users;

    public void AddConnection(ulong userId, string connectionId)
    {
        if (_users.TryGetValue(userId, out var connections))
        {
            connections.Add(connectionId);
        }
        else
        {
            _users.TryAdd(userId, [connectionId]);
        }
    }

    public void RemoveConnection(ulong userId, string connectionId)
    {
        if (!_users.TryGetValue(userId, out var connections)) return;
        
        connections.Remove(connectionId);
        if (connections.Count == 0)
            _users.TryRemove(userId, out _);
    }

    public List<string>? GetConnections(ulong userId)
    {
        _users.TryGetValue(userId, out var connections);
        return connections;
    }

    public ulong? GetUserId(string connectionId)
    {
        _users.
        
    }
    */


}