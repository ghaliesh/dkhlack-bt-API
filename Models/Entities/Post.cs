using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace API.Models.Entities
{
	public class Post
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public Photo Photo { get; set; }
		public int Views { get; set; }
		public string Content { get; set; }
		public string ContentHtml { get; set; }
		public DateTime PublishDate { get; set; }
		public bool published { get; set; }
		public Category Category { get; set; }
		public int CategoryId { get; set; }
		public AppUser User { get; set; }
		public string UserId { get; set; }
	}
}