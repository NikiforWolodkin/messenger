﻿using MessengerApi.Bots;
using MessengerApi.Helpers;
using MessengerApiDomain.Models;
using MessengerApiServices.Exceptions;
using MessengerApiServices.Interfaces;
using MessengerModels.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessengerApi.Controllers;

[Authorize]
[Route("api/events")]
[ApiController]
public class CalendarEventController : ControllerBase
{
    private readonly EventBot _eventBot;
    private readonly ICalendarEventService _eventService;

    public CalendarEventController(ICalendarEventService eventService, EventBot eventBot)
    {
        _eventService = eventService;
        _eventBot = eventBot;
    }

    [HttpPost("by-day")]
    public async Task<IActionResult> GetAsync(CalendarEventsForUserByDayRequest request)
    {
        var id = JwtClaimsHelper.GetId(User.Identity);

        return Ok(await _eventService.GetUserEventsForDayAsync(id, request.Day));
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(CalendarEventAddRequest request)
    {
        var id = JwtClaimsHelper.GetId(User.Identity);

        var calendarEvent = await _eventService.AddAsync(id, request);

        await _eventBot.NotifyUsersOfNewEventAsync(calendarEvent);

        return Created($"api/events/{calendarEvent.Id}", calendarEvent);
    }

    [HttpDelete("{eventId:Guid}")]
    public async Task<IActionResult> RemoveAsync(Guid eventId)
    {
        var id = JwtClaimsHelper.GetId(User.Identity);

        try
        {
            await _eventService.RemoveAsync(eventId, id);
        }
        catch (UnauthorizedException ex)
        {
            return new ObjectResult(new ErrorResponse(ex.Message)) { StatusCode = 403 };
        }

        return Ok();
    }

    [HttpPut("{eventId:Guid}")]
    public async Task<IActionResult> SetAttendanceAsync(Guid eventId, CalendarEventUserAttendanceSetRequest request)
    {
        var id = JwtClaimsHelper.GetId(User.Identity);

        var calendarEvent = await _eventService.SetUserAttendanceAsync(eventId, id, request.IsAttending);

        return Ok(calendarEvent);
    }
}
