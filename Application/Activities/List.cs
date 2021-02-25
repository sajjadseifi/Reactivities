using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities
{
    public class List
    {
        public class ActivityEnvlop
        {
            public List<ActivityDto> Activities { get; set; }
            public int ActivitiesCount { get; set; }
            public int ActivitiesLoadedCount { get; set; }
        }
        public class Query : IRequest<ActivityEnvlop>
        {
            public int? Limit { get; set; }
            public int? Offset { get; set; }
            public bool IsGing { get; }
            public bool IsHost { get; }
            public DateTime? StartDate { get; }

            public Query(int? limit, int? offset, bool isGing, bool isHost, DateTime? startDate)
            {
                Limit = limit;
                Offset = offset;
                IsGing = isGing;
                IsHost = isHost;
                StartDate = startDate ?? DateTime.Now;
            }

        }
        public class Handler : IRequestHandler<Query, ActivityEnvlop>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }
            public async Task<ActivityEnvlop> Handle(Query request, CancellationToken cancellationToken)
            {
                var queryable = _context.Activities
                .Where(x => x.Date >= request.StartDate)
                .OrderBy(x => x.Date)
                .AsQueryable();

                if (request.IsGing && request.IsHost == false)
                {
                    queryable = queryable.Where(x => x.UserActivity.Any(
                        u => u.AppUser.UserName == _userAccessor.GetCurrentUsername()
                    ));
                }
                if (request.IsHost && request.IsGing == false)
                {
                    queryable = queryable.Where(x => x.UserActivity.Any(
                        u => u.AppUser.UserName == _userAccessor.GetCurrentUsername() && u.IsHost
                    ));
                }
                var activities = await queryable
                .Skip(request.Offset ?? 0)
                .Take(request.Limit ?? 5)
                .ToListAsync();

                var mapedActivities = _mapper.Map<List<Activity>, List<ActivityDto>>(activities);
                return new ActivityEnvlop
                {
                    Activities = mapedActivities,
                    ActivitiesCount = queryable.Count(),
                    ActivitiesLoadedCount = mapedActivities.Count
                };
            }
        }

    }
}