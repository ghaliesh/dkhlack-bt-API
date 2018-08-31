using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace API.Models.Entities
{
	public class APIDbContext : IdentityDbContext<AppUser>
	{
		public DbSet<Post> Posts { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Photo> Photos { get; set; }

		public APIDbContext(DbContextOptions options) : base(options) { }
	}
}