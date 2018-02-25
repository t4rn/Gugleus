grant all on database erexus to erexus;
grant gugleus to erexus;

  -- Table: public."AspNetUsers"

-- DROP TABLE public."AspNetUsers";

CREATE TABLE public."AspNetUsers"
(
  "Id" text NOT NULL,
  "AccessFailedCount" integer NOT NULL,
  "ConcurrencyStamp" text,
  "Email" character varying(256),
  "EmailConfirmed" boolean NOT NULL,
  "LockoutEnabled" boolean NOT NULL,
  "LockoutEnd" timestamp with time zone,
  "NormalizedEmail" character varying(256),
  "NormalizedUserName" character varying(256),
  "PasswordHash" text,
  "PhoneNumber" text,
  "PhoneNumberConfirmed" boolean NOT NULL,
  "SecurityStamp" text,
  "TwoFactorEnabled" boolean NOT NULL,
  "UserName" character varying(256),
  CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetUsers" OWNER TO erexus;
  
  -- Table: public."AspNetRoles"

-- DROP TABLE public."AspNetRoles";

CREATE TABLE public."AspNetRoles"
(
  "Id" text NOT NULL,
  "ConcurrencyStamp" text,
  "Name" character varying(256),
  "NormalizedName" character varying(256),
  CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetRoles" OWNER TO erexus;
  
-- Table: public."AspNetRoleClaims"

-- DROP TABLE public."AspNetRoleClaims";

CREATE TABLE public."AspNetRoleClaims"
(
  "Id" serial NOT NULL,
  "ClaimType" text,
  "ClaimValue" text,
  "RoleId" text NOT NULL,
  CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
  CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId")
      REFERENCES public."AspNetRoles" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetRoleClaims" OWNER TO erexus;

-- Index: public."IX_AspNetRoleClaims_RoleId"

-- DROP INDEX public."IX_AspNetRoleClaims_RoleId";

CREATE INDEX "IX_AspNetRoleClaims_RoleId"
  ON public."AspNetRoleClaims"
  USING btree
  ("RoleId" COLLATE pg_catalog."default");




-- Index: public."RoleNameIndex"

-- DROP INDEX public."RoleNameIndex";

CREATE UNIQUE INDEX "RoleNameIndex"
  ON public."AspNetRoles"
  USING btree
  ("NormalizedName" COLLATE pg_catalog."default");


  -- Table: public."AspNetUserClaims"

-- DROP TABLE public."AspNetUserClaims";

CREATE TABLE public."AspNetUserClaims"
(
  "Id" serial NOT NULL,
  "ClaimType" text,
  "ClaimValue" text,
  "UserId" text NOT NULL,
  CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
  CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId")
      REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetUserClaims" OWNER TO erexus;

-- Index: public."IX_AspNetUserClaims_UserId"

-- DROP INDEX public."IX_AspNetUserClaims_UserId";

CREATE INDEX "IX_AspNetUserClaims_UserId"
  ON public."AspNetUserClaims"
  USING btree
  ("UserId" COLLATE pg_catalog."default");

  -- Table: public."AspNetUserLogins"

-- DROP TABLE public."AspNetUserLogins";

CREATE TABLE public."AspNetUserLogins"
(
  "LoginProvider" text NOT NULL,
  "ProviderKey" text NOT NULL,
  "ProviderDisplayName" text,
  "UserId" text NOT NULL,
  CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
  CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId")
      REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetUserLogins" OWNER TO erexus;

-- Index: public."IX_AspNetUserLogins_UserId"

-- DROP INDEX public."IX_AspNetUserLogins_UserId";

CREATE INDEX "IX_AspNetUserLogins_UserId"
  ON public."AspNetUserLogins"
  USING btree
  ("UserId" COLLATE pg_catalog."default");


  -- Table: public."AspNetUserRoles"

-- DROP TABLE public."AspNetUserRoles";

CREATE TABLE public."AspNetUserRoles"
(
  "UserId" text NOT NULL,
  "RoleId" text NOT NULL,
  CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
  CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId")
      REFERENCES public."AspNetRoles" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId")
      REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetUserRoles" OWNER TO erexus;

-- Index: public."IX_AspNetUserRoles_RoleId"

-- DROP INDEX public."IX_AspNetUserRoles_RoleId";

CREATE INDEX "IX_AspNetUserRoles_RoleId"
  ON public."AspNetUserRoles"
  USING btree
  ("RoleId" COLLATE pg_catalog."default");


  -- Table: public."AspNetUserTokens"

-- DROP TABLE public."AspNetUserTokens";

CREATE TABLE public."AspNetUserTokens"
(
  "UserId" text NOT NULL,
  "LoginProvider" text NOT NULL,
  "Name" text NOT NULL,
  "Value" text,
  CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
  CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId")
      REFERENCES public."AspNetUsers" ("Id") MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."AspNetUserTokens" OWNER TO erexus;



-- Index: public."EmailIndex"

-- DROP INDEX public."EmailIndex";

CREATE INDEX "EmailIndex"
  ON public."AspNetUsers"
  USING btree
  ("NormalizedEmail" COLLATE pg_catalog."default");

-- Index: public."UserNameIndex"

-- DROP INDEX public."UserNameIndex";

CREATE UNIQUE INDEX "UserNameIndex"
  ON public."AspNetUsers"
  USING btree
  ("NormalizedUserName" COLLATE pg_catalog."default");

  -- Table: public."__EFMigrationsHistory"

-- DROP TABLE public."__EFMigrationsHistory";

CREATE TABLE public."__EFMigrationsHistory"
(
  "MigrationId" character varying(150) NOT NULL,
  "ProductVersion" character varying(32) NOT NULL,
  CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."__EFMigrationsHistory" OWNER TO erexus;

