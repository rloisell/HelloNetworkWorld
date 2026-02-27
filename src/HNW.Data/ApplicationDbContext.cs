/*
 * ApplicationDbContext.cs
 * Ryan Loiselle — Developer / Architect
 * GitHub Copilot — AI pair programmer / code generation
 * February 2026
 *
 * EF Core DbContext for HelloNetworkWorld. Manages all entities:
 * NetworkTestDefinitions, NetworkTestResults, NetworkTestStateChanges, ReferenceLinks.
 * AI-assisted: entity configuration scaffolding; reviewed and directed by Ryan Loiselle.
 */

using HNW.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HNW.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    // ── ENTITY SETS ──────────────────────────────────────────────────────────
    public DbSet<NetworkTestDefinition> NetworkTestDefinitions => Set<NetworkTestDefinition>();
    public DbSet<NetworkTestResult>     NetworkTestResults     => Set<NetworkTestResult>();
    public DbSet<NetworkTestStateChange> NetworkTestStateChanges => Set<NetworkTestStateChange>();
    public DbSet<ReferenceLink>         ReferenceLinks         => Set<ReferenceLink>();

    // ── CONFIGURATION ────────────────────────────────────────────────────────
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NetworkTestDefinition
        modelBuilder.Entity<NetworkTestDefinition>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Destination).HasMaxLength(253).IsRequired();
            e.Property(x => x.CronExpression).HasMaxLength(100).IsRequired();
            e.Property(x => x.ServiceType).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.PolicyStatus).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.PolicyPrUrl).HasMaxLength(500);
            e.HasMany(x => x.Results).WithOne(r => r.NetworkTestDefinition)
             .HasForeignKey(r => r.NetworkTestDefinitionId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.StateChanges).WithOne(s => s.NetworkTestDefinition)
             .HasForeignKey(s => s.NetworkTestDefinitionId).OnDelete(DeleteBehavior.Cascade);
        });

        // NetworkTestResult — index on ExecutedAt and FK for trendline performance
        modelBuilder.Entity<NetworkTestResult>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ExecutedAt);
            e.HasIndex(x => x.NetworkTestDefinitionId);
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
        });

        // NetworkTestStateChange
        modelBuilder.Entity<NetworkTestStateChange>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.ChangedAt);
        });

        // ReferenceLink
        modelBuilder.Entity<ReferenceLink>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Category).HasConversion<string>().HasMaxLength(50);
        });
    }

} // end ApplicationDbContext
