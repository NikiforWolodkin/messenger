using Messenger.Exceptions;
using Messenger.Utilities;
using MessengerModels.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Messenger.Services;

public static class EventService
{
    public static async Task<ICollection<CalendarEventResponse>> GetUserEventsForDayAsync(DateTime day)
    {
        var result = await Api.PostAsync<ICollection<CalendarEventResponse>, CalendarEventsForUserByDayRequest>("events/by-day", new() { Day = day });

        if (!result.IsSuccessful)
        {
            throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
        }

        return result.Response;
    }

    public static async Task<CalendarEventResponse> SetAttendanceAsync(Guid eventId, bool isAttending)
    {
        var result = await Api.PutAsync<CalendarEventResponse, CalendarEventUserAttendanceSetRequest>($"events/{eventId}", new() { IsAttending = isAttending });

        if (!result.IsSuccessful)
        {
            throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
        }

        return result.Response;
    }

    public static async Task DeleteAsync(Guid eventId)
    {
        var result = await Api.DeleteAsync($"events/{eventId}");

        if (!result.IsSuccessful)
        {
            throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
        }
    }

    public static async Task<CalendarEventResponse> AddAsync(string name, 
                                                             string agenda, 
                                                             DateTime startTime, 
                                                             int durationInMinutes, 
                                                             ICollection<Guid> participantIds)
    {
        var request = new CalendarEventAddRequest
        {
            Name = name,
            Agenda = agenda,
            StartTime = startTime,
            DurationInMinutes = durationInMinutes,
            ParticipantIds = participantIds
        };

        var result = await Api.PostAsync<CalendarEventResponse, CalendarEventAddRequest>("events", request);

        if (!result.IsSuccessful)
        {
            throw new HttpException(result.Error, (HttpStatusCode)result.StatusCode);
        }

        return result.Response;
    }
}
