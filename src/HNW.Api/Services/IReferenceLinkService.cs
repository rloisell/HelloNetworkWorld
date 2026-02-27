/*
 * IReferenceLinkService.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Service interface for documentation hub reference links.
 * AI-assisted: interface scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Data.Models;

namespace HNW.Api.Services;

// ── DTOS ─────────────────────────────────────────────────────────────────────

public record ReferenceLinkDto(
    Guid Id,
    string Category,
    string Title,
    string Url,
    string? Description,
    bool IsEnvironmentRelative,
    int SortOrder
);

public record CreateReferenceLinkRequest(
    ReferenceLinkCategory Category,
    string Title,
    string Url,
    string? Description,
    int SortOrder = 0
);

// ── INTERFACE ─────────────────────────────────────────────────────────────────

public interface IReferenceLinkService
{
    // returns all active reference links, optionally filtered by category
    Task<IReadOnlyList<ReferenceLinkDto>> GetAllAsync(ReferenceLinkCategory? category = null, CancellationToken ct = default);

    // returns a single link by id; throws NotFoundException if not found
    Task<ReferenceLinkDto> GetByIdAsync(Guid id, CancellationToken ct = default);

    // creates a new reference link
    Task<ReferenceLinkDto> CreateAsync(CreateReferenceLinkRequest request, CancellationToken ct = default);

    // updates an existing link; throws NotFoundException if not found
    Task<ReferenceLinkDto> UpdateAsync(Guid id, CreateReferenceLinkRequest request, CancellationToken ct = default);

    // soft-deletes a link (sets IsActive = false)
    Task DeleteAsync(Guid id, CancellationToken ct = default);
} // end IReferenceLinkService
