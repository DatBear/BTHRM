using bthrm.core.Websockets.Messages;
using bthrm.server.Database;

namespace bthrm.server.Websockets.Handlers;

public class StopSessionHandler : BaseRequestHandler<StopSessionRequest>
{
    public StopSessionHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }

    public override async Task Handle(StopSessionRequest request, CancellationToken cancellationToken)
    {
        if (!Session.UserId.HasValue) return;

        var currentSession = await HeartRateRepository.GetCurrentSession(Session.UserId);
        if(currentSession == null) return;

        currentSession.EndDate = DateTime.UtcNow;

        var session = await HeartRateRepository.StopSession(currentSession);
        SessionManager.Broadcast(Session.UserId, new StopSessionResponse()
        {
            Data = session
        });

    }
}