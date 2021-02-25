using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var username = GetUsername();
            command.Username = username;

            var comment = await _mediator.Send(command);

            await Clients.Group(command.ActivityId.ToString()).SendAsync("ReciveComment", comment);
        }
        private string GetUsername()
        {
            return Context.User?.Claims?
            .FirstOrDefault(
                 x => x.Type == ClaimTypes.NameIdentifier
            )?.Value;
        }
        public async Task AddToGroup(string gorupName){
            await Groups.AddToGroupAsync(Context.ConnectionId,gorupName);

            var username = GetUsername();

            await Clients.Group(gorupName).SendAsync("Send",$"{username} has join the group");
        }
        public async Task RemoveToGroup(string gorupName){
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,gorupName);

            var username = GetUsername();

            await Clients.Group(gorupName).SendAsync("Send",$"{username} has left the group");
        }
    }
}