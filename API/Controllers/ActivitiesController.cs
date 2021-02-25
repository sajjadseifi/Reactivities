using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseController
    {

        [HttpGet]
        public async Task<ActionResult<List.ActivityEnvlop>> List(int? limit, int? offset, bool isGing, bool isHost, DateTime? startDate)
        {
            return await Mediator.Send(new List.Query(limit, offset, isGing, isHost, startDate));
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> Ditales(Guid id)
        {
            return await Mediator.Send(new Ditales.Query { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Create([FromBody] Create.Command command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, [FromBody] Edit.Command command)
        {
            command.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return await Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id)
        {
            return await Mediator.Send(new Delete.Command { Id = id });
        }

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id)
        {
            return await Mediator.Send(new Attend.Command { Id = id });
        }
    }
}