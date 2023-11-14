using bthrm.core.Data.Model;
using bthrm.core.Websockets.Messages;
using bthrm.server.Database;
using MediatR;

namespace bthrm.server.Websockets.Handlers;

public class AddHeartRateReadingHandler : BaseRequestHandler<AddHeartRateReadingRequest>
{
    public AddHeartRateReadingHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }

    public override async Task Handle(AddHeartRateReadingRequest request, CancellationToken cancellationToken)
    {
        if (Session.UserId == null) return;
        var heartRateSession = await HeartRateRepository.GetCurrentSession(Session.UserId.Value);
        if (heartRateSession == null) return;

        var reading = new HeartRateReading
        {
            HeartRateSessionId = heartRateSession.Id,
            Date = request.Data.Date,
            HeartRate = request.Data.HeartRate
        };
        await HeartRateRepository.CreateReading(reading);
        SessionManager.Broadcast(Session.UserId.Value, new GetHeartRateResponse
        {
            Data = reading
        });
        Console.WriteLine($"Created Reading: {reading.HeartRate}");
    }
}