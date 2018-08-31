using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Resources;

namespace API.Models.Repository
{
	public interface IPostRepository
	{
		Task<List<Post>> CatPost(int catId);
		Task AddPost(Post post);
		Task<List<Post>> GetPublishedPosts();
		Task<List<Post>> GetPendingPosts();
		Task<Post> GetPost(int? id);
		Task<List<Post>> GetUserPosts(string userId);
		void DeletePost(Post post);
		Task<List<Post>> GetProducts();
		Task PublishPost(int id);

	}
}