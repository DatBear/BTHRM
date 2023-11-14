using bthrm.core.Websockets;

namespace bthrm.server.Websockets;

public class HeartRateSessionManager
{
    private readonly List<HeartRateSession> _sessions = new();

    public void AddSession(HeartRateSession session)
    {
        _sessions.Add(session);
    }

    public void RemoveSession(HeartRateSession session)
    {
        _sessions.Remove(session);
    }

    public void Broadcast(int? userId, IResponsePacket message)
    {
        if (!userId.HasValue) return;

        try
        {
            var sessions = _sessions.Where(x => x.UserId == userId).ToList();
            sessions.ForEach(x => x.Send(message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error broadcasting {ex}");
        }
    }
}