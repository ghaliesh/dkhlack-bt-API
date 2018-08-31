using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Models.Repository
{
	public class PostRepository : IPostRepository
	{
		private readonly APIDbContext context;
		public PostRepository(APIDbContext context)
		{
			this.context = context;
		}

		public async Task AddPost(Post post)
		{
			await context.AddAsync(post);
		}

		public async Task<List<Post>> GetProducts()
		{
			return await context.Posts.Include(p => p.Photo).Include(p => p.Category).ToListAsync();
		}

		public async Task<Post> GetPost(int? id)
		{
			var result = await context.Posts.Include(p => p.Photo).Include(p => p.Category).SingleOrDefaultAsync(i => i.Id == id);
			return result;
		}

		public void DeletePost(Post post)
		{
			context.Entry(post).State = EntityState.Deleted;
			context.Posts.Remove(post);
		}

		public async Task<List<Post>> GetPublishedPosts()
		{
			return await context.Posts.Where(p => p.published == true).ToListAsync();
		}

		public async Task<List<Post>> GetPendingPosts()
		{
			return await context.Posts.Where(p => p.published == false).ToListAsync();
		}

		public async Task<List<Post>> CatPost(int catId)
		{
			return await context.Posts.Where(p => p.CategoryId == catId).Include(p => p.Category).Include(m => m.Photo).ToListAsync();
		}

		public async Task PublishPost(int id)
		{
			var target = await this.GetPost(id);
			target.published = true;
		}

		public async Task<List<Post>> GetUserPosts(string userId)
		{
			var result = context.Posts.Where(p => p.UserId == userId);
			return await result.ToListAsync();
		}
	}
}