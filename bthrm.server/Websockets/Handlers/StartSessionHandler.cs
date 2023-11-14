using bthrm.core.Websockets.Messages;
using bthrm.server.Database;

namespace bthrm.server.Websockets.Handlers;

public class StartSessionHandler : BaseRequestHandler<StartSessionRequest>
{
    public StartSessionHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }

    public override async Task Handle(StartSessionRequest request, CancellationToken cancellationToken)
    {
        if (!Session.UserId.HasValue) return;

        var currentSession = await HeartRateRepository.GetCurrentSession(Session.UserId);
        if (currentSession != null)
        {
            Session.Send(new GetSessionResponse
            {
                Data = (await HeartRateRepository.GetSessionDetails(currentSession.Id))!
            });
            return;
        }

        var newSession = await HeartRateRepository.CreateSession(new core.Data.Model.HeartRateSession
        {
            UserId = Session.UserId.Value,
            StartDate = request.Data.StartDate
        });

        Session.Send(new GetSessionResponse
        {
            Data = newSession
        });
        Console.WriteLine($"Started new session: {newSession.Id}");
    }
}