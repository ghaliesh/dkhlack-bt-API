using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models.Entities;
using API.Models.Repository;
using API.Models.UnitOfWork;
using API.Resources;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
// Refactored

namespace API.Controllers
{
	[Authorize(Roles = "Admin")]
	[Route("api/posts")]
	public class PostController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly IPostRepository repository;
		private readonly IUnitOfWork unit;
		private readonly IHostingEnvironment host;
		public PostController(IMapper mapper, IPostRepository repository, IUnitOfWork unit, IHostingEnvironment host)
		{
			this.host = host;
			this.unit = unit;
			this.repository = repository;
			this.mapper = mapper;
		}

		[AllowAnonymous]
		[HttpGet("published")]
		public async Task<List<PostResource>> GetPublishedPosts()
		{
			var result = await repository.GetPublishedPosts();
			return mapper.Map<List<Post>, List<PostResource>>(result);
		}

		[HttpGet("pending")]
		public async Task<List<PostResource>> GetPendingPosts()
		{
			var posts = await repository.GetPendingPosts();
			return mapper.Map<List<Post>, List<PostResource>>(posts);
		}

		[AllowAnonymous]
		[HttpGet("post_by_user")]
		public async Task<IActionResult> GetPostByUser()
		{
			string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (String.IsNullOrEmpty(userId)) return NotFound();
			var posts = await repository.GetUserPosts(userId);
			var result = mapper.Map<List<Post>, List<PostResource>>(posts);
			return Ok(result);
		}

		[AllowAnonymous]
		[HttpGet("cat/{id}")]
		public async Task<List<PostResource>> GetPostPerCat(int id)
		{
			var posts = await repository.CatPost(id);
			var result = mapper.Map<List<Post>, List<PostResource>>(posts);
			return result;
		}

		[HttpPut("publish/{id}")]
		public async Task<IActionResult> PublishPost(int id)
		{
			await repository.PublishPost(id);
			await unit.Commit();
			return Ok();
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> DeletePost(int id)
		{
			var target = await repository.GetPost(id);
			if (target == null) return NotFound();
			var result = mapper.Map<Post, PostResource>(target);
			repository.DeletePost(target);
			await unit.Commit();
			return Ok(result);
		}

		[HttpPost("edit/{id?}")]
		public async Task<IActionResult> AddPost(int? id, PostResource post, [FromForm] IFormFile file)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null) return Unauthorized();
			if (!ModelState.IsValid) return BadRequest(ModelState);
			var target = await repository.GetPost(id);
			if (target != null)
			{
				target.Title = post.Title;
				target.UserId = userId;
				target.Content = post.Content;
				target.CategoryId = post.CategoryId;
				target.PublishDate = DateTime.Now;
				target.published = false;
				await unit.Commit();
				await AddPhoto(file, target);
				return Ok(mapper.Map<Post, PostResource>(target));
			}
			else
			{
				var createPost = mapper.Map<PostResource, Post>(post);
				createPost.UserId = userId;
				createPost.PublishDate = DateTime.Now;
				createPost.published = false;
				await repository.AddPost(createPost);
				await unit.Commit();
				if (file != null)
					await AddPhoto(file, createPost);
				var result = mapper.Map<Post, PostResource>(createPost);
				return Ok(result);
			}
		}

		private async Task AddPhoto(IFormFile file, Post post)
		{
			var uploadedFils = Path.Combine(host.WebRootPath, "Uploads");
			if (!Directory.Exists(uploadedFils))
				Directory.CreateDirectory(uploadedFils);
			var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			var filePath = Path.Combine(uploadedFils, filename);
			using(var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			var photo = new Photo() { ImageUrl = filename };
			post.Photo = photo;
			await unit.Commit();
		}
	}
}