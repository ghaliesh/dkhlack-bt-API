using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Repository;
using API.Models.UnitOfWork;
using API.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
	[Route("api/user")]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<AppUser> manager;
		private readonly SignInManager<AppUser> signInManager;
		private readonly IUnitOfWork unit;
		private readonly IConfiguration configuration;
		private readonly IUserRepository repository;
		private readonly IMapper mapper;

		public AccountController(UserManager<AppUser> manager, IMapper mapper, IUnitOfWork unit, IUserRepository repository, IConfiguration configuration, SignInManager<AppUser> signInManager)
		{
			this.mapper = mapper;
			this.repository = repository;
			this.signInManager = signInManager;
			this.manager = manager;
			this.configuration = configuration;
			this.unit = unit;
		}

		[HttpGet("get_users")]
		public async Task<IActionResult> GetUsers()
		{
			var result = await repository.GetUsers();
			return Ok(result);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet("dude")]
		public string GetDudes()
		{
			return "ok dude";
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromForm] UserResources user)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var newUser = mapper.Map<UserResources, AppUser>(user);
			newUser.UserName = user.Name;
			var result = await manager.CreateAsync(newUser, user.Password);
			if (!result.Succeeded)
				return BadRequest(result.Errors);
			await SetRole(newUser);
			await unit.Commit();
			await signInManager.SignInAsync(newUser, isPersistent : false);
			newUser.LastActivity = DateTime.Now;
			var token = GenerateJwtToken(newUser.Email, newUser);
			return Ok(token);
		}

		[HttpPost("login")]
		public async Task<IActionResult> LogIn(UserResources user)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var myUser = await manager.FindByEmailAsync(user.Email);
			if (myUser == null)
				return BadRequest(ModelState);
			var result = await signInManager.CheckPasswordSignInAsync(myUser, user.Password, lockoutOnFailure : false);
			if (!result.Succeeded)
				return BadRequest("The entries did'nt match any of our records");
			myUser.LastActivity = DateTime.Now;
			await signInManager.SignInAsync(myUser, false);
			var finalResult = mapper.Map<AppUser, UserResources>(myUser);
			return Ok(finalResult);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> LogOut()
		{
			await signInManager.SignOutAsync();
			return Ok("you just logged out dude.");
		}

		[HttpGet("is_admin")]
		public async Task<IActionResult> isAdmin()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = await manager.FindByIdAsync(userId);
			var inRole = await manager.IsInRoleAsync(user, "Admin");
			return Ok(inRole);
		}

		[HttpPost("promote")]
		public async Task<IActionResult> Promote()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var target = await manager.FindByIdAsync(userId);
			Console.WriteLine(target);
			if (target == null)
				return BadRequest(ModelState);
			await manager.AddToRoleAsync(target, "Admin");
			await unit.Commit();
			return Ok(target);
		}

		[HttpGet("profile")]
		public async Task<IActionResult> GetProfile()
		{
			string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return BadRequest("dude, what are u doing??");
			var user = await manager.FindByIdAsync(userId);
			if (user == null) return NotFound();
			var result = mapper.Map<AppUser, UserResources>(user);
			return Ok(result);
		}

		private async Task<object> GenerateJwtToken(string email, AppUser user)
		{
			var userRole = await manager.GetRolesAsync(user);
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, email),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
			};
			foreach (var item in userRole)
			{
				claims.Add(new Claim(ClaimTypes.Role, item));
			}

			var userEmail = user.Email;
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.Now.AddDays(Convert.ToDouble(configuration["JwtExpireDays"]));

			var token = new JwtSecurityToken(
				configuration["JwtIssuer"],
				configuration["JwtIssuer"],
				claims,
				expires : expires,
				signingCredentials : creds
			);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private async Task SetRole(AppUser user)
		{
			await manager.AddToRoleAsync(user, "Admin");
		}
	}
}