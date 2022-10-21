using BlazorSPA.Server.Data;
using BlazorSPA.Server.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace BlazorSPA.Server
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{

			services.AddDbContext<BlazorSPAContext>(options =>
			   options.UseSqlite("Data Source=../BlazorSPA.db")
			);
			services.AddScoped<IBlazorSPAContext>(provider => provider.GetService<BlazorSPAContext>());

			services.AddScoped<IOrderRepository, OrderRepository>();

			services.AddControllersWithViews();
			services.AddRazorPages();

			services.AddSwaggerGen();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BlazorSPAContext db)
		{
			if (env.IsDevelopment())
			{
				db.Database.EnsureCreated();

				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();

				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor SPA");
				});
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToFile("index.html");
			});
		}

	}
}
