using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly APIDbContext context;
		public UserRepository(APIDbContext context)
		{
			this.context = context;
		}

		public async Task<List<AppUser>> GetUsers()
		{
			return await context.Users.ToListAsync();
		}
	}
}