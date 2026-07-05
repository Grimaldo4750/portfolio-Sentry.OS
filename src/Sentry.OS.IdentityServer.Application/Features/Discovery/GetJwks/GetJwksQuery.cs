using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetJwks;

/// <summary>Projects the IdP's currently-published public signing keys (FR-014).</summary>
public record GetJwksQuery : IRequest<GetJwksResponse>;
