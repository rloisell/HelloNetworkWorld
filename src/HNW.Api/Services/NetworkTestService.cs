/*
 * NetworkTestService.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Implementation of INetworkTestService — CRUD for network test definitions.
 * All database access via ApplicationDbContext + EF Core.
 * Implements: 003-network-test-config (WP02)
 * AI-assisted: service implementation; reviewed and directed by Ryan Loiselle.
 */

using HNW.Api.Infrastructure;
using HNW.Data;
using HNW.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HNW.Api.Services;

public class NetworkTestService(ApplicationDbContext db) : INetworkTestService
{
    // ── QUERIES ──────────────────────────────────────────────────────────────

    // returns all network test definitions with their latest result, newest first
    public async Task<IReadOnlyList<NetworkTestDefinitionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var defs = await db.NetworkTestDefinitions
            .AsNoTracking()
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync(ct);

        // Attach most recent result per definition
        var ids = defs.Select(d => d.Id).ToList();
        var lastResults = await db.NetworkTestResults
            .Where(r => ids.Contains(r.NetworkTestDefinitionId))
            .GroupBy(r => r.NetworkTestDefinitionId)
            .Select(g => g.OrderByDescending(r => r.ExecutedAt).First())
            .ToListAsync(ct);

        var resultMap = lastResults.ToDictionary(r => r.NetworkTestDefinitionId);
        return defs.Select(d => ToDto(d, resultMap.GetValueOrDefault(d.Id))).ToList();
    }

    // returns a single definition by id; throws NotFoundException if not found
    public async Task<NetworkTestDefinitionDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct)
            ?? throw new NotFoundException($"NetworkTestDefinition {id} not found.");

        var last = await db.NetworkTestResults
            .Where(r => r.NetworkTestDefinitionId == id)
            .OrderByDescending(r => r.ExecutedAt)
            .FirstOrDefaultAsync(ct);

        return ToDto(def, last);
    }

    // ── MUTATIONS ────────────────────────────────────────────────────────────

    // creates a new definition; returns the created DTO
    public async Task<NetworkTestDefinitionDto> CreateAsync(CreateNetworkTestRequest request, CancellationToken ct = default)
    {
        var def = new NetworkTestDefinition
        {
            Name           = request.Name,
            Destination    = request.Destination,
            Port           = request.Port,
            ServiceType    = request.ServiceType,
            CronExpression = request.CronExpression,
            IsEnabled      = true,
            CreatedAt      = DateTimeOffset.UtcNow,
            UpdatedAt      = DateTimeOffset.UtcNow,
        };

        db.NetworkTestDefinitions.Add(def);
        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    // updates a definition; returns updated DTO; throws NotFoundException
    public async Task<NetworkTestDefinitionDto> UpdateAsync(
        Guid id, UpdateNetworkTestRequest request, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct)
            ?? throw new NotFoundException($"NetworkTestDefinition {id} not found.");

        def.Name           = request.Name;
        def.Destination    = request.Destination;
        def.Port           = request.Port;
        def.ServiceType    = request.ServiceType;
        def.CronExpression = request.CronExpression;
        def.IsEnabled      = request.IsEnabled;
        def.UpdatedAt      = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    // soft-deletes a definition; throws NotFoundException
    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct)
            ?? throw new NotFoundException($"NetworkTestDefinition {id} not found.");

        // Hard delete for now; switch to soft delete with IsDeleted flag in Phase 2
        db.NetworkTestDefinitions.Remove(def);
        await db.SaveChangesAsync(ct);
    }

    // toggles IsEnabled and returns updated DTO; throws NotFoundException
    public async Task<NetworkTestDefinitionDto> ToggleAsync(Guid id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct)
            ?? throw new NotFoundException($"NetworkTestDefinition {id} not found.");

        def.IsEnabled = !def.IsEnabled;
        def.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    // ── MAPPING HELPERS ──────────────────────────────────────────────────────

    // maps entity + optional last result to DTO
    private static NetworkTestDefinitionDto ToDto(NetworkTestDefinition d, NetworkTestResult? last) =>
        new(
            Id:             d.Id,
            Name:           d.Name,
            Destination:    d.Destination,
            Port:           d.Port,
            ServiceType:    d.ServiceType.ToString(),
            CronExpression: d.CronExpression,
            IsEnabled:      d.IsEnabled,
            PolicyStatus:   d.PolicyStatus.ToString(),
            PolicyPrUrl:    d.PolicyPrUrl,
            CreatedAt:      d.CreatedAt,
            UpdatedAt:      d.UpdatedAt,
            LatestResult:   last is null ? null : new NetworkTestResultSummaryDto(
                IsSuccess:    last.IsSuccess,
                LatencyMs:    last.LatencyMs,
                ErrorMessage: last.ErrorMessage,
                ExecutedAt:   last.ExecutedAt
            )
        );

} // end NetworkTestService
