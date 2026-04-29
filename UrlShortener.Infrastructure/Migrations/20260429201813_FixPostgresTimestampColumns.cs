using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPostgresTimestampColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE public."Users"
                ALTER COLUMN "CreatedAt"
                TYPE timestamp with time zone
                USING "CreatedAt"::timestamp with time zone;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."Urls"
                ALTER COLUMN "CreatedAt"
                TYPE timestamp with time zone
                USING "CreatedAt"::timestamp with time zone;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."Urls"
                ALTER COLUMN "ExpiresAt"
                TYPE timestamp with time zone
                USING NULLIF("ExpiresAt", '')::timestamp with time zone;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."UrlClicks"
                ALTER COLUMN "ClickedAt"
                TYPE timestamp with time zone
                USING "ClickedAt"::timestamp with time zone;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE public."Users"
                ALTER COLUMN "CreatedAt"
                TYPE text
                USING "CreatedAt"::text;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."Urls"
                ALTER COLUMN "CreatedAt"
                TYPE text
                USING "CreatedAt"::text;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."Urls"
                ALTER COLUMN "ExpiresAt"
                TYPE text
                USING "ExpiresAt"::text;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE public."UrlClicks"
                ALTER COLUMN "ClickedAt"
                TYPE text
                USING "ClickedAt"::text;
                """);
        }
    }
}
