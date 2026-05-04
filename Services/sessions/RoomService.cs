using ScholaAi.Models;

namespace ScholaAi.Services.sessions
{
    /// <summary>
    /// Tracks who is in which room.
    /// Media (SDP/ICE) is handled entirely by the mediasoup Node.js server.
    /// </summary>
    public class RoomService
    {
        private readonly Dictionary<string, RoomState> _rooms = new();
        private readonly Dictionary<string, UserState> _users = new();
        private readonly object _lock = new();

        public (bool Ok, string? Error, RoomJoinedPayload? Payload)
            TryJoin(string roomId, string connectionId, string role)
        {
            lock (_lock)
            {
                if (!_rooms.ContainsKey(roomId))
                    _rooms[roomId] = new RoomState { RoomId = roomId };

                var room = _rooms[roomId];
                var existing = room.ConnectionIds.ToList();

                room.ConnectionIds.Add(connectionId);
                _users[connectionId] = new UserState
                {
                    ConnectionId = connectionId,
                    RoomId = roomId,
                    Role = role,
                };

                return (true, null, new RoomJoinedPayload
                {
                    RoomId = roomId,
                    YourId = connectionId,
                    Role = role,
                    ExistingUsers = existing,
                });
            }
        }

        public string? Remove(string connectionId)
        {
            lock (_lock)
            {
                if (!_users.TryGetValue(connectionId, out var user)) return null;
                _users.Remove(connectionId);

                if (_rooms.TryGetValue(user.RoomId, out var room))
                {
                    room.ConnectionIds.Remove(connectionId);
                    if (room.ConnectionIds.Count == 0)
                        _rooms.Remove(user.RoomId);
                }

                return user.RoomId;
            }
        }

        public RoomState? GetRoom(string roomId)
        {
            lock (_lock)
            {
                return _rooms.TryGetValue(roomId, out var room) ? room : null;
            }
        }

        public UserState? GetUser(string connectionId)
        {
            lock (_lock)
            {
                return _users.TryGetValue(connectionId, out var user) ? user : null;
            }
        }
    }
}
