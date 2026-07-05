namespace Sentry.OS.Admin.Application.Features.Applications.Dtos;

public class ApplicationDto
{
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}

public class ApplicationCreateRequest
{
    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public string? Description { get; set; }
}

public class ApplicationUpdateRequest
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
