using bthrm.core.Websockets.Messages;
using bthrm.server.Database;

namespace bthrm.server.Websockets.Handlers;

public class GetUserHandler : BaseRequestHandler<GetUserRequest>
{
    public GetUserHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }

    public override async Task Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await HeartRateRepository.GetUser(request.Data);
        if (user != null)
        {
            Session.Send(new GetUserResponse
            {
                Data = user
            });
        }
    }
}