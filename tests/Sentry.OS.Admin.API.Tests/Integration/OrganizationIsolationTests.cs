using Sentry.OS.Admin.Application.Common;

namespace Sentry.OS.Admin.API.Tests.Integration;

public class OrganizationIsolationTests
{
    private class FakeCurrentActor(Guid? organizationId, bool isGlobalAdministrator) : ICurrentActor
    {
        public Guid? UserId => Guid.NewGuid();
        public Guid? OrganizationId => organizationId;
        public bool IsGlobalAdministrator => isGlobalAdministrator;
        public int? HighestRoleLevel => null;
    }

    [Fact]
    public void EnsureOrganizationAccess_AllowsMatchingOrganization()
    {
        var organizationId = Guid.NewGuid();
        var actor = new FakeCurrentActor(organizationId, isGlobalAdministrator: false);

        var exception = Record.Exception(() => actor.EnsureOrganizationAccess(organizationId));

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureOrganizationAccess_RejectsMismatchedOrganization()
    {
        var actor = new FakeCurrentActor(Guid.NewGuid(), isGlobalAdministrator: false);

        Assert.Throws<ForbiddenException>(() => actor.EnsureOrganizationAccess(Guid.NewGuid()));
    }

    [Fact]
    public void EnsureOrganizationAccess_AllowsGlobalAdministratorRegardlessOfOrganization()
    {
        var actor = new FakeCurrentActor(Guid.NewGuid(), isGlobalAdministrator: true);

        var exception = Record.Exception(() => actor.EnsureOrganizationAccess(Guid.NewGuid()));

        Assert.Null(exception);
    }
}
