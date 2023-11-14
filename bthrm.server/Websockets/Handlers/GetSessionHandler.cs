using bthrm.core.Websockets.Messages;
using bthrm.server.Database;

namespace bthrm.server.Websockets.Handlers;

public class GetSessionHandler : BaseRequestHandler<GetSessionRequest>
{
    public GetSessionHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }

    public override async Task Handle(GetSessionRequest request, CancellationToken cancellationToken)
    {
        if (request.Data.HasValue)
        {
            var session = await HeartRateRepository.GetSessionDetails(request.Data.Value);
            if (session == null) return;
            if (session.EndDate == null)
            {
                Session.UserId = session.UserId;
            }
            Session.Send(new GetSessionResponse
            {
                Data = session
            });
        }
        else
        {
            if (!Session.UserId.HasValue) return;
            var currentSession = HeartRateRepository.GetCurrentSession(Session.UserId);
            var session = await HeartRateRepository.GetSessionDetails(currentSession.Id);
            if (session.EndDate == null)
            {
                Session.UserId = session.UserId;
            }
            Session.Send(new GetSessionResponse
            {
                Data = session
            });
        }
        
    }
}