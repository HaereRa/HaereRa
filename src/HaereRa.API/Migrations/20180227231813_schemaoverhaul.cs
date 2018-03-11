﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HaereRa.API.Migrations
{
    public partial class schemaoverhaul : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_People_Departments_DepartmentId",
                table: "People");

            migrationBuilder.DropTable(
                name: "DepartmentEmailAlerts");

            migrationBuilder.DropTable(
                name: "PeopleStatuses");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "ProfileSuggestions");

            migrationBuilder.DropTable(
                name: "ProfileTypeEmailAlerts");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "ProfileTypes");

            migrationBuilder.DropIndex(
                name: "IX_People_DepartmentId",
                table: "People");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "People");

            migrationBuilder.CreateTable(
                name: "ExternalPlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    PluginAssembly = table.Column<string>(nullable: true),
                    PluginAssemblyOptions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalAccountIdentifier = table.Column<string>(nullable: false),
                    ExternalPlatformId = table.Column<int>(nullable: false),
                    IsPlatformManager = table.Column<bool>(nullable: false),
                    IsSuggestionAccepted = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAccounts_ExternalPlatforms_ExternalPlatformId",
                        column: x => x.ExternalPlatformId,
                        principalTable: "ExternalPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExternalAccounts_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupMemberships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GroupId = table.Column<int>(nullable: false),
                    IsGroupManager = table.Column<bool>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMemberships_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMemberships_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAccounts_ExternalPlatformId",
                table: "ExternalAccounts",
                column: "ExternalPlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAccounts_PersonId",
                table: "ExternalAccounts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_GroupId",
                table: "GroupMemberships",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberships_PersonId",
                table: "GroupMemberships",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAccounts");

            migrationBuilder.DropTable(
                name: "GroupMemberships");

            migrationBuilder.DropTable(
                name: "ExternalPlatforms");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "People",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeopleStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DateTime = table.Column<DateTimeOffset>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    PersonId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeopleStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeopleStatuses_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false),
                    PluginAssembly = table.Column<string>(nullable: true),
                    PluginAssemblyOptions = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentEmailAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DepartmentId = table.Column<int>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentEmailAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentEmailAlerts_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonId = table.Column<int>(nullable: false),
                    ProfileAccountIdentifier = table.Column<string>(nullable: false),
                    ProfileTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Profiles_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSuggestions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsAccepted = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false),
                    ProfileAccountIdentifier = table.Column<string>(nullable: false),
                    ProfileTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSuggestions_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSuggestions_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTypeEmailAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EmailAddress = table.Column<string>(nullable: false),
                    ProfileTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTypeEmailAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileTypeEmailAlerts_ProfileTypes_ProfileTypeId",
                        column: x => x.ProfileTypeId,
                        principalTable: "ProfileTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_People_DepartmentId",
                table: "People",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentEmailAlerts_DepartmentId",
                table: "DepartmentEmailAlerts",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PeopleStatuses_PersonId",
                table: "PeopleStatuses",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_PersonId",
                table: "Profiles",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_ProfileTypeId",
                table: "Profiles",
                column: "ProfileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSuggestions_PersonId",
                table: "ProfileSuggestions",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSuggestions_ProfileTypeId",
                table: "ProfileSuggestions",
                column: "ProfileTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTypeEmailAlerts_ProfileTypeId",
                table: "ProfileTypeEmailAlerts",
                column: "ProfileTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_People_Departments_DepartmentId",
                table: "People",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
