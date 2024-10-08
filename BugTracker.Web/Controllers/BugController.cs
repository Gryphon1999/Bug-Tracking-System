using BugTracker.Shared.Helper;
using BugTracker.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;

namespace BugTracker.Web.Controllers;

[Authorize]
public class BugController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public BugController(HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AppSetting.BaseUrl);
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: BugController
    public async Task<IActionResult> Index()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync("BugReport/GetPagedBugReport");

        var userDropdown = await _httpClient.GetAsync("BugReport/GetUserDropdown");

        if (userDropdown.IsSuccessStatusCode)
        {
            var users = await userDropdown.Content.ReadFromJsonAsync<ApiResponseHandler<List<AuthUserDto>>>();
            ViewBag.Users = new SelectList(users.Data, "Id", "UserName");
        }
        else
        {
            ViewBag.Users = new SelectList(new List<AuthUserDto>());
        }

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponseHandler<PagedListResult<BugReportDto>>>();
            return View(result.Data);
        }

        return View(new PagedListResult<BugReportDto>(new List<BugReportDto>(), 0, 1, 10));
    }

    // GET: BugController/Details/5
    public ActionResult Details(int id)
    {
        return View();
    }

    // GET: BugController/Create
    public async Task<IActionResult> Create()
    {
        var token = HttpContext.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("BugReport/GetUserDropdown");

        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<ApiResponseHandler<List<AuthUserDto>>>();
            ViewBag.Users = new SelectList(users.Data, "Id", "UserName");
        }
        else
        {
            ViewBag.Users = new SelectList(new List<AuthUserDto>());
        }

        return View();
    }

    // POST: BugController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(BugReportCreateUpdateDto createUpdateDto)
    {
        try
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (createUpdateDto.Attachments != null && createUpdateDto.Attachments.Count > 0)
            {
                var bugAttachments = new List<BugAttachmentCreateUpdateDto>();
                foreach (IFormFile file in createUpdateDto.Attachments)
                {
                    if (file.Length > 0)
                    {
                        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        bugAttachments.Add(new BugAttachmentCreateUpdateDto { FileName = fileName, FilePath = filePath });
                    }
                }
                createUpdateDto.BugAttachments.AddRange(bugAttachments);
            }
            createUpdateDto.Attachments = null;

            var response = await _httpClient.PostAsJsonAsync("bugreport/AddBugReport", createUpdateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Bug report has been added successfully.";
                return RedirectToAction("Index", "Bug");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while creating the bug report.";
                return View(createUpdateDto);
            }
        }
        catch
        {
            return View();
        }
    }

    // GET: BugController/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var token = HttpContext.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync($"BugReport/GetBugReportById/{id}");
        var userResponse = await _httpClient.GetAsync("BugReport/GetUserDropdown");

        if (userResponse.IsSuccessStatusCode)
        {
            var users = await userResponse.Content.ReadFromJsonAsync<ApiResponseHandler<List<AuthUserDto>>>();
            ViewBag.Users = new SelectList(users.Data, "Id", "UserName");
        }
        else
        {
            ViewBag.Users = new SelectList(new List<AuthUserDto>());
        }
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ApiResponseHandler<BugReportDto>>();
            var updateDto = new BugReportCreateUpdateDto
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                Description = result.Data.Description,
                ReproductionSteps = result.Data.ReproductionSteps,
                Severity = result.Data.Severity,
                Status = result.Data.Status,
                UserId = result.Data.UserId,
                BugAttachments = result.Data.FileName
                .Select(fileName => new BugAttachmentCreateUpdateDto { FileName = fileName })
                .ToList()
            };
            return View(updateDto);
        }

        return View();
    }

    // POST: BugController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, BugReportCreateUpdateDto createUpdateDto)
    {
        try
        {
            var token = HttpContext.Session.GetString("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (createUpdateDto.Attachments != null && createUpdateDto.Attachments.Count > 0)
            {
                var bugAttachments = new List<BugAttachmentCreateUpdateDto>();
                foreach (IFormFile file in createUpdateDto.Attachments)
                {
                    if (file.Length > 0)
                    {


                        var uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        bugAttachments.Add(new BugAttachmentCreateUpdateDto { FileName = fileName, FilePath = filePath });
                    }
                }
                createUpdateDto.BugAttachments.AddRange(bugAttachments);
            }
            createUpdateDto.Attachments = null;

            var response = await _httpClient.PutAsJsonAsync($"bugreport/UpdateBugReport/{id}", createUpdateDto);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Bug report has been updated successfully.";
                return RedirectToAction("Index", "Bug");
            }
            else
            {
                // Handle the error response here (you may want to log it or return an error message)
                ModelState.AddModelError(string.Empty, "An error occurred while creating the bug report.");
                return View(createUpdateDto); // Return the view with the original model for corrections
            }
        }
        catch
        {
            return View();
        }
    }

    // GET: BugController/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
        var token = HttpContext.Session.GetString("AccessToken");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.DeleteAsync($"BugReport/DeleteBugReport/{id}");
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Bug report deleted successfully.";
            return RedirectToAction("Index", "Bug");
        }

        TempData["ErrorMessage"] = "An error occurred while deleting the bug report.";
        return RedirectToAction("Index", "Bug");
    }

}
