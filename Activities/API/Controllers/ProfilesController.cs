﻿using Application.Profiles;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProfilesController : BaseApiController
{
    [HttpGet("{userName}")]
    public async Task<IActionResult> GetProfile(string userName)
    {
        return HandleResult(await Mediator.Send(new Details.Query { UserName = userName }));
    }

    [HttpGet("{userName}/activities")]
    public async Task<ActionResult> GetActivities(string userName, [FromQuery] string predicate)
    {
        return HandleResult(
            await Mediator.Send(new ListActivities.Query { Username = userName, Predicate = predicate }));
    }
}