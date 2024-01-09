using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LineNotifyLab.Controllers;

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
  public async Task<ActionResult<LineNotifyResponse>> Post(string message = "我來自網站")
  {
    try
    {
      // POST https://notify-api.line.me/api/notify?message=我出運了圖圖&stickerPackageId=1&stickerId=2
      // 發行存取權杖（開發人員用）
      // MyLineNotifyStaff (自己)
      //const string AccessToken = @"ZXivR2Mnmb4s4qpRzfMlPekTPNwR7QOX3yJNUo9jtCH";
      // 測試 LINE Notify (群組)
      const string AccessToken = @"oW0nOgzS6swSUfCSPWnsLKhiOfEHr5zLSUyq1ng7Doa";
      // Y福 (個人)
      //const string AccessToken = @"AUNXGUjekMDCNSxRncWpWD7IoGwgfj9sc1t7EOXEDCO";

      using var _http = _httpFactory.CreateClient();
      _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
      _http.BaseAddress = new Uri($"https://notify-api.line.me");

      using var resp = await _http.PostAsync($"api/notify?message={message}@{DateTime.Now}。", new StringContent(string.Empty));
      var json = await resp.Content.ReadAsStringAsync();
      // {"status":400,"message":"LINE Notify account doesn't join group which you want to send."}
      var result = JsonSerializer.Deserialize<LineNotifyResponse>(json);

      if (result!.status != 200)
        return BadRequest(result);

      return Ok(result);        
    }
    catch (Exception ex)
    {
      return BadRequest("使用 LINE Notify 送訊息給自己出現例外！" + ex.Message);
      //throw new BadHttpRequestException("使用 LINE Notify 送訊息給自己出現例外！" + ex.Message, ex); // 送回完整的例外訊息。
    }
  }
}

public record LineNotifyResponse(int status, string message);