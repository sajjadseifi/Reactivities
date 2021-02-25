using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Comments
{
    public class Create
    {
        public class Command : IRequest<CommentDto>
        {
            public Guid ActivityId { get; set; }
            public string Body { get; set; }
            public string Username { get; set; }
        }

        public class Handler : IRequestHandler<Command, CommentDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }
            public async Task<CommentDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.ActivityId);

                if(activity == null)
                {
                    throw new RestExeption(HttpStatusCode.NotFound,new {
                        Activity="Activity IS NOT Found"
                    });
                }

                var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName == request.Username);
                
                var comment = new Comment{
                    Auther=user,
                    Body=request.Body,
                    CreateAt=DateTime.Now,
                    Activity=activity
                };

                activity.Comments.Add(comment);

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                    return _mapper.Map<CommentDto>(comment);

                throw new Exception("Problem Save Changes");
            }
        }
    }
}