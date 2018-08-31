using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repository
{
	public interface IUserRepository
	{
		Task<List<AppUser>> GetUsers();
	}
}