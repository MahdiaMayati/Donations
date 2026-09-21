using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    // دالة مساعدة لإرجاع استجابة ناجحة
    protected IActionResult CustomResponse(object? data = null, string message = "Success", int statusCode = 200)
    {
        return StatusCode(statusCode, new
        {
            Success = true,
            Message = message,
            Data = data
        });
    }

    // دالة مساعدة لإرجاع خطأ (مثل 400 أو 401)
    protected IActionResult CustomErrorResponse(string message, int statusCode = 400)
    {
        return StatusCode(statusCode, new
        {
            Success = false,
            Message = message,
            Data = (object?)null
        });
    }
}