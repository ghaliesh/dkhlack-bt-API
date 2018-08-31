using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.Repository
{
	public interface ICategoryRepository
	{
		Task AddCat(Category category);
		Task<List<Category>> GetCategories();
		Task<Category> GetCat(int id);
		void DeleteCat(Category category);
		Category UpdateCat(Category category);
	}
}