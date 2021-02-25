using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Application.Errors;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ProfileReader : IProfileReader
    {
        private readonly IUserAccessor _userAccessor;
        private readonly DataContext _context;
        public ProfileReader(DataContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;

        }

        public async Task<Profile> ReadProfile(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);

            if (user == null)
                throw new RestExeption(HttpStatusCode.NotFound, new { User = "Not found" });

            var currentUser = await _context.Users.SingleOrDefaultAsync(
                x => x.UserName == _userAccessor.GetCurrentUsername()
            );

            var profile = new Profile
            {
                DisplayName = user.DisplayName,
                UserName = user.UserName,
                Bio = user.Bio,
                Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                Photos = user.Photos,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Followings.Count,
                isFollowed = false
            };

            if(currentUser.Followings.Any(x=>x.TargetId==user.Id))
                profile.isFollowed=true;


            return profile;
        }
    }
}