using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using GoogleCalenderApi.EventUtils;
using Microsoft.IdentityModel.Tokens;

public class GoogleCalendarService : IGoogleCalendarService
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleCalendarService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _configuration = configuration;
    }

    public string GetAuthCode()
    {
        try
        {
            string redirect_uri_encode = Method.urlEncodeForGoogle(_configuration["redirectURL"]);
            var mainURL = string.Format(_configuration["scopeURL1"], redirect_uri_encode, _configuration["prompt"], _configuration["response_type"], _configuration["clientId"], _configuration["scope"], _configuration["access_type"]);
            return mainURL;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<GoogleTokenResponse> GetTokens(string code)
    {
        var content = new StringContent($"code={code}&redirect_uri={Uri.EscapeDataString(_configuration["redirectURL"])}&client_id={_configuration["clientId"]}&client_secret={_configuration["clientSecret"]}&grant_type=authorization_code", Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await _httpClient.PostAsync(_configuration["tokenEndpoint"], content);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleTokenResponse>(responseContent);
            return tokenResponse;
        }
        else
        {
            throw new Exception($"Failed to authenticate: {responseContent}");
        }
    }

    public List<GoogleCalendarReqDTO> GetAll(EventSearchParams eventParams)
    {
        var service = GetCalenderService(eventParams.token);
        var CalenderItems = new List<GoogleCalendarReqDTO>();
        var req = service.Events.List(_configuration["calendarId"]);

        req.TimeMin = eventParams.start ?? req.TimeMin;
        req.TimeMax = eventParams.end ?? req.TimeMax;
        req.Q = eventParams.name ?? req.Q;
        req.MaxResults = eventParams.pageSize ?? req.MaxResults;
        req.OrderBy = EventsResource.ListRequest.OrderByEnum.Updated;

        int count = 1;

        List<Event> allEvents = new List<Event>();
        do
        {
            var events = req.Execute();
            IList<Event> pageEvents = events.Items;

            if (pageEvents != null && eventParams.pageNumber == count)
            {
                allEvents.AddRange(pageEvents);
            }

            req.PageToken = events.NextPageToken;
            count++;

        } while (!string.IsNullOrEmpty(req.PageToken));

        //var events = req.Execute();
        foreach (var e in allEvents)
        {
            CalenderItems.Add(new GoogleCalendarReqDTO()
            {
                Summary = e.Summary,
                Description = e.Description,
                StartTime = e.Start.DateTime,
                EndTime = e.End.DateTime,
                CalendarId = _configuration["calendarId"],
                EventId = e.Id
            });
        }
        return CalenderItems;

    }

    public string Add(GoogleCalendarReqDTO googleCalendarReqDTO)
    {
        try
        {
            var service = GetCalenderService(googleCalendarReqDTO.refreshToken);

            Event newEvent = new Event()
            {
                Summary = googleCalendarReqDTO.Summary,
                Description = googleCalendarReqDTO.Description,
                Start = new EventDateTime()
                {
                    DateTimeDateTimeOffset = googleCalendarReqDTO.StartTime,
                },
                End = new EventDateTime()
                {
                    DateTimeDateTimeOffset = googleCalendarReqDTO.EndTime,
                },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {

                new EventReminder() {
                    Method = "email", Minutes = 30
                  },

                  new EventReminder() {
                    Method = "popup", Minutes = 15
                  },

                  new EventReminder() {
                    Method = "popup", Minutes = 1
                  },
              }
                }
            };

            EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, _configuration["calendarId"]);
            Event createdEvent = insertRequest.Execute();
            return createdEvent.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }

    public string Delete(string id, string refreshToken)
    {
            var service = GetCalenderService(refreshToken);
            var req = service.Events.Delete(_configuration["calendarId"], id);
            var result = req.Execute();
            return id;
    }



    private CalendarService GetCalenderService(string refreshToken)
    {
        var token = new TokenResponse()
        {
            RefreshToken = refreshToken

        };
        var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
              new GoogleAuthorizationCodeFlow.Initializer
              {
                  ClientSecrets = new ClientSecrets
                  {
                      ClientId = _configuration["clientId"],
                      ClientSecret = _configuration["clientSecret"]
                  }

              }), "user", token);

        return new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credentials,
        });

    }

}
