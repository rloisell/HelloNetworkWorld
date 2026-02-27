/*
 * NetworkTestsController.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Thin HTTP controller for network test definition CRUD.
 * Delegates all business logic to INetworkTestService.
 * AI-assisted: controller scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HNW.Api.Controllers;

[ApiController]
[Route("api/network-tests")]
public class NetworkTestsController(INetworkTestService service) : ControllerBase
{
    // ── QUERIES ──────────────────────────────────────────────────────────────

    // returns all network test definitions
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await service.GetAllAsync(ct);
        return Ok(items);
    }

    // returns a single network test definition by id
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var item = await service.GetByIdAsync(id, ct);
        return Ok(item);
    }

    // ── MUTATIONS ────────────────────────────────────────────────────────────

    // creates a new network test definition
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNetworkTestRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // updates an existing network test definition
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNetworkTestRequest request, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return Ok(updated);
    }

    // deletes a network test definition (soft delete)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    // enables or disables a network test without editing other fields
    [HttpPatch("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
    {
        var updated = await service.ToggleAsync(id, ct);
        return Ok(updated);
    }

} // end NetworkTestsController
