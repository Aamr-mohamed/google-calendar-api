using Google.Apis.Calendar.v3.Data;
using GoogleCalenderApi.EventUtils;

public interface IGoogleCalendarService {
  string GetAuthCode();

  Task < GoogleTokenResponse > GetTokens(string code);
  string Add(GoogleCalendarReqDTO googleCalendarReqDTO);
  List<GoogleCalendarReqDTO> GetAll(EventSearchParams eventSearch);
  string Delete(string id, string refreshToken);
}