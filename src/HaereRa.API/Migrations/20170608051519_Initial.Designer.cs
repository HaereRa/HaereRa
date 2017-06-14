using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using HaereRa.API.DAL;
using HaereRa.API.Models;
using HaereRa.API;

namespace HaereRa.API.Migrations
{
    [DbContext(typeof(HaereRaDbContext))]
    [Migration("20170608051519_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("HaereRa.API.Models.Department", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("HaereRa.API.Models.DepartmentEmailAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DepartmentId");

                    b.Property<string>("EmailAddress")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("DepartmentEmailAlerts");
                });

            modelBuilder.Entity("HaereRa.API.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DepartmentId");

                    b.Property<string>("FullName")
                        .IsRequired();

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("KnownAs");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("HaereRa.API.Models.PersonStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DateTime");

                    b.Property<string>("Notes");

                    b.Property<int>("PersonId");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("PeopleStatuses");
                });

            modelBuilder.Entity("HaereRa.API.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("PersonId");

                    b.Property<string>("ProfileAccountIdentifier")
                        .IsRequired();

                    b.Property<int>("ProfileTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.HasIndex("ProfileTypeId");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("HaereRa.API.Models.ProfileSuggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("IsAccepted");

                    b.Property<int>("PersonId");

                    b.Property<string>("ProfileAccountIdentifier")
                        .IsRequired();

                    b.Property<int>("ProfileTypeId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.HasIndex("ProfileTypeId");

                    b.ToTable("ProfileSuggestions");
                });

            modelBuilder.Entity("HaereRa.API.Models.ProfileType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("PluginAssembly");

                    b.Property<string>("PluginAssemblyOptions");

                    b.HasKey("Id");

                    b.ToTable("ProfileTypes");
                });

            modelBuilder.Entity("HaereRa.API.Models.ProfileTypeEmailAlert", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmailAddress")
                        .IsRequired();

                    b.Property<int>("ProfileTypeId");

                    b.HasKey("Id");

                    b.HasIndex("ProfileTypeId");

                    b.ToTable("ProfileTypeEmailAlerts");
                });

            modelBuilder.Entity("HaereRa.API.Models.DepartmentEmailAlert", b =>
                {
                    b.HasOne("HaereRa.API.Models.Department", "Department")
                        .WithMany("EmailAlerts")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.Person", b =>
                {
                    b.HasOne("HaereRa.API.Models.Department", "Department")
                        .WithMany("Members")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.PersonStatus", b =>
                {
                    b.HasOne("HaereRa.API.Models.Person", "Person")
                        .WithMany("StatusHistory")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.Profile", b =>
                {
                    b.HasOne("HaereRa.API.Models.Person", "Person")
                        .WithMany("Profiles")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HaereRa.API.Models.ProfileType", "ProfileType")
                        .WithMany("Profiles")
                        .HasForeignKey("ProfileTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.ProfileSuggestion", b =>
                {
                    b.HasOne("HaereRa.API.Models.Person", "Person")
                        .WithMany("ProfileSuggestions")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("HaereRa.API.Models.ProfileType", "ProfileType")
                        .WithMany()
                        .HasForeignKey("ProfileTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("HaereRa.API.Models.ProfileTypeEmailAlert", b =>
                {
                    b.HasOne("HaereRa.API.Models.ProfileType", "ProfileType")
                        .WithMany("EmailAlerts")
                        .HasForeignKey("ProfileTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
