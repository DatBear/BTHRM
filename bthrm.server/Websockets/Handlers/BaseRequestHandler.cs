using bthrm.server.Database;
using MediatR;

namespace bthrm.server.Websockets.Handlers;

public abstract class BaseRequestHandler<T> : IRequestHandler<T> where T : IRequest
{
    protected HeartRateSession Session;
    protected HeartRateSessionManager SessionManager;
    protected HeartRateRepository HeartRateRepository;

    protected BaseRequestHandler(HeartRateSession session, HeartRateSessionManager sessionManager, HeartRateRepository heartRateRepository)
    {
        Session = session;
        HeartRateRepository = heartRateRepository;
        SessionManager = sessionManager;
    }

    public abstract Task Handle(T request, CancellationToken cancellationToken);
}