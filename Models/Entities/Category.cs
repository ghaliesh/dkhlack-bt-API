using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace API.Models.Entities
{
    public class Category
    {
      public int Id { get; set; }
			public string Name { get; set; }
      public string Describition { get; set; }
			public ICollection<Post> Posts { get; set; }

			public Category()
			{
				Posts = new Collection<Post>();
			}
    }
}
