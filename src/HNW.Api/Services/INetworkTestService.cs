/*
 * INetworkTestService.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * Service interface for network test definition CRUD and scheduling.
 * AI-assisted: interface scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Data.Models;

namespace HNW.Api.Services;

// ── DTOS ─────────────────────────────────────────────────────────────────────

public record NetworkTestDefinitionDto(
    Guid Id,
    string Name,
    string Destination,
    int? Port,
    string ServiceType,
    string CronExpression,
    bool IsEnabled,
    string PolicyStatus,
    string? PolicyPrUrl,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    NetworkTestResultSummaryDto? LatestResult
);

public record NetworkTestResultSummaryDto(
    bool IsSuccess,
    int? LatencyMs,
    string? ErrorMessage,
    DateTimeOffset ExecutedAt
);

public record CreateNetworkTestRequest(
    string Name,
    string Destination,
    int? Port,
    NetworkServiceType ServiceType,
    string CronExpression
);

public record UpdateNetworkTestRequest(
    string Name,
    string Destination,
    int? Port,
    NetworkServiceType ServiceType,
    string CronExpression,
    bool IsEnabled
);

// ── INTERFACE ─────────────────────────────────────────────────────────────────

public interface INetworkTestService
{
    // returns all network test definitions, newest first
    Task<IReadOnlyList<NetworkTestDefinitionDto>> GetAllAsync(CancellationToken ct = default);

    // returns a single definition by id; throws NotFoundException if not found
    Task<NetworkTestDefinitionDto> GetByIdAsync(Guid id, CancellationToken ct = default);

    // creates a new definition, schedules a Quartz job, triggers policy check; returns the created DTO
    Task<NetworkTestDefinitionDto> CreateAsync(CreateNetworkTestRequest request, CancellationToken ct = default);

    // updates a definition, reschedules the Quartz job if schedule changed; returns updated DTO
    Task<NetworkTestDefinitionDto> UpdateAsync(Guid id, UpdateNetworkTestRequest request, CancellationToken ct = default);

    // soft-deletes a definition and removes the Quartz job
    Task DeleteAsync(Guid id, CancellationToken ct = default);

    // enables or disables a test without changing other fields; returns updated DTO
    Task<NetworkTestDefinitionDto> ToggleAsync(Guid id, CancellationToken ct = default);
} // end INetworkTestService
