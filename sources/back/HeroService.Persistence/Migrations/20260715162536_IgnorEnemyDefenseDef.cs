using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IgnorEnemyDefenseDef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "IgnoreEnemyDefenseAdjustment",
                table: "HeroModifiers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "BaseIgnoreEnemyDefense",
                table: "HeroBaseStats",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IgnoreEnemyDefenseAdjustment",
                table: "HeroModifiers");

            migrationBuilder.DropColumn(
                name: "BaseIgnoreEnemyDefense",
                table: "HeroBaseStats");
        }
    }
}
