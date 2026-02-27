// ReferenceLinksController.cs — HNW.Api
// Ryan Loiselle — Developer / Architect
// GitHub Copilot — AI pair programmer / code generation
// February 2026
//
// Thin HTTP controller for the documentation hub reference links.
// Implements: 002-documentation-hub
// AI-assisted: controller scaffold; reviewed and directed by Ryan Loiselle.

using HNW.Api.Services;
using HNW.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace HNW.Api.Controllers;

[ApiController]
[Route("api/reference-links")]
public class ReferenceLinksController(IReferenceLinkService service) : ControllerBase
{
    /// <summary>Returns all reference links ordered by category and sort order.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReferenceLinkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReferenceLinkDto>>> GetAll(
        [FromQuery] ReferenceLinkCategory? category,
        CancellationToken ct)
    {
        var links = await service.GetAllAsync(category, ct);
        return Ok(links);
    }
}
