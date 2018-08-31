using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API.Configuration;
using API.Models.Entities;
using API.Models.Repository;
using API.Models.UnitOfWork;
using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API
{

	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.WithOrigins("http://localhost:4200")
					.AllowAnyMethod()
					.AllowAnyHeader());
			});
			services.AddAutoMapper();
			services.AddScoped<IPostRepository, PostRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();
			services.AddScoped<IUnitOfWork, UniteOfWork>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddIdentity<AppUser, IdentityRole>()
				.AddEntityFrameworkStores<APIDbContext>();
			services.AddAuthorization(options =>
			{
				options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
				options.AddPolicy("RequireAuthorRole", policy => policy.RequireRole("Author"));
			});
			services.AddDbContext<APIDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
			services.AddAuthentication(options =>
				{

					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(cfg =>
				{
					cfg.RequireHttpsMetadata = false;
					cfg.SaveToken = true;
					cfg.TokenValidationParameters = new TokenValidationParameters
					{
						ValidIssuer = Configuration["JwtIssuer"],
							ValidAudience = Configuration["JwtIssuer"],
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
							ClockSkew = TimeSpan.Zero // remove delay of token when expire
					};
				});

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
		{
			Seed.CreateRole(serviceProvider, Configuration).Wait();
			app.UseAuthentication();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.Use(async(context, next) =>
			{
				await next();
				if (context.Response.StatusCode == 404 &&
					!Path.HasExtension(context.Request.Path.Value) &&
					!context.Request.Path.Value.StartsWith("/api/"))
				{
					context.Request.Path = "/index.html";
					await next();
				}
			});
			app.UseMvc();
			app.UseDefaultFiles();
			app.UseStaticFiles();
		}
	}
}