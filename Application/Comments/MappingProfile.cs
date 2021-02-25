using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Comments
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Comment,CommentDto>()
            .ForMember(d=>d.UserName,o=>o.MapFrom(s=>s.Auther.UserName))
            .ForMember(d=>d.DisplayName,o=>o.MapFrom(s=>s.Auther.DisplayName))
            .ForMember(d=>d.Image,o=>o.MapFrom(s=>s.Auther.Photos.FirstOrDefault(x=>x.IsMain).Url));
        }
    }
}