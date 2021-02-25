using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class Edit
    {
        public class Command : IRequest
        {
            public string DisplayName { get; set; }
            public string Bio { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }
            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.SingleOrDefaultAsync(u=> u.UserName == _userAccessor.GetCurrentUsername());
                
                Console.WriteLine("request : " +request.Bio+" - "+request.DisplayName);
                Console.WriteLine("user : " +user.Bio+" - "+user.DisplayName);

                user.DisplayName = request.DisplayName ?? user.DisplayName;
                user.Bio = request.Bio ?? user.Bio;

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                    return Unit.Value;

                throw new Exception("Problem Save Changes");
            }
        }
    }
}