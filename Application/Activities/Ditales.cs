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

namespace Application.Activities
{
    public class Ditales
    {
        public class Query : IRequest<ActivityDto>
        {
            public Guid Id { get; set; }
        }
        public class Handler : IRequestHandler<Query, ActivityDto>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                this._mapper = mapper;
                this._context = context;
            }
            public async Task<ActivityDto> Handle(Query request, CancellationToken cancellationToken)
            {
                // throw new Exception("Computer Says No");

                var activitie = await this._context.Activities
                .FindAsync(request.Id);

                if (activitie == null)
                    throw new RestExeption(HttpStatusCode.NotFound, new { activitiy = "Not Found" });

                var activitiesToReturn = _mapper.Map<Activity, ActivityDto>(activitie);

                return activitiesToReturn;
            }
        }
    }
}