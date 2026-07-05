using MediatR;
using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.Application.Features.Organizations.ListOrganizations;

public class ListOrganizationsQuery : PagingRequest, IRequest<ListOrganizationsResponse>;
