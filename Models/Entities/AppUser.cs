using System;
using Microsoft.AspNetCore.Identity;

namespace API.Models.Entities
{
	public class AppUser : IdentityUser
	{
		public DateTime BirthDay { get; set; }
		public string Name { get; set; }
		public DateTime LastActivity { get; set; }
	}
}