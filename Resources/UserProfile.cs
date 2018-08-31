using System;

namespace API.Resources
{
	public class UserProfile
	{
		public string userId { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public DateTime BirthDay { get; set; }
	}
}