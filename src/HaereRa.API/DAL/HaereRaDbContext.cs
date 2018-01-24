using System;
using HaereRa.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HaereRa.API.DAL
{
    public class HaereRaDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<ExternalAccount> ExternalAccounts { get; set; }
        public DbSet<ExternalPlatform> ExternalPlatforms { get; set; }

		public HaereRaDbContext(DbContextOptions<HaereRaDbContext> options) : base(options)
        {
			Database.EnsureCreated();
		}

	}
}
