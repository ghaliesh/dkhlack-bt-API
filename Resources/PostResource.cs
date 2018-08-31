using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Models.Entities;

namespace API.Resources
{
	public class PostResource
	{
		public int Id { get; set; }

		[Required]
		[StringLength(350)]
		[MinLength(7)]
		public string Title { get; set; }
		public PhotoResource Photo { get; set; }

		[Required]
		[MinLength(25)]
		public string Content { get; set; }
		public DateTime PublishDate { get; set; }
		public int CategoryId { get; set; }
		public CategoryResource Category { get; set; }
		public bool Published { get; set; }
	}
}