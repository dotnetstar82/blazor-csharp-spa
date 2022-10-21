using BlazorSPA.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorSPA.Server.Data
{
	public interface IBlazorSPAContext
	{
		DbSet<Order> Orders { get; set; }

		int SaveChanges();
	}

	public class BlazorSPAContext : DbContext, IBlazorSPAContext
	{
		public BlazorSPAContext(DbContextOptions<BlazorSPAContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders { get; set; }

	}
}
