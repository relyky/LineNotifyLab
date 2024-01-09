using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LineNotifyLab.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LineNotifyMeController : ControllerBase
  {
    //# Injection Member
    readonly IHttpClientFactory _httpFactory;

    public LineNotifyMeController(IHttpClientFactory httpFactory)
    {
      _httpFactory = httpFactory;
    }

    [HttpPost(Name = "NotifyMe")]
    public async Task<ActionResult<string>> Post(string message = "我來自網站")
    {
      try
      {
        // POST https://notify-api.line.me/api/notify?message=我出運了圖圖&stickerPackageId=1&stickerId=2
        // 發行存取權杖（開發人員用）
        // MyLineNotifyStaff
        // ZXivR2Mnmb4s4qpRzfMlPekTPNwR7QOX3yJNUo9jtCH

        const string AccessToken = @"ZXivR2Mnmb4s4qpRzfMlPekTPNwR7QOX3yJNUo9jtCH";

        using var _http = _httpFactory.CreateClient();
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
        _http.BaseAddress = new Uri($"https://notify-api.line.me");

        HttpContent content = new StringContent("");

        using var resp = await _http.PostAsync($"api/notify?message={message}@{DateTime.Now}。", new StringContent(string.Empty));
        var result = await resp.Content.ReadAsStringAsync();
        //var result = JsonSerializer.Deserialize<FooBarResp>(json);
        
        return Ok("SUCCESS. " + result);
      }
      catch (Exception ex)
      {
        return BadRequest("使用 LINE Notify 送訊息給自己出現例外！" + ex.Message);
        //throw new BadHttpRequestException("使用 LINE Notify 送訊息給自己出現例外！" + ex.Message, ex); // 送回完整的例外訊息。
      }
    }
  }
}
