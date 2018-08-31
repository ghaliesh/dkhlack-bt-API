using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository
{
	public class CategoryRepository : ICategoryRepository
	{
		private readonly APIDbContext context;
		public CategoryRepository(APIDbContext context)
		{
			this.context = context;
		}

		public async Task<List<Category>> GetCategories()
		{
			return await context.Categories.ToListAsync();
		}

		public async Task<Category> GetCat(int id)
		{
			var target = await context.Categories.FindAsync(id);
			return target;
		}

		public async Task AddCat(Category category)
		{
			await context.Categories.AddAsync(category);
		}

		public void DeleteCat(Category category)
		{
			context.Remove(category);
		}

		public Category UpdateCat(Category category)
		{
			context.Categories.Update(category);
			return category;
		}
	}
}