using System;
using System.Threading.Tasks;
using API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API.Configuration
{
	public static class Seed
	{
		public static async Task CreateRole(IServiceProvider serviceProvider, IConfiguration configuration)
		{
			var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var UserManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

			string[] roleNames = { "Admin", "Author" };
			foreach (var role in roleNames)
			{
				bool roleExist = await RoleManager.RoleExistsAsync(role);
				if (!roleExist)
					await RoleManager.CreateAsync(new IdentityRole(role));
			}

			var target = await UserManager.FindByEmailAsync(configuration["God:GodEmail"]);
			if (target == null)
			{
				var god = new AppUser
				{
					Email = configuration["God:GodEmail"]
				};
				var password = configuration["God:GodPassword"];
				var result = await UserManager.CreateAsync(god, password);
				if (result.Succeeded)
					await UserManager.AddToRoleAsync(god, "Admin");
			}
			else
			{
				await UserManager.AddToRoleAsync(target, "Admin");
			}
		}
	}
}