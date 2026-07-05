using MediatR;

namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetDiscoveryDocument;

/// <summary>Builds the OIDC discovery document (FR-013).</summary>
public record GetDiscoveryDocumentQuery : IRequest<GetDiscoveryDocumentResponse>;
