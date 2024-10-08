using BugTracker.Shared.Helper;
using BugTracker.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace BugTracker.Web.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(AppSetting.BaseUrl + "auth/Login"),
                    Content = content
                };
                var httpMessageResponse = await httpClient.SendAsync(request);
                var readContent = await httpMessageResponse.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponseHandler<TokenDto>>(readContent);
                if (result.Data != null && !string.IsNullOrEmpty(result.Data.AccessToken))
                {

                    HttpContext.Session.SetString("AccessToken", result.Data.AccessToken);
                    HttpContext.Session.SetString("TokenType", result.Data.TokenType);
                    return Redirect("~/home/Index");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                    return View();
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View();
        }
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AuthUserCreateUpdateDto userDto)
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(userDto), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(AppSetting.BaseUrl + "auth/Register"),
                    Content = content
                };
                var httpMessageResponse = await httpClient.SendAsync(request);
                if (httpMessageResponse.IsSuccessStatusCode)
                {
                    return Redirect("~/account/Login");
                }
                else
                {
                    var readContent = await httpMessageResponse.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = readContent;
                    return View();
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View();
        }
    }

    [Authorize]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("~/account/Login");
    }
}
