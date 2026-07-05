using Sentry.OS.IdentityServer.Application.Common;

namespace Sentry.OS.IdentityServer.Application.Features.Discovery.GetJwks;

public record GetJwksResponse(IReadOnlyList<JsonWebKeyData> Keys);
