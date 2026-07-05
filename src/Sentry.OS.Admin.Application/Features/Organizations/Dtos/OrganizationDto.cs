namespace Sentry.OS.Admin.Application.Features.Organizations.Dtos;

public class OrganizationDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}

public class OrganizationCreateRequest
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
}

public class OrganizationUpdateRequest
{
    public string Name { get; set; } = null!;

    public string DisplayName { get; set; } = null!;
}
