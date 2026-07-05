using MediatR;
using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetJwks;

public class GetJwksHandler(IJwksProvider jwksProvider) : IRequestHandler<GetJwksQuery, GetJwksResponse>
{
    public Task<GetJwksResponse> Handle(GetJwksQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(new GetJwksResponse(jwksProvider.GetPublicKeys()));
}
