using System.Threading.Tasks;
using API.Models.Entities;

namespace API.Models.UnitOfWork
{
	public class UniteOfWork : IUnitOfWork
	{
		private readonly APIDbContext context;
		public UniteOfWork(APIDbContext context)
		{
			this.context = context;

		}
		public async Task Commit()
		{
			await context.SaveChangesAsync();
		}
	}
}