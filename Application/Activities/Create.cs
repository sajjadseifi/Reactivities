using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class Create
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public string City { get; set; }
            public string Venue { get; set; }
        }
        public class CommandValidation : AbstractValidator<Command>
        {
            public CommandValidation()
            {
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Category).NotEmpty();
                RuleFor(x => x.Date).NotEmpty();
                RuleFor(x => x.Description).NotEmpty();
                RuleFor(x => x.Venue).NotEmpty();
                RuleFor(x => x.City).NotEmpty();
            }
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
                var activitie = new Activity
                {
                    Id = request.Id,
                    Title = request.Title,
                    Category = request.Category,
                    City = request.City,
                    Description = request.Description,
                    Date = request.Date,
                    Venue = request.Venue,
                };

                _context.Activities.Add(activitie);

                var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName == _userAccessor.GetCurrentUsername());

                var attendee =new UserActivity{
                    Activity=activitie,
                    DateJoined=DateTime.Now,
                    IsHost=true,
                    AppUser=user
                };
                _context.UserActivities.Add(attendee);

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                    return Unit.Value;


                throw new Exception("Problem Save Changes");
            }
        }
    }
}