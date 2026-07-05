using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sentry.OS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Auth");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Audit");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ActorDisplay = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TargetType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PhoneNumberVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsDisabled = table.Column<bool>(type: "bit", nullable: false),
                    IsGlobalAdministrator = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEndUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Applications_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMemberships",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsOrganizationAdministrator = table.Column<bool>(type: "bit", nullable: false),
                    IsHomeOrganization = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    JoinedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMemberships_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfilePictures",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<int>(type: "int", nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfilePictures", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfilePictures_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResources",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResources_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Identity",
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApiResources_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ClientSecretHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RequirePkce = table.Column<bool>(type: "bit", nullable: false),
                    RequireClientSecret = table.Column<bool>(type: "bit", nullable: false),
                    AccessTokenLifetimeSeconds = table.Column<int>(type: "int", nullable: false),
                    IdentityTokenLifetimeSeconds = table.Column<int>(type: "int", nullable: false),
                    RefreshTokenLifetimeSeconds = table.Column<int>(type: "int", nullable: false),
                    RefreshTokenRotationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "Identity",
                        principalTable: "Applications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Clients_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleAssignments",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAssignments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleAssignments_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApiResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scopes_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalSchema: "Auth",
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Scopes_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientCorsOrigins",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCorsOrigins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientCorsOrigins_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientGrantTypes",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGrantTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientRedirectUris",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRedirectUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientRedirectUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "Auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConsumedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevocationReason = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ReplacedByTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Auth",
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Identity",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RefreshTokens_RefreshTokens_ReplacedByTokenId",
                        column: x => x.ReplacedByTokenId,
                        principalSchema: "Auth",
                        principalTable: "RefreshTokens",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClientAllowedScopes",
                schema: "Auth",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedScopes", x => new { x.ClientId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_ClientAllowedScopes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "Auth",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientAllowedScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "Auth",
                        principalTable: "Scopes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleScopes",
                schema: "Auth",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleScopes", x => new { x.RoleId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_RoleScopes_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Auth",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "Auth",
                        principalTable: "Scopes",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Organizations",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "DisplayName", "IsActive", "IsDeleted", "ModifiedAtUtc", "ModifiedBy", "Name", "Slug" },
                values: new object[] { new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Acron", true, false, null, null, "Acron", "acron" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "Email", "EmailVerified", "FirstName", "IsDeleted", "IsDisabled", "IsGlobalAdministrator", "LastLoginAtUtc", "LastName", "LockoutEnabled", "LockoutEndUtc", "ModifiedAtUtc", "ModifiedBy", "NormalizedEmail", "PasswordHash", "PhoneNumber", "PhoneNumberVerified", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "TwoFactorMethod", "UserName" },
                values: new object[] { new Guid("e23b2eae-0a19-4e08-b752-282af674137a"), 0, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "c_grimaldo@outlook.com", true, "Christian", false, false, true, null, "Grimaldo", true, null, null, null, "C_GRIMALDO@OUTLOOK.COM", "PBKDF2.SHA256.100000$vZ9+wS/g0hfSNvcAAizprg==$1AAjcmDjarHZQNAyaJ5vJF6v1wur5LJ0yb52HAeXFVs=", null, false, null, "SEEDSTAMP11062f87a73b41f6a26e6d580aeb02a9", false, null, "c_grimaldo" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "Applications",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "DeletedAtUtc", "Description", "IsActive", "IsDeleted", "ModifiedAtUtc", "ModifiedBy", "Name", "OrganizationId", "Slug" },
                values: new object[] { new Guid("0b12880d-dc23-4f74-a28f-f71525390a9c"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "The single web application used to sign in and administer Sentry.OS.", true, false, null, null, "Sentry Management Web App", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8"), "sentry-management-web-app" });

            migrationBuilder.InsertData(
                schema: "Identity",
                table: "OrganizationMemberships",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "IsActive", "IsHomeOrganization", "IsOrganizationAdministrator", "JoinedAtUtc", "ModifiedAtUtc", "ModifiedBy", "OrganizationId", "UserId" },
                values: new object[] { new Guid("fa1d0cb9-6f57-442d-bab0-7c43079cb7a8"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, true, true, true, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8"), new Guid("e23b2eae-0a19-4e08-b752-282af674137a") });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "Roles",
                columns: new[] { "Id", "CreatedAtUtc", "CreatedBy", "Description", "Level", "ModifiedAtUtc", "ModifiedBy", "Name", "OrganizationId" },
                values: new object[] { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "Full administrative access to the Sentry Management API.", 100, null, null, "GlobalAdministrator", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "ApiResources",
                columns: new[] { "Id", "ApplicationId", "CreatedAtUtc", "CreatedBy", "DisplayName", "IsActive", "ModifiedAtUtc", "ModifiedBy", "Name", "OrganizationId" },
                values: new object[] { new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new Guid("0b12880d-dc23-4f74-a28f-f71525390a9c"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sentry Management API", true, null, null, "api-sentry-management", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "Clients",
                columns: new[] { "Id", "AccessTokenLifetimeSeconds", "ApplicationId", "ClientId", "ClientSecretHash", "CreatedAtUtc", "CreatedBy", "DisplayName", "IdentityTokenLifetimeSeconds", "IsActive", "ModifiedAtUtc", "ModifiedBy", "OrganizationId", "RefreshTokenLifetimeSeconds", "RefreshTokenRotationEnabled", "RequireClientSecret", "RequirePkce" },
                values: new object[] { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), 3600, new Guid("0b12880d-dc23-4f74-a28f-f71525390a9c"), "sentry-management-web-app", null, new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sentry Management Web App (SPA)", 300, true, null, null, new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8"), 1209600, true, false, true });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "RoleAssignments",
                columns: new[] { "Id", "AssignedAtUtc", "CreatedAtUtc", "CreatedBy", "ModifiedAtUtc", "ModifiedBy", "OrganizationId", "RoleId", "UserId" },
                values: new object[] { new Guid("e07b9119-aaa4-4d10-9026-5968402243ce"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8"), new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("e23b2eae-0a19-4e08-b752-282af674137a") });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "ClientCorsOrigins",
                columns: new[] { "Id", "ClientId", "Origin" },
                values: new object[] { new Guid("184172f9-0490-4bb0-906e-65a1bf1e9fb4"), new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), "http://localhost:5173" });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "ClientGrantTypes",
                columns: new[] { "Id", "ClientId", "GrantType" },
                values: new object[,]
                {
                    { new Guid("09717d58-4a26-4945-9020-3f44d409bcc0"), new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), "authorization_code" },
                    { new Guid("3ef56dae-8cb1-465c-a011-7c66054fc362"), new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), "refresh_token" }
                });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "ClientRedirectUris",
                columns: new[] { "Id", "ClientId", "Uri" },
                values: new object[] { new Guid("86d91a06-f0bc-4550-bff0-8538c99b538c"), new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), "http://localhost:5173/callback" });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "Scopes",
                columns: new[] { "Id", "ApiResourceId", "CreatedAtUtc", "CreatedBy", "Description", "DisplayName", "ModifiedAtUtc", "ModifiedBy", "Name", "OrganizationId" },
                values: new object[,]
                {
                    { new Guid("01ab320f-5bbc-4c68-a91d-e578b4501d75"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage users and role assignments", null, null, "users.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("1c954386-ac5d-45cf-94ff-8595fdaccb76"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage roles", null, null, "roles.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("42a42a3e-d8d1-42d2-894e-9151e69b0c2e"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage applications", null, null, "applications.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("91710057-3024-42c6-8e53-f2a7958b9e00"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Read the audit log", null, null, "audit.read", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("95bcf91b-28b0-494a-ac55-c0d9cd328298"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage OAuth clients", null, null, "clients.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("b22ed850-e3ee-4831-8edb-9cb1b882a03c"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage API resources and scopes", null, null, "resources.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") },
                    { new Guid("be5eadc2-d9e1-4bf1-93c0-0e29f2016f92"), new Guid("d642f40e-bbef-4f01-b75c-f3ab939b240f"), new DateTime(2026, 7, 5, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "Manage organizations", null, null, "organizations.manage", new Guid("02ab59f7-88da-4a57-b351-eea5207f34b8") }
                });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "ClientAllowedScopes",
                columns: new[] { "ClientId", "ScopeId" },
                values: new object[,]
                {
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("01ab320f-5bbc-4c68-a91d-e578b4501d75") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("1c954386-ac5d-45cf-94ff-8595fdaccb76") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("42a42a3e-d8d1-42d2-894e-9151e69b0c2e") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("91710057-3024-42c6-8e53-f2a7958b9e00") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("95bcf91b-28b0-494a-ac55-c0d9cd328298") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("b22ed850-e3ee-4831-8edb-9cb1b882a03c") },
                    { new Guid("88a5a3f3-a5a8-4ed2-ad22-34181ff54a4f"), new Guid("be5eadc2-d9e1-4bf1-93c0-0e29f2016f92") }
                });

            migrationBuilder.InsertData(
                schema: "Auth",
                table: "RoleScopes",
                columns: new[] { "RoleId", "ScopeId" },
                values: new object[,]
                {
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("01ab320f-5bbc-4c68-a91d-e578b4501d75") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("1c954386-ac5d-45cf-94ff-8595fdaccb76") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("42a42a3e-d8d1-42d2-894e-9151e69b0c2e") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("91710057-3024-42c6-8e53-f2a7958b9e00") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("95bcf91b-28b0-494a-ac55-c0d9cd328298") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("b22ed850-e3ee-4831-8edb-9cb1b882a03c") },
                    { new Guid("f76fd1c9-48d6-4381-81cf-290dc89caad7"), new Guid("be5eadc2-d9e1-4bf1-93c0-0e29f2016f92") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiResources_ApplicationId_Name",
                schema: "Auth",
                table: "ApiResources",
                columns: new[] { "ApplicationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiResources_OrganizationId",
                schema: "Auth",
                table: "ApiResources",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OrganizationId_Slug",
                schema: "Identity",
                table: "Applications",
                columns: new[] { "OrganizationId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OrganizationId_OccurredAtUtc",
                schema: "Audit",
                table: "AuditLogs",
                columns: new[] { "OrganizationId", "OccurredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ClientAllowedScopes_ScopeId",
                schema: "Auth",
                table: "ClientAllowedScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCorsOrigins_ClientId_Origin",
                schema: "Auth",
                table: "ClientCorsOrigins",
                columns: new[] { "ClientId", "Origin" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId_GrantType",
                schema: "Auth",
                table: "ClientGrantTypes",
                columns: new[] { "ClientId", "GrantType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientRedirectUris_ClientId_Uri",
                schema: "Auth",
                table: "ClientRedirectUris",
                columns: new[] { "ClientId", "Uri" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ApplicationId",
                schema: "Auth",
                table: "Clients",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ClientId",
                schema: "Auth",
                table: "Clients",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_OrganizationId",
                schema: "Auth",
                table: "Clients",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMemberships_OrganizationId_UserId",
                schema: "Identity",
                table: "OrganizationMemberships",
                columns: new[] { "OrganizationId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_OrganizationMemberships_HomeOrganization_PerUser",
                schema: "Identity",
                table: "OrganizationMemberships",
                column: "UserId",
                unique: true,
                filter: "[IsHomeOrganization] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Slug",
                schema: "Identity",
                table: "Organizations",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ClientId",
                schema: "Auth",
                table: "RefreshTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAtUtc",
                schema: "Auth",
                table: "RefreshTokens",
                column: "ExpiresAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_OrganizationId",
                schema: "Auth",
                table: "RefreshTokens",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ReplacedByTokenId",
                schema: "Auth",
                table: "RefreshTokens",
                column: "ReplacedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                schema: "Auth",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_ClientId",
                schema: "Auth",
                table: "RefreshTokens",
                columns: new[] { "UserId", "ClientId" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_OrganizationId",
                schema: "Auth",
                table: "RoleAssignments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_RoleId",
                schema: "Auth",
                table: "RoleAssignments",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignments_UserId_RoleId",
                schema: "Auth",
                table: "RoleAssignments",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrganizationId_Name",
                schema: "Auth",
                table: "Roles",
                columns: new[] { "OrganizationId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleScopes_ScopeId",
                schema: "Auth",
                table: "RoleScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_ApiResourceId_Name",
                schema: "Auth",
                table: "Scopes",
                columns: new[] { "ApiResourceId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Scopes_OrganizationId",
                schema: "Auth",
                table: "Scopes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId_ClaimType_ClaimValue",
                schema: "Identity",
                table: "UserClaims",
                columns: new[] { "UserId", "ClaimType", "ClaimValue" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Identity",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "Identity",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTokens_UserId_Purpose",
                schema: "Identity",
                table: "UserTokens",
                columns: new[] { "UserId", "Purpose" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "ClientAllowedScopes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "ClientCorsOrigins",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "ClientGrantTypes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "ClientRedirectUris",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "OrganizationMemberships",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleAssignments",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "RoleScopes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserProfilePictures",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ApiResources",
                schema: "Auth");

            migrationBuilder.DropTable(
                name: "Applications",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "Identity");
        }
    }
}
