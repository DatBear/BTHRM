using bthrm.core.Websockets;
using bthrm.core.Websockets.Messages;
using bthrm.server.Database;
using MediatR;
using MySqlX.XDevAPI;

namespace bthrm.server.Websockets.Handlers;

public class SetUserHandler : BaseRequestHandler<SetUserRequest>
{
    public SetUserHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository) : base(session, sessionManager, heartRateRepository)
    {
    }


    public override async Task Handle(SetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await HeartRateRepository.GetUser(request.Data);
        if (user == null) return;
        Session.UserId = request.Data;
        Session.Send(new SetUserResponse
        {
            Data = user
        });
    }
}