using Sentry.OS.Admin.Application.Common;
using Sentry.OS.Admin.Application.Features.Organizations.Dtos;

namespace Sentry.OS.Admin.Application.Features.Organizations.ListOrganizations;

public class ListOrganizationsResponse : PagedResult<OrganizationDto>;
