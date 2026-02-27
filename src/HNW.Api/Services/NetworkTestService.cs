// NetworkTestService.cs — HNW.Api
// Ryan Loiselle — Developer / Architect
// GitHub Copilot — AI pair programmer / code generation
// February 2026
//
// Stub implementation of INetworkTestService.
// All CRUD delegates to ApplicationDbContext via EF Core.
// Implements: 003-network-test-config (WP02)
// AI-assisted: service implementation scaffold; reviewed and directed by Ryan Loiselle.

using HNW.Data;
using HNW.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HNW.Api.Services;

public class NetworkTestService(ApplicationDbContext db) : INetworkTestService
{
    public async Task<IEnumerable<NetworkTestDefinitionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var defs = await db.NetworkTestDefinitions
            .AsNoTracking()
            .ToListAsync(ct);

        // Attach last result per definition
        var ids = defs.Select(d => d.Id).ToList();
        var lastResults = await db.NetworkTestResults
            .Where(r => ids.Contains(r.NetworkTestDefinitionId))
            .GroupBy(r => r.NetworkTestDefinitionId)
            .Select(g => g.OrderByDescending(r => r.ExecutedAt).First())
            .ToListAsync(ct);

        var resultMap = lastResults.ToDictionary(r => r.NetworkTestDefinitionId);

        return defs.Select(d => ToDto(d, resultMap.GetValueOrDefault(d.Id)));
    }

    public async Task<NetworkTestDefinitionDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct);
        if (def is null) return null;

        var last = await db.NetworkTestResults
            .Where(r => r.NetworkTestDefinitionId == id)
            .OrderByDescending(r => r.ExecutedAt)
            .FirstOrDefaultAsync(ct);

        return ToDto(def, last);
    }

    public async Task<NetworkTestDefinitionDto> CreateAsync(CreateNetworkTestRequest request, CancellationToken ct = default)
    {
        var def = new NetworkTestDefinition
        {
            Name           = request.Name,
            Host           = request.Host,
            Port           = request.Port,
            ServiceType    = request.ServiceType,
            CronExpression = request.CronExpression,
            TimeoutMs      = request.TimeoutMs,
            IsEnabled      = request.IsEnabled,
            Description    = request.Description,
            CreatedAt      = DateTimeOffset.UtcNow,
            UpdatedAt      = DateTimeOffset.UtcNow,
        };

        db.NetworkTestDefinitions.Add(def);
        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    public async Task<NetworkTestDefinitionDto?> UpdateAsync(int id, UpdateNetworkTestRequest request, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct);
        if (def is null) return null;

        def.Name           = request.Name;
        def.Host           = request.Host;
        def.Port           = request.Port;
        def.ServiceType    = request.ServiceType;
        def.CronExpression = request.CronExpression;
        def.TimeoutMs      = request.TimeoutMs;
        def.IsEnabled      = request.IsEnabled;
        def.Description    = request.Description;
        def.UpdatedAt      = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct);
        if (def is null) return false;

        db.NetworkTestDefinitions.Remove(def);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<NetworkTestDefinitionDto?> ToggleEnabledAsync(int id, bool enabled, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct);
        if (def is null) return null;

        def.IsEnabled = enabled;
        def.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return ToDto(def, null);
    }

    public Task TriggerNowAsync(int id, CancellationToken ct = default)
    {
        // TODO: Implement: 004-network-test-execution — enqueue immediate Quartz job
        return Task.CompletedTask;
    }

    public async Task<TestResultSummaryDto?> GetResultSummaryAsync(int id, CancellationToken ct = default)
    {
        var def = await db.NetworkTestDefinitions.FindAsync([id], ct);
        if (def is null) return null;

        var results = await db.NetworkTestResults
            .Where(r => r.NetworkTestDefinitionId == id)
            .OrderByDescending(r => r.ExecutedAt)
            .Take(100)
            .ToListAsync(ct);

        return new TestResultSummaryDto(
            DefinitionId: id,
            Total:        results.Count,
            Passed:       results.Count(r => r.Passed),
            Failed:       results.Count(r => !r.Passed),
            AvgLatencyMs: results.Any() ? (double?)results.Average(r => r.LatencyMs ?? 0) : null,
            Results:      results.Select(ToResultDto).ToList()
        );
    }

    public async Task<IEnumerable<NetworkTestResultDto>> GetResultsAsync(int id, int limit = 50, CancellationToken ct = default)
    {
        var results = await db.NetworkTestResults
            .Where(r => r.NetworkTestDefinitionId == id)
            .OrderByDescending(r => r.ExecutedAt)
            .Take(limit)
            .ToListAsync(ct);

        return results.Select(ToResultDto);
    }

    // ── Mapping helpers ──────────────────────────────────────────────────────

    private static NetworkTestDefinitionDto ToDto(NetworkTestDefinition d, NetworkTestResult? last) =>
        new(
            Id:             d.Id,
            Name:           d.Name,
            Host:           d.Host,
            Port:           d.Port,
            ServiceType:    d.ServiceType.ToString(),
            CronExpression: d.CronExpression,
            TimeoutMs:      d.TimeoutMs,
            IsEnabled:      d.IsEnabled,
            Description:    d.Description,
            PolicyStatus:   d.PolicyStatus.ToString(),
            PolicyPrUrl:    d.PolicyPrUrl,
            CreatedAt:      d.CreatedAt,
            UpdatedAt:      d.UpdatedAt,
            LastResult:     last is null ? null : ToResultDto(last)
        );

    private static NetworkTestResultDto ToResultDto(NetworkTestResult r) =>
        new(
            Id:           r.Id,
            Passed:       r.Passed,
            LatencyMs:    r.LatencyMs,
            ErrorMessage: r.ErrorMessage,
            ExecutedAt:   r.ExecutedAt
        );
}
