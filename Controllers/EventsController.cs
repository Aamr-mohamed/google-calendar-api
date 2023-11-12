using GoogleCalenderApi.EventUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

public class UserController : Controller
{
    private IGoogleCalendarService _googleCalendarService;
    public UserController(IGoogleCalendarService googleCalendarService, IConfiguration configuration)

    {
        _googleCalendarService = googleCalendarService;
    }

    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route("/auth")]
    public async Task<IActionResult> GoogleAuth()
    {
        return Redirect(_googleCalendarService.GetAuthCode());
    }

    [HttpGet]
    [Route("/api/auth/callback")]
    public async Task<IActionResult> Callback()
    {
        string code = HttpContext.Request.Query["code"];
        var token = await _googleCalendarService.GetTokens(code);
        return Ok(token);
    }

    [HttpGet]
    [Route("/api/events")]
    public IActionResult Get([FromQuery] EventSearchParams eventParams)
    {
        try
        {
            var data = _googleCalendarService.GetAll(eventParams);
            return StatusCode(StatusCodes.Status200OK, data);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [HttpPost]
    [Route("/api/events")]
    public IActionResult Post([FromBody] GoogleCalendarReqDTO calendarEventReqDTO)
    {
        try
        {
            Console.WriteLine(calendarEventReqDTO.Summary);
            var data = _googleCalendarService.Add(calendarEventReqDTO);
            calendarEventReqDTO.refreshToken = null;
            return StatusCode(StatusCodes.Status201Created, calendarEventReqDTO);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e);
        }
    }

    [HttpDelete]
    [Route("/api/events")]
    public IActionResult Delete(string id, string refreshToken)
    {
        try
        {
            var data = _googleCalendarService.Delete(id, refreshToken);
            return StatusCode(StatusCodes.Status204NoContent);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}