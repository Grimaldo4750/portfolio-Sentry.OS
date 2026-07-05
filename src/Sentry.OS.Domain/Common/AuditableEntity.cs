namespace Sentry.OS.Domain.Common;

/// <summary>
/// Base type for mutable identity entities. Supplies the primary key, audit stamps, and a
/// concurrency token. Append-only entities (refresh tokens, audit logs) do not inherit this.
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>Sequential GUID primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>UTC timestamp when the record was created.</summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>Identifier of the actor that created the record, when known.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>UTC timestamp of the last modification, when applicable.</summary>
    public DateTime? ModifiedAtUtc { get; set; }

    /// <summary>Identifier of the actor that last modified the record, when known.</summary>
    public Guid? ModifiedBy { get; set; }

    /// <summary>Optimistic-concurrency token (SQL Server <c>rowversion</c>).</summary>
    public byte[] RowVersion { get; set; } = [];
}
