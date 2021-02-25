using System.Linq;
using AutoMapper;
using Domain;

namespace Application.Activities
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity,ActivityDto>();
            CreateMap<UserActivity,AttendeeDto>()
            .ForMember(d=>d.UserName,o=>o.MapFrom(s=>s.AppUser.UserName))
            .ForMember(d=>d.DisplayName,o=>o.MapFrom(s=>s.AppUser.DisplayName))
            .ForMember(d=>d.Image,o=>o.MapFrom(s=>s.AppUser.Photos.FirstOrDefault(p=>p.IsMain).Url))
            .ForMember(d=>d.Following,o=>o.ResolveUsing<FollowingResolver>());
            // .ForMember(d=>d.Following,o=>o.MapFrom(s=>s.AppUser.Followers.Any(x => x.TargetId == s.AppUserId)));
            
        }
    }
    
}