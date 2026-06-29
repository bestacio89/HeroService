using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeroService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<string>(type: "text", nullable: true),
                    VersionName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroLore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BackgroundStory = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroLore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    HealthMultiplier = table.Column<float>(type: "real", nullable: true),
                    ManaMultiplier = table.Column<float>(type: "real", nullable: true),
                    AttackDamageMultiplier = table.Column<float>(type: "real", nullable: true),
                    AbilityPowerMultiplier = table.Column<float>(type: "real", nullable: true),
                    AttackSpeedMultiplier = table.Column<float>(type: "real", nullable: true),
                    CastSpeedMultiplier = table.Column<float>(type: "real", nullable: true),
                    CritChanceMultiplier = table.Column<float>(type: "real", nullable: true),
                    CritDamageMultiplier = table.Column<float>(type: "real", nullable: true),
                    ArmorMultiplier = table.Column<float>(type: "real", nullable: true),
                    MagicResistanceMultiplier = table.Column<float>(type: "real", nullable: true),
                    DamageReductionMultiplier = table.Column<float>(type: "real", nullable: true),
                    ShieldStrengthMultiplier = table.Column<float>(type: "real", nullable: true),
                    MovementSpeedMultiplier = table.Column<float>(type: "real", nullable: true),
                    AttackRangeMultiplier = table.Column<float>(type: "real", nullable: true),
                    CooldownReductionMultiplier = table.Column<float>(type: "real", nullable: true),
                    ResourceRegenerationMultiplier = table.Column<float>(type: "real", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroProgressionModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    HealthPerLevel = table.Column<float>(type: "real", nullable: false),
                    ManaPerLevel = table.Column<float>(type: "real", nullable: false),
                    AttackDamagePerLevel = table.Column<float>(type: "real", nullable: false),
                    AbilityPowerPerLevel = table.Column<float>(type: "real", nullable: false),
                    ArmorPerLevel = table.Column<float>(type: "real", nullable: false),
                    MagicResistancePerLevel = table.Column<float>(type: "real", nullable: false),
                    AttackSpeedPerLevel = table.Column<float>(type: "real", nullable: false),
                    CastSpeedPerLevel = table.Column<float>(type: "real", nullable: false),
                    ResourceRegenerationPerLevel = table.Column<float>(type: "real", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroProgressionModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MythologyTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MythologyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OriginArchetypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginArchetypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OriginCultures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginCultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillLore",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    VisualExplanation = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillLore", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    CooldownMultiplier = table.Column<float>(type: "real", nullable: false),
                    ManaCostMultiplier = table.Column<float>(type: "real", nullable: false),
                    DamageMultiplier = table.Column<float>(type: "real", nullable: false),
                    HealingMultiplier = table.Column<float>(type: "real", nullable: false),
                    ShieldMultiplier = table.Column<float>(type: "real", nullable: false),
                    CastTimeMultiplier = table.Column<float>(type: "real", nullable: false),
                    ChannelDurationMultiplier = table.Column<float>(type: "real", nullable: false),
                    CrowdControlDurationMultiplier = table.Column<float>(type: "real", nullable: false),
                    RangeMultiplier = table.Column<float>(type: "real", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    SkillType = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillScalingModifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttackDamageRatio = table.Column<float>(type: "real", nullable: false),
                    AbilityPowerRatio = table.Column<float>(type: "real", nullable: false),
                    MaxHealthRatio = table.Column<float>(type: "real", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillScalingModifiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    VisualTheme = table.Column<string>(type: "text", nullable: true),
                    VfxBundleKey = table.Column<string>(type: "text", nullable: true),
                    SfxBundleKey = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    HeroClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    Affiliation_ArchetypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Affiliation_MythologyId = table.Column<Guid>(type: "uuid", nullable: true),
                    Affiliation_OriginCultureId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillKit_PassiveSkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillKit_PrimarySkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillKit_SecondarySkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillKit_TertiarySkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    SkillKit_UltimateSkillId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Heroes_HeroClasses_HeroClassId",
                        column: x => x.HeroClassId,
                        principalTable: "HeroClasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heroes_MythologyTypes_Affiliation_MythologyId",
                        column: x => x.Affiliation_MythologyId,
                        principalTable: "MythologyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heroes_OriginArchetypes_Affiliation_ArchetypeId",
                        column: x => x.Affiliation_ArchetypeId,
                        principalTable: "OriginArchetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Heroes_OriginCultures_Affiliation_OriginCultureId",
                        column: x => x.Affiliation_OriginCultureId,
                        principalTable: "OriginCultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillBaseStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseCooldown = table.Column<float>(type: "real", nullable: true),
                    BaseManaCost = table.Column<float>(type: "real", nullable: true),
                    BaseDamage = table.Column<float>(type: "real", nullable: true),
                    BaseHealing = table.Column<float>(type: "real", nullable: true),
                    BaseShieldValue = table.Column<float>(type: "real", nullable: true),
                    BaseCastTime = table.Column<float>(type: "real", nullable: true),
                    BaseChannelDuration = table.Column<float>(type: "real", nullable: true),
                    BaseCrowdControlDuration = table.Column<float>(type: "real", nullable: true),
                    BaseRange = table.Column<float>(type: "real", nullable: true),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillBaseStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillBaseStats_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillEffects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectType = table.Column<int>(type: "integer", nullable: false),
                    Magnitude = table.Column<float>(type: "real", nullable: false),
                    Duration = table.Column<float>(type: "real", nullable: false),
                    Radius = table.Column<float>(type: "real", nullable: false),
                    AttackDamageRatio = table.Column<float>(type: "real", nullable: true),
                    AbilityPowerRatio = table.Column<float>(type: "real", nullable: true),
                    MaxHealthRatio = table.Column<float>(type: "real", nullable: true),
                    IsPeriodic = table.Column<bool>(type: "boolean", nullable: false),
                    IsInstant = table.Column<bool>(type: "boolean", nullable: false),
                    IsChannelled = table.Column<bool>(type: "boolean", nullable: false),
                    TargetType = table.Column<int>(type: "integer", nullable: false),
                    StackType = table.Column<int>(type: "integer", nullable: false),
                    MaxStacks = table.Column<int>(type: "integer", nullable: false),
                    Revision = table.Column<int>(type: "integer", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillEffects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillEffects_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeroBaseStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeroId = table.Column<Guid>(type: "uuid", nullable: false),
                    BaseHealth = table.Column<float>(type: "real", nullable: false),
                    BaseMana = table.Column<float>(type: "real", nullable: false),
                    BaseAttackDamage = table.Column<float>(type: "real", nullable: false),
                    BaseAbilityPower = table.Column<float>(type: "real", nullable: false),
                    BaseAttackSpeed = table.Column<float>(type: "real", nullable: false),
                    BaseCastSpeed = table.Column<float>(type: "real", nullable: false),
                    BaseCritChance = table.Column<float>(type: "real", nullable: false),
                    BaseCritDamageMultiplier = table.Column<float>(type: "real", nullable: false),
                    BaseArmor = table.Column<float>(type: "real", nullable: false),
                    BaseMagicResistance = table.Column<float>(type: "real", nullable: false),
                    BaseDamageReduction = table.Column<float>(type: "real", nullable: false),
                    BaseShieldStrengthMultiplier = table.Column<float>(type: "real", nullable: false),
                    BaseMovementSpeed = table.Column<float>(type: "real", nullable: false),
                    BaseAttackRange = table.Column<float>(type: "real", nullable: false),
                    BaseCooldownReduction = table.Column<float>(type: "real", nullable: false),
                    BaseResourceRegeneration = table.Column<float>(type: "real", nullable: false),
                    DateCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DateDeleted = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroBaseStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroBaseStats_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroBaseStats_HeroId",
                table: "HeroBaseStats",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Affiliation_ArchetypeId",
                table: "Heroes",
                column: "Affiliation_ArchetypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Affiliation_MythologyId",
                table: "Heroes",
                column: "Affiliation_MythologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Affiliation_OriginCultureId",
                table: "Heroes",
                column: "Affiliation_OriginCultureId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_HeroClassId",
                table: "Heroes",
                column: "HeroClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Heroes_Name",
                table: "Heroes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillBaseStats_SkillId",
                table: "SkillBaseStats",
                column: "SkillId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillEffects_SkillId",
                table: "SkillEffects",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameVersions");

            migrationBuilder.DropTable(
                name: "HeroBaseStats");

            migrationBuilder.DropTable(
                name: "HeroLore");

            migrationBuilder.DropTable(
                name: "HeroModifiers");

            migrationBuilder.DropTable(
                name: "HeroProgressionModifiers");

            migrationBuilder.DropTable(
                name: "SkillBaseStats");

            migrationBuilder.DropTable(
                name: "SkillEffects");

            migrationBuilder.DropTable(
                name: "SkillLore");

            migrationBuilder.DropTable(
                name: "SkillModifiers");

            migrationBuilder.DropTable(
                name: "SkillScalingModifiers");

            migrationBuilder.DropTable(
                name: "Skins");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "HeroClasses");

            migrationBuilder.DropTable(
                name: "MythologyTypes");

            migrationBuilder.DropTable(
                name: "OriginArchetypes");

            migrationBuilder.DropTable(
                name: "OriginCultures");
        }
    }
}
