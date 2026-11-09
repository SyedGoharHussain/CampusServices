using Campus.Business;
using Campus.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Campus.Api.Controllers;

[ApiController]
[Route("api/requests")]
[Authorize]   // every action needs a valid token unless stated otherwise
public class RequestsController : ControllerBase
{
    private readonly IRequestService _service;
    public RequestsController(IRequestService service) { _service = service; }

    private string UserId => User.FindFirst("uid")!.Value;
    private bool IsStaff => User.IsInRole("Staff");

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<RequestDto>>> List()
    {
        return await _service.GetListAsync(UserId, IsStaff);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestDto>> Get(int id)
    {
        var r = await _service.GetByIdAsync(id, UserId, IsStaff);
        if (r == null) return NotFound();
        return r;
    }

    [HttpPost]
    [ProducesResponseType(typeof(RequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RequestDto>> Create(CreateRequestDto dto)
    {
        var r = await _service.CreateAsync(dto, UserId);
        if (r == null) return BadRequest(new { error = "Category does not exist" });
        return CreatedAtAction(nameof(Get), new { id = r.Id }, r);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Staff")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
    {
        try
        {
            var found = await _service.UpdateStatusAsync(id, dto.NewStatus);
            if (!found) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/assign")]
    [Authorize(Roles = "Staff")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(int id)
    {
        var found = await _service.AssignAsync(id, UserId);
        if (!found) return NotFound();
        return NoContent();
    }

    [HttpGet("/api/categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<List<CategoryDto>> Categories()
    {
        return await _service.GetCategoriesAsync();
    }
}
