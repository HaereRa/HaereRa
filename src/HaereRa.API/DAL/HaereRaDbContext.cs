using System;
using Microsoft.EntityFrameworkCore;
using HaereRa.API.Models;
namespace HaereRa.API.DAL
{
    public class HaereRaDbContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentEmailAlert> DepartmentEmailAlerts { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<PersonStatus> PeopleStatuses { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileType> ProfileTypes { get; set; }
        public DbSet<ProfileTypeEmailAlert> ProfileTypeEmailAlerts { get; set; }
        public DbSet<ProfileSuggestion> ProfileSuggestions { get; set; }

		public HaereRaDbContext(DbContextOptions<HaereRaDbContext> options) : base(options)
        {
			Database.EnsureCreated();
		}

	}
}
