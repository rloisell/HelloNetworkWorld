// ReferenceLinkService.cs — HNW.Api
// Ryan Loiselle — Developer / Architect
// GitHub Copilot — AI pair programmer / code generation
// February 2026
//
// Stub implementation of IReferenceLinkService.
// Implements: 002-documentation-hub
// AI-assisted: service implementation scaffold; reviewed and directed by Ryan Loiselle.

using HNW.Api.Infrastructure;
using HNW.Data;
using HNW.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HNW.Api.Services;

public class ReferenceLinkService(ApplicationDbContext db) : IReferenceLinkService
{
    public async Task<IReadOnlyList<ReferenceLinkDto>> GetAllAsync(
        ReferenceLinkCategory? category = null,
        CancellationToken ct = default)
    {
        var q = db.ReferenceLinks
            .AsNoTracking()
            .Where(l => l.IsActive);

        if (category.HasValue)
            q = q.Where(l => l.Category == category.Value);

        var links = await q
            .OrderBy(l => l.Category)
            .ThenBy(l => l.SortOrder)
            .ThenBy(l => l.Title)
            .ToListAsync(ct);

        return links.Select(ToDto).ToList();
    }

    public async Task<ReferenceLinkDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var link = await db.ReferenceLinks.FindAsync([id], ct)
            ?? throw new NotFoundException($"ReferenceLink {id} not found.");
        return ToDto(link);
    }

    public async Task<ReferenceLinkDto> CreateAsync(CreateReferenceLinkRequest request, CancellationToken ct = default)
    {
        var link = new ReferenceLink
        {
            Category             = request.Category,
            Title                = request.Title,
            Url                  = request.Url,
            Description          = request.Description,
            SortOrder            = request.SortOrder,
        };
        db.ReferenceLinks.Add(link);
        await db.SaveChangesAsync(ct);
        return ToDto(link);
    }

    public async Task<ReferenceLinkDto> UpdateAsync(Guid id, CreateReferenceLinkRequest request, CancellationToken ct = default)
    {
        var link = await db.ReferenceLinks.FindAsync([id], ct)
            ?? throw new NotFoundException($"ReferenceLink {id} not found.");

        link.Category    = request.Category;
        link.Title       = request.Title;
        link.Url         = request.Url;
        link.Description = request.Description;
        link.SortOrder   = request.SortOrder;

        await db.SaveChangesAsync(ct);
        return ToDto(link);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var link = await db.ReferenceLinks.FindAsync([id], ct)
            ?? throw new NotFoundException($"ReferenceLink {id} not found.");
        link.IsActive = false;
        await db.SaveChangesAsync(ct);
    }

    private static ReferenceLinkDto ToDto(ReferenceLink l) =>
        new(
            Id:                    l.Id,
            Category:              l.Category.ToString(),
            Title:                 l.Title,
            Url:                   l.Url,
            Description:           l.Description,
            IsEnvironmentRelative: l.IsEnvironmentRelative,
            SortOrder:             l.SortOrder
        );
}
