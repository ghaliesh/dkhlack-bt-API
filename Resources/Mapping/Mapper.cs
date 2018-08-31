using System.Linq;
using API.Models.Entities;
using AutoMapper;

namespace API.Resources.Mapping
{
	public class Mapper : Profile
	{
		public Mapper()
		{
			// API to Domain models
			CreateMap<PostResource, Post>()
				.ForMember(pr => pr.Id, p => p.Ignore())
				.ForMember(pr => pr.Photo, m => m.MapFrom(p => p.Photo));

			CreateMap<Category, CategoryResource>();

			CreateMap<AppUser, UserResources>();

			// Domain models to API
			CreateMap<Post, PostResource>()
				.ForMember(pr => pr.CategoryId, g => g.MapFrom(p => p.CategoryId))
				.ForMember(pr => pr.Photo, m => m.MapFrom(p => p.Photo));

			CreateMap<CategoryResource, Category>()
				.ForMember(c => c.Id, r => r.Ignore());

			CreateMap<UserResources, AppUser>()
				.ForMember(au => au.Id, r => r.Ignore());
		}
	}
}