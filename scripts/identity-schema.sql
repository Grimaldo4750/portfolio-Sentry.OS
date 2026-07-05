IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF SCHEMA_ID(N'Auth') IS NULL EXEC(N'CREATE SCHEMA [Auth];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF SCHEMA_ID(N'Identity') IS NULL EXEC(N'CREATE SCHEMA [Identity];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF SCHEMA_ID(N'Audit') IS NULL EXEC(N'CREATE SCHEMA [Audit];');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Audit].[AuditLogs] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NULL,
        [ActorUserId] uniqueidentifier NULL,
        [ActorDisplay] nvarchar(256) NULL,
        [Action] nvarchar(100) NOT NULL,
        [TargetType] nvarchar(100) NULL,
        [TargetId] uniqueidentifier NULL,
        [DataJson] nvarchar(max) NULL,
        [IpAddress] nvarchar(64) NULL,
        [OccurredAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[Organizations] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Organizations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[Users] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [Email] nvarchar(256) NOT NULL,
        [NormalizedEmail] nvarchar(256) NOT NULL,
        [EmailVerified] bit NOT NULL,
        [UserName] nvarchar(256) NOT NULL,
        [PasswordHash] nvarchar(400) NOT NULL,
        [SecurityStamp] nvarchar(64) NOT NULL,
        [FirstName] nvarchar(100) NULL,
        [LastName] nvarchar(100) NULL,
        [ProfilePictureUrl] nvarchar(1000) NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [TwoFactorMethod] nvarchar(20) NULL,
        [PhoneNumber] nvarchar(30) NULL,
        [PhoneNumberVerified] bit NOT NULL,
        [IsDisabled] bit NOT NULL,
        [LockoutEnabled] bit NOT NULL,
        [LockoutEndUtc] datetime2 NULL,
        [AccessFailedCount] int NOT NULL,
        [LastLoginAtUtc] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[Applications] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Slug] nvarchar(100) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [DeletedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Applications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Applications_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[Roles] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [Level] int NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Roles_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[OrganizationMemberships] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [IsOrganizationAdministrator] bit NOT NULL,
        [IsHomeOrganization] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [JoinedAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_OrganizationMemberships] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrganizationMemberships_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id]),
        CONSTRAINT [FK_OrganizationMemberships_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[UserClaims] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [UserId] uniqueidentifier NOT NULL,
        [ClaimType] nvarchar(200) NOT NULL,
        [ClaimValue] nvarchar(1000) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[UserProfilePictures] (
        [UserId] uniqueidentifier NOT NULL,
        [Content] varbinary(max) NOT NULL,
        [ContentType] nvarchar(100) NOT NULL,
        [SizeBytes] int NOT NULL,
        [UploadedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_UserProfilePictures] PRIMARY KEY ([UserId]),
        CONSTRAINT [FK_UserProfilePictures_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Identity].[UserTokens] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [UserId] uniqueidentifier NOT NULL,
        [Purpose] nvarchar(30) NOT NULL,
        [TokenHash] nvarchar(200) NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [ConsumedAtUtc] datetime2 NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_UserTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[ApiResources] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_ApiResources] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ApiResources_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Identity].[Applications] ([Id]),
        CONSTRAINT [FK_ApiResources_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[Clients] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [ApplicationId] uniqueidentifier NOT NULL,
        [ClientId] nvarchar(100) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [ClientSecretHash] nvarchar(200) NULL,
        [RequirePkce] bit NOT NULL,
        [RequireClientSecret] bit NOT NULL,
        [AccessTokenLifetimeSeconds] int NOT NULL,
        [IdentityTokenLifetimeSeconds] int NOT NULL,
        [RefreshTokenLifetimeSeconds] int NOT NULL,
        [RefreshTokenRotationEnabled] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Clients] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clients_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Identity].[Applications] ([Id]),
        CONSTRAINT [FK_Clients_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[RoleAssignments] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [UserId] uniqueidentifier NOT NULL,
        [RoleId] uniqueidentifier NOT NULL,
        [OrganizationId] uniqueidentifier NOT NULL,
        [AssignedAtUtc] datetime2 NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_RoleAssignments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RoleAssignments_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id]),
        CONSTRAINT [FK_RoleAssignments_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Auth].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RoleAssignments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[Scopes] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [ApiResourceId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [DisplayName] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [CreatedBy] uniqueidentifier NULL,
        [ModifiedAtUtc] datetime2 NULL,
        [ModifiedBy] uniqueidentifier NULL,
        [RowVersion] rowversion NOT NULL,
        CONSTRAINT [PK_Scopes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Scopes_ApiResources_ApiResourceId] FOREIGN KEY ([ApiResourceId]) REFERENCES [Auth].[ApiResources] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Scopes_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[ClientCorsOrigins] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [ClientId] uniqueidentifier NOT NULL,
        [Origin] nvarchar(500) NOT NULL,
        CONSTRAINT [PK_ClientCorsOrigins] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClientCorsOrigins_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Auth].[Clients] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[ClientGrantTypes] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [ClientId] uniqueidentifier NOT NULL,
        [GrantType] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_ClientGrantTypes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClientGrantTypes_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Auth].[Clients] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[ClientRedirectUris] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [ClientId] uniqueidentifier NOT NULL,
        [Uri] nvarchar(2000) NOT NULL,
        CONSTRAINT [PK_ClientRedirectUris] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClientRedirectUris_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Auth].[Clients] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[RefreshTokens] (
        [Id] uniqueidentifier NOT NULL DEFAULT (NEWSEQUENTIALID()),
        [OrganizationId] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [ClientId] uniqueidentifier NOT NULL,
        [TokenHash] nvarchar(200) NOT NULL,
        [CreatedAtUtc] datetime2 NOT NULL,
        [ExpiresAtUtc] datetime2 NOT NULL,
        [ConsumedAtUtc] datetime2 NULL,
        [RevokedAtUtc] datetime2 NULL,
        [RevocationReason] nvarchar(50) NULL,
        [ReplacedByTokenId] uniqueidentifier NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Auth].[Clients] ([Id]),
        CONSTRAINT [FK_RefreshTokens_Organizations_OrganizationId] FOREIGN KEY ([OrganizationId]) REFERENCES [Identity].[Organizations] ([Id]),
        CONSTRAINT [FK_RefreshTokens_RefreshTokens_ReplacedByTokenId] FOREIGN KEY ([ReplacedByTokenId]) REFERENCES [Auth].[RefreshTokens] ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Identity].[Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[ClientAllowedScopes] (
        [ClientId] uniqueidentifier NOT NULL,
        [ScopeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ClientAllowedScopes] PRIMARY KEY ([ClientId], [ScopeId]),
        CONSTRAINT [FK_ClientAllowedScopes_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Auth].[Clients] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ClientAllowedScopes_Scopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [Auth].[Scopes] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE TABLE [Auth].[RoleScopes] (
        [RoleId] uniqueidentifier NOT NULL,
        [ScopeId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RoleScopes] PRIMARY KEY ([RoleId], [ScopeId]),
        CONSTRAINT [FK_RoleScopes_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Auth].[Roles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RoleScopes_Scopes_ScopeId] FOREIGN KEY ([ScopeId]) REFERENCES [Auth].[Scopes] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'DisplayName', N'IsActive', N'IsDeleted', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'Slug') AND [object_id] = OBJECT_ID(N'[Identity].[Organizations]'))
        SET IDENTITY_INSERT [Identity].[Organizations] ON;
    EXEC(N'INSERT INTO [Identity].[Organizations] ([Id], [CreatedAtUtc], [CreatedBy], [DeletedAtUtc], [DisplayName], [IsActive], [IsDeleted], [ModifiedAtUtc], [ModifiedBy], [Name], [Slug])
    VALUES (''11111111-1111-1111-1111-111111111111'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Sentry Platform'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Sentry'', N''sentry'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'DisplayName', N'IsActive', N'IsDeleted', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'Slug') AND [object_id] = OBJECT_ID(N'[Identity].[Organizations]'))
        SET IDENTITY_INSERT [Identity].[Organizations] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'Email', N'EmailVerified', N'FirstName', N'IsDeleted', N'IsDisabled', N'LastLoginAtUtc', N'LastName', N'LockoutEnabled', N'LockoutEndUtc', N'ModifiedAtUtc', N'ModifiedBy', N'NormalizedEmail', N'PasswordHash', N'PhoneNumber', N'PhoneNumberVerified', N'ProfilePictureUrl', N'SecurityStamp', N'TwoFactorEnabled', N'TwoFactorMethod', N'UserName') AND [object_id] = OBJECT_ID(N'[Identity].[Users]'))
        SET IDENTITY_INSERT [Identity].[Users] ON;
    EXEC(N'INSERT INTO [Identity].[Users] ([Id], [AccessFailedCount], [CreatedAtUtc], [CreatedBy], [DeletedAtUtc], [Email], [EmailVerified], [FirstName], [IsDeleted], [IsDisabled], [LastLoginAtUtc], [LastName], [LockoutEnabled], [LockoutEndUtc], [ModifiedAtUtc], [ModifiedBy], [NormalizedEmail], [PasswordHash], [PhoneNumber], [PhoneNumberVerified], [ProfilePictureUrl], [SecurityStamp], [TwoFactorEnabled], [TwoFactorMethod], [UserName])
    VALUES (''22222222-2222-2222-2222-222222222222'', 0, ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''admin@sentry.os'', CAST(1 AS bit), N''Sentry'', CAST(0 AS bit), CAST(0 AS bit), NULL, N''Administrator'', CAST(1 AS bit), NULL, NULL, NULL, N''ADMIN@SENTRY.OS'', N''PBKDF2.SHA256.100000$AQIDBAUGBwgJCgsMDQ4PEA==$sWL+DhI+SQS25GASsBG4DVnKPUL144v0nRCNQOhPk04='', NULL, CAST(0 AS bit), NULL, N''SEEDSTAMP0000000000000000000000A'', CAST(0 AS bit), NULL, N''admin'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'Email', N'EmailVerified', N'FirstName', N'IsDeleted', N'IsDisabled', N'LastLoginAtUtc', N'LastName', N'LockoutEnabled', N'LockoutEndUtc', N'ModifiedAtUtc', N'ModifiedBy', N'NormalizedEmail', N'PasswordHash', N'PhoneNumber', N'PhoneNumberVerified', N'ProfilePictureUrl', N'SecurityStamp', N'TwoFactorEnabled', N'TwoFactorMethod', N'UserName') AND [object_id] = OBJECT_ID(N'[Identity].[Users]'))
        SET IDENTITY_INSERT [Identity].[Users] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'Description', N'IsActive', N'IsDeleted', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId', N'Slug') AND [object_id] = OBJECT_ID(N'[Identity].[Applications]'))
        SET IDENTITY_INSERT [Identity].[Applications] ON;
    EXEC(N'INSERT INTO [Identity].[Applications] ([Id], [CreatedAtUtc], [CreatedBy], [DeletedAtUtc], [Description], [IsActive], [IsDeleted], [ModifiedAtUtc], [ModifiedBy], [Name], [OrganizationId], [Slug])
    VALUES (''44444444-4444-4444-4444-444444444444'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Administrative portal for the Sentry.OS platform.'', CAST(1 AS bit), CAST(0 AS bit), NULL, NULL, N''Sentry Admin Portal'', ''11111111-1111-1111-1111-111111111111'', N''admin-portal'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'DeletedAtUtc', N'Description', N'IsActive', N'IsDeleted', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId', N'Slug') AND [object_id] = OBJECT_ID(N'[Identity].[Applications]'))
        SET IDENTITY_INSERT [Identity].[Applications] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'IsActive', N'IsHomeOrganization', N'IsOrganizationAdministrator', N'JoinedAtUtc', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[Identity].[OrganizationMemberships]'))
        SET IDENTITY_INSERT [Identity].[OrganizationMemberships] ON;
    EXEC(N'INSERT INTO [Identity].[OrganizationMemberships] ([Id], [CreatedAtUtc], [CreatedBy], [IsActive], [IsHomeOrganization], [IsOrganizationAdministrator], [JoinedAtUtc], [ModifiedAtUtc], [ModifiedBy], [OrganizationId], [UserId])
    VALUES (''33333333-3333-3333-3333-333333333333'', ''2026-01-01T00:00:00.0000000Z'', NULL, CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, ''11111111-1111-1111-1111-111111111111'', ''22222222-2222-2222-2222-222222222222'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'IsActive', N'IsHomeOrganization', N'IsOrganizationAdministrator', N'JoinedAtUtc', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'UserId') AND [object_id] = OBJECT_ID(N'[Identity].[OrganizationMemberships]'))
        SET IDENTITY_INSERT [Identity].[OrganizationMemberships] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'Description', N'Level', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[Roles]'))
        SET IDENTITY_INSERT [Auth].[Roles] ON;
    EXEC(N'INSERT INTO [Auth].[Roles] ([Id], [CreatedAtUtc], [CreatedBy], [Description], [Level], [ModifiedAtUtc], [ModifiedBy], [Name], [OrganizationId])
    VALUES (''99999999-9999-9999-9999-999999999999'', ''2026-01-01T00:00:00.0000000Z'', NULL, N''Full administrative access within the organization.'', 100, NULL, NULL, N''OrganizationAdmin'', ''11111111-1111-1111-1111-111111111111'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAtUtc', N'CreatedBy', N'Description', N'Level', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[Roles]'))
        SET IDENTITY_INSERT [Auth].[Roles] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApplicationId', N'CreatedAtUtc', N'CreatedBy', N'DisplayName', N'IsActive', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[ApiResources]'))
        SET IDENTITY_INSERT [Auth].[ApiResources] ON;
    EXEC(N'INSERT INTO [Auth].[ApiResources] ([Id], [ApplicationId], [CreatedAtUtc], [CreatedBy], [DisplayName], [IsActive], [ModifiedAtUtc], [ModifiedBy], [Name], [OrganizationId])
    VALUES (''66666666-6666-6666-6666-666666666666'', ''44444444-4444-4444-4444-444444444444'', ''2026-01-01T00:00:00.0000000Z'', NULL, N''Sentry Admin API'', CAST(1 AS bit), NULL, NULL, N''sentry-admin-api'', ''11111111-1111-1111-1111-111111111111'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApplicationId', N'CreatedAtUtc', N'CreatedBy', N'DisplayName', N'IsActive', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[ApiResources]'))
        SET IDENTITY_INSERT [Auth].[ApiResources] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessTokenLifetimeSeconds', N'ApplicationId', N'ClientId', N'ClientSecretHash', N'CreatedAtUtc', N'CreatedBy', N'DisplayName', N'IdentityTokenLifetimeSeconds', N'IsActive', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'RefreshTokenLifetimeSeconds', N'RefreshTokenRotationEnabled', N'RequireClientSecret', N'RequirePkce') AND [object_id] = OBJECT_ID(N'[Auth].[Clients]'))
        SET IDENTITY_INSERT [Auth].[Clients] ON;
    EXEC(N'INSERT INTO [Auth].[Clients] ([Id], [AccessTokenLifetimeSeconds], [ApplicationId], [ClientId], [ClientSecretHash], [CreatedAtUtc], [CreatedBy], [DisplayName], [IdentityTokenLifetimeSeconds], [IsActive], [ModifiedAtUtc], [ModifiedBy], [OrganizationId], [RefreshTokenLifetimeSeconds], [RefreshTokenRotationEnabled], [RequireClientSecret], [RequirePkce])
    VALUES (''55555555-5555-5555-5555-555555555555'', 3600, ''44444444-4444-4444-4444-444444444444'', N''sentry-admin-portal'', NULL, ''2026-01-01T00:00:00.0000000Z'', NULL, N''Sentry Admin Portal (SPA)'', 300, CAST(1 AS bit), NULL, NULL, ''11111111-1111-1111-1111-111111111111'', 1209600, CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit))');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessTokenLifetimeSeconds', N'ApplicationId', N'ClientId', N'ClientSecretHash', N'CreatedAtUtc', N'CreatedBy', N'DisplayName', N'IdentityTokenLifetimeSeconds', N'IsActive', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'RefreshTokenLifetimeSeconds', N'RefreshTokenRotationEnabled', N'RequireClientSecret', N'RequirePkce') AND [object_id] = OBJECT_ID(N'[Auth].[Clients]'))
        SET IDENTITY_INSERT [Auth].[Clients] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssignedAtUtc', N'CreatedAtUtc', N'CreatedBy', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[Auth].[RoleAssignments]'))
        SET IDENTITY_INSERT [Auth].[RoleAssignments] ON;
    EXEC(N'INSERT INTO [Auth].[RoleAssignments] ([Id], [AssignedAtUtc], [CreatedAtUtc], [CreatedBy], [ModifiedAtUtc], [ModifiedBy], [OrganizationId], [RoleId], [UserId])
    VALUES (''dddddddd-dddd-dddd-dddd-ddddddddddd1'', ''2026-01-01T00:00:00.0000000Z'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, NULL, ''11111111-1111-1111-1111-111111111111'', ''99999999-9999-9999-9999-999999999999'', ''22222222-2222-2222-2222-222222222222'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssignedAtUtc', N'CreatedAtUtc', N'CreatedBy', N'ModifiedAtUtc', N'ModifiedBy', N'OrganizationId', N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[Auth].[RoleAssignments]'))
        SET IDENTITY_INSERT [Auth].[RoleAssignments] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'Origin') AND [object_id] = OBJECT_ID(N'[Auth].[ClientCorsOrigins]'))
        SET IDENTITY_INSERT [Auth].[ClientCorsOrigins] ON;
    EXEC(N'INSERT INTO [Auth].[ClientCorsOrigins] ([Id], [ClientId], [Origin])
    VALUES (''cccccccc-cccc-cccc-cccc-ccccccccccc1'', ''55555555-5555-5555-5555-555555555555'', N''http://localhost:5173'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'Origin') AND [object_id] = OBJECT_ID(N'[Auth].[ClientCorsOrigins]'))
        SET IDENTITY_INSERT [Auth].[ClientCorsOrigins] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'GrantType') AND [object_id] = OBJECT_ID(N'[Auth].[ClientGrantTypes]'))
        SET IDENTITY_INSERT [Auth].[ClientGrantTypes] ON;
    EXEC(N'INSERT INTO [Auth].[ClientGrantTypes] ([Id], [ClientId], [GrantType])
    VALUES (''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1'', ''55555555-5555-5555-5555-555555555555'', N''authorization_code''),
    (''aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2'', ''55555555-5555-5555-5555-555555555555'', N''refresh_token'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'GrantType') AND [object_id] = OBJECT_ID(N'[Auth].[ClientGrantTypes]'))
        SET IDENTITY_INSERT [Auth].[ClientGrantTypes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'Uri') AND [object_id] = OBJECT_ID(N'[Auth].[ClientRedirectUris]'))
        SET IDENTITY_INSERT [Auth].[ClientRedirectUris] ON;
    EXEC(N'INSERT INTO [Auth].[ClientRedirectUris] ([Id], [ClientId], [Uri])
    VALUES (''bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1'', ''55555555-5555-5555-5555-555555555555'', N''http://localhost:5173/callback'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClientId', N'Uri') AND [object_id] = OBJECT_ID(N'[Auth].[ClientRedirectUris]'))
        SET IDENTITY_INSERT [Auth].[ClientRedirectUris] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApiResourceId', N'CreatedAtUtc', N'CreatedBy', N'Description', N'DisplayName', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[Scopes]'))
        SET IDENTITY_INSERT [Auth].[Scopes] ON;
    EXEC(N'INSERT INTO [Auth].[Scopes] ([Id], [ApiResourceId], [CreatedAtUtc], [CreatedBy], [Description], [DisplayName], [ModifiedAtUtc], [ModifiedBy], [Name], [OrganizationId])
    VALUES (''77777777-7777-7777-7777-777777777777'', ''66666666-6666-6666-6666-666666666666'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Read administrative data'', NULL, NULL, N''admin.read'', ''11111111-1111-1111-1111-111111111111''),
    (''88888888-8888-8888-8888-888888888888'', ''66666666-6666-6666-6666-666666666666'', ''2026-01-01T00:00:00.0000000Z'', NULL, NULL, N''Modify administrative data'', NULL, NULL, N''admin.write'', ''11111111-1111-1111-1111-111111111111'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ApiResourceId', N'CreatedAtUtc', N'CreatedBy', N'Description', N'DisplayName', N'ModifiedAtUtc', N'ModifiedBy', N'Name', N'OrganizationId') AND [object_id] = OBJECT_ID(N'[Auth].[Scopes]'))
        SET IDENTITY_INSERT [Auth].[Scopes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClientId', N'ScopeId') AND [object_id] = OBJECT_ID(N'[Auth].[ClientAllowedScopes]'))
        SET IDENTITY_INSERT [Auth].[ClientAllowedScopes] ON;
    EXEC(N'INSERT INTO [Auth].[ClientAllowedScopes] ([ClientId], [ScopeId])
    VALUES (''55555555-5555-5555-5555-555555555555'', ''77777777-7777-7777-7777-777777777777''),
    (''55555555-5555-5555-5555-555555555555'', ''88888888-8888-8888-8888-888888888888'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ClientId', N'ScopeId') AND [object_id] = OBJECT_ID(N'[Auth].[ClientAllowedScopes]'))
        SET IDENTITY_INSERT [Auth].[ClientAllowedScopes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'ScopeId') AND [object_id] = OBJECT_ID(N'[Auth].[RoleScopes]'))
        SET IDENTITY_INSERT [Auth].[RoleScopes] ON;
    EXEC(N'INSERT INTO [Auth].[RoleScopes] ([RoleId], [ScopeId])
    VALUES (''99999999-9999-9999-9999-999999999999'', ''77777777-7777-7777-7777-777777777777''),
    (''99999999-9999-9999-9999-999999999999'', ''88888888-8888-8888-8888-888888888888'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'ScopeId') AND [object_id] = OBJECT_ID(N'[Auth].[RoleScopes]'))
        SET IDENTITY_INSERT [Auth].[RoleScopes] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ApiResources_ApplicationId_Name] ON [Auth].[ApiResources] ([ApplicationId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_ApiResources_OrganizationId] ON [Auth].[ApiResources] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Applications_OrganizationId_Slug] ON [Identity].[Applications] ([OrganizationId], [Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_AuditLogs_OrganizationId_OccurredAtUtc] ON [Audit].[AuditLogs] ([OrganizationId], [OccurredAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_ClientAllowedScopes_ScopeId] ON [Auth].[ClientAllowedScopes] ([ScopeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ClientCorsOrigins_ClientId_Origin] ON [Auth].[ClientCorsOrigins] ([ClientId], [Origin]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ClientGrantTypes_ClientId_GrantType] ON [Auth].[ClientGrantTypes] ([ClientId], [GrantType]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ClientRedirectUris_ClientId_Uri] ON [Auth].[ClientRedirectUris] ([ClientId], [Uri]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_Clients_ApplicationId] ON [Auth].[Clients] ([ApplicationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clients_ClientId] ON [Auth].[Clients] ([ClientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_Clients_OrganizationId] ON [Auth].[Clients] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OrganizationMemberships_OrganizationId_UserId] ON [Identity].[OrganizationMemberships] ([OrganizationId], [UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UX_OrganizationMemberships_HomeOrganization_PerUser] ON [Identity].[OrganizationMemberships] ([UserId]) WHERE [IsHomeOrganization] = 1');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Organizations_Slug] ON [Identity].[Organizations] ([Slug]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ClientId] ON [Auth].[RefreshTokens] ([ClientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ExpiresAtUtc] ON [Auth].[RefreshTokens] ([ExpiresAtUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_OrganizationId] ON [Auth].[RefreshTokens] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_ReplacedByTokenId] ON [Auth].[RefreshTokens] ([ReplacedByTokenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_TokenHash] ON [Auth].[RefreshTokens] ([TokenHash]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId_ClientId] ON [Auth].[RefreshTokens] ([UserId], [ClientId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RoleAssignments_OrganizationId] ON [Auth].[RoleAssignments] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RoleAssignments_RoleId] ON [Auth].[RoleAssignments] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_RoleAssignments_UserId_RoleId] ON [Auth].[RoleAssignments] ([UserId], [RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Roles_OrganizationId_Name] ON [Auth].[Roles] ([OrganizationId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_RoleScopes_ScopeId] ON [Auth].[RoleScopes] ([ScopeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Scopes_ApiResourceId_Name] ON [Auth].[Scopes] ([ApiResourceId], [Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_Scopes_OrganizationId] ON [Auth].[Scopes] ([OrganizationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_UserClaims_UserId_ClaimType_ClaimValue] ON [Identity].[UserClaims] ([UserId], [ClaimType], [ClaimValue]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Identity].[Users] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_NormalizedEmail] ON [Identity].[Users] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_UserName] ON [Identity].[Users] ([UserName]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    CREATE INDEX [IX_UserTokens_UserId_Purpose] ON [Identity].[UserTokens] ([UserId], [Purpose]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260705032018_InitialIdentitySchema'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260705032018_InitialIdentitySchema', N'10.0.9');
END;

COMMIT;
GO

