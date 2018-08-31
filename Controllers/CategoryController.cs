using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Repository;
using API.Models.UnitOfWork;
using API.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// Refactored

namespace API.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/categories")]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryRepository repository;
		private readonly IMapper mapper;
		private readonly IUnitOfWork unit;

		public CategoryController(IPostRepository Postrepo, ICategoryRepository repository, IMapper mapper, IUnitOfWork unit)
		{
			this.repository = repository;
			this.unit = unit;
			this.mapper = mapper;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<List<CategoryResource>> GetCategories()
		{
			var result = await repository.GetCategories();
			return mapper.Map<List<Category>, List<CategoryResource>>(result);
		}

		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<IActionResult> GetCategory(int id)
		{
			var target = await repository.GetCat(id);
			if (target == null) return NotFound();
			return Ok(target);
		}

		[HttpPost("create")]
		public async Task<IActionResult> AddCategory([FromForm] CategoryResource category)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var created = mapper.Map<CategoryResource, Category>(category);
			await repository.AddCat(created);
			await unit.Commit();
			var result = mapper.Map<Category, CategoryResource>(created);
			return Ok(result);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryResource category)
		{
			var target = await repository.GetCat(id);
			if (target == null) return NotFound();
			mapper.Map<CategoryResource, Category>(category, target);
			repository.UpdateCat(target);
			return Ok(target);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var target = await repository.GetCat(id);
			if (target == null) return NotFound();
			repository.DeleteCat(target);
			await unit.Commit();
			return Ok("Category has been deleted");
		}
	}
}