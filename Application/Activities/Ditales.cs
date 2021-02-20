using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Ditales
    {
        public class Query : IRequest<Activity>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, Activity>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                this._context = context;
            }
            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                // throw new Exception("Computer Says No");

                var activitie = await this._context.Activities.FindAsync(request.Id);
                
                if(activitie == null)
                    throw new RestExeption(HttpStatusCode.NotFound,new {activitiy="Not Found"});

                return activitie;
            }
        }
    }
}