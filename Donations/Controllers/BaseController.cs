using Microsoft.AspNetCore.Mvc;

namespace Donation.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected IActionResult CustomResponse(object? data = null, string message = "Success", int statusCode = 200)
    {
        return StatusCode(statusCode, new
        {
            Success = true,
            Message = message,
            Errors = (object?)null,
            Data = data
        });
    }

    protected IActionResult CustomErrorResponse(
        string message,
        int statusCode = 400,
        object? errors = null)
    {
        return StatusCode(statusCode, new
        {
            Success = false,
            Message = message,
            Errors = errors,
            Data = (object?)null
        });
    }
}
