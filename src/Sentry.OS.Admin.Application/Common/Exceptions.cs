namespace Sentry.OS.Admin.Application.Common;

/// <summary>Thrown when a requested entity does not exist, or is not visible to the caller's organization.</summary>
public class NotFoundException(string entityName, object key)
    : Exception($"{entityName} '{key}' was not found.");

/// <summary>Thrown when a mutation would violate a uniqueness or dependent-entity constraint.</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Thrown when the caller is authenticated but not authorized to perform the requested action.</summary>
public class ForbiddenException(string message) : Exception(message);
