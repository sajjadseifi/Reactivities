using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Edit
    {
                public class Command : IRequest{
                        public Guid Id { get; set; }
                        public string Title { get; set; }
                        public string Description { get; set; }
                        public string Category { get; set; }
                        public DateTime? Date { get; set; }
                        public string City { get; set; }
                        public string Venue { get; set; }
                }
        
                public class Handler : IRequestHandler<Command>
                {
                    private readonly DataContext _context;
                    public Handler(DataContext context)
                    {
                        this._context = context;
                    }
                    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
                    {
                        var activitie = await _context.Activities.FindAsync(request.Id);

                        if(activitie == null)
                            throw new Exception("Coud Not Found Activity");

                        activitie.Title = request.Title ?? activitie.Title;
                        activitie.Description = request.Description ?? activitie.Description;
                        activitie.City = request.City ?? activitie.City;
                        activitie.Category = request.Category ?? activitie.Category;
                        activitie.Venue = request.Venue ?? activitie.Venue;
                        activitie.Date = request.Date ?? activitie.Date;

                        var success = await _context.SaveChangesAsync() > 0;
                       
                       if(success)
                            return Unit.Value;
            
                        throw new Exception("Problem Save Changes");
                    }
                }
    }
}