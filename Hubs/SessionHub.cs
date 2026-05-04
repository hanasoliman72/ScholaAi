using ScholaAi.Services.sessions;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace ScholaAi.Hubs
{
    /// <summary>
    /// SessionHub — room presence only.
    /// No SDP or ICE candidate exchange here; mediasoup handles all media.
    /// </summary>
    public class SessionHub : Hub
    {
        private readonly RoomService _rooms;
        private readonly ILogger<SessionHub> _logger;

        public SessionHub(RoomService rooms, ILogger<SessionHub> logger)
        {
            _rooms = rooms;
            _logger = logger;
        }

        /// <summary>Join a room as "host" or "viewer".</summary>
        public async Task JoinRoom(string roomId, string role = "viewer")
        {
            var id = Context.ConnectionId;
            var (ok, error, payload) = _rooms.TryJoin(roomId, id, role);

            if (!ok)
            {
                await Clients.Caller.SendAsync("Error", error);
                return;
            }

            await Groups.AddToGroupAsync(id, roomId);
            await Clients.Caller.SendAsync("RoomJoined", payload);
            await Clients.OthersInGroup(roomId).SendAsync("PeerJoined", new { socketId = id, role });

            _logger.LogInformation("{Id} joined room {Room} as {Role}", id, roomId, role);
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            var id = Context.ConnectionId;
            var roomId = _rooms.Remove(id);

            if (roomId is not null)
            {
                await Clients.OthersInGroup(roomId).SendAsync("PeerLeft", id);
                _logger.LogInformation("{Id} left room {Room}", id, roomId);
            }

            await base.OnDisconnectedAsync(ex);
        }

        public async Task StudentDistracted(string roomId, string reason)
        {
            var room = _rooms.GetRoom(roomId);
            if (room == null) return;

            foreach (var connId in room.ConnectionIds)
            {
                var user = _rooms.GetUser(connId);
                if (user?.Role == "host")
                {
                    await Clients.Client(connId).SendAsync("DistractionAlert", reason);
                }
            }
            _logger.LogInformation("Distraction in room {Room}: {Reason}", roomId, reason);
        }
    }
}
