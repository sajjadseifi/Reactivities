using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Ditales
    {
        public class Query : IRequest<Profile>
        {
            public string Username{get;set;}
        }

        public class Handler : IRequestHandler<Query, Profile>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Profile> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u=> u.UserName == request.Username);   

                return new Profile{
                    DisplayName=user.DisplayName,
                    UserName= user.UserName,    
                    Bio= user.Bio,
                    Image=user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                    Photos = user.Photos
                };  
            }
        }

    }
}