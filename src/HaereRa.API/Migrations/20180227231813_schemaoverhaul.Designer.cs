using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HaereRa.API.DAL;
using HaereRa.API.Models;

namespace HaereRa.API.Migrations
{
    [DbContext(typeof(HaereRaDbContext))]
    [Migration("20180227231813_schemaoverhaul")]
    partial class schemaoverhaul
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("HaereRa.API.Models.ExternalAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExternalAccountIdentifier")
                        .IsRequired();

                    b.Property<int>("ExternalPlatformId");

                    b.Property<bool>("IsPlatformManager");

                    b.Property<int>("IsSuggestionAccepted");

                    b.Property<int>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("ExternalPlatformId");

                    b.HasIndex("PersonId");

                    b.ToTable("ExternalAccounts");
                });

            modelBuilder.Entity("HaereRa.API.Models.ExternalPlatform", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PluginAssembly");

                    b.Property<string>("PluginAssemblyOptions");

                    b.HasKey("Id");

                    b.ToTable("ExternalPlatforms");
                });

            modelBuilder.Entity("HaereRa.API.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("HaereRa.API.Models.GroupMembership", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GroupId");

                    b.Property<bool>("IsGroupManager");

                    b.Property<int>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PersonId");

                    b.ToTable("GroupMemberships");
                });

            modelBuilder.Entity("HaereRa.API.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("KnownAs");

                    b.HasKey("Id");

                    b.ToTable("People");
                });

            modelBuilder.Entity("HaereRa.API.Models.ExternalAccount", b =>
                {
                    b.HasOne("HaereRa.API.Models.ExternalPlatform", "ExternalPlatform")
                        .WithMany("ExternalAccounts")
                        .HasForeignKey("ExternalPlatformId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HaereRa.API.Models.Person", "Person")
                        .WithMany("ExternalAccounts")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.GroupMembership", b =>
                {
                    b.HasOne("HaereRa.API.Models.Group", "Group")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HaereRa.API.Models.Person", "Person")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
