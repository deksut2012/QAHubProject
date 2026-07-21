using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using QAHub.Api.Domain.Auditing;
using QAHub.Api.Domain.Identity;
using QAHub.Api.Domain.Products;
using QAHub.Api.Domain.Requirements;
using QAHub.Api.Domain.TestDesign;
using QAHub.Api.Domain.Execution;
using QAHub.Api.Domain.Defects;
using QAHub.Api.Domain.Releases;
using QAHub.Api.Domain.Reporting;
using QAHub.Api.Domain.Toolbox;
using QAHub.Api.Domain.Integrations;

namespace QAHub.Api.Infrastructure.Data;

public sealed class QAHubDbContext(
    DbContextOptions<QAHubDbContext> options,
    IHttpContextAccessor? httpContextAccessor = null) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductModule> ProductModules => Set<ProductModule>();
    public DbSet<ProductEnvironment> ProductEnvironments => Set<ProductEnvironment>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<AppRole> Roles => Set<AppRole>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Requirement> Requirements => Set<Requirement>();
    public DbSet<RequirementComment> RequirementComments => Set<RequirementComment>();
    public DbSet<RequirementAttachment> RequirementAttachments => Set<RequirementAttachment>();
    public DbSet<TestCase> TestCases => Set<TestCase>();
    public DbSet<TestCaseComment> TestCaseComments => Set<TestCaseComment>();
    public DbSet<TestCaseVersion> TestCaseVersions => Set<TestCaseVersion>();
    public DbSet<TestCaseStep> TestCaseSteps => Set<TestCaseStep>();
    public DbSet<TestCaseHistoryEntry> TestCaseHistory => Set<TestCaseHistoryEntry>();
    public DbSet<ProductBuild> ProductBuilds => Set<ProductBuild>(); public DbSet<TestCycle> TestCycles => Set<TestCycle>(); public DbSet<TestCycleItem> TestCycleItems => Set<TestCycleItem>(); public DbSet<TestRunAttempt> TestRunAttempts => Set<TestRunAttempt>(); public DbSet<TestRunEvidence> TestRunEvidenceFiles => Set<TestRunEvidence>();
    public DbSet<Bug> Bugs => Set<Bug>(); public DbSet<BugRunLink> BugRunLinks => Set<BugRunLink>(); public DbSet<BugStatusHistory> BugStatusHistories => Set<BugStatusHistory>(); public DbSet<BugComment> BugComments => Set<BugComment>(); public DbSet<BugEvidence> BugEvidenceFiles => Set<BugEvidence>(); public DbSet<BugRelation> BugRelations => Set<BugRelation>();
    public DbSet<ReleaseCandidate> Releases => Set<ReleaseCandidate>(); public DbSet<ReleaseChecklistItem> ReleaseChecklistItems => Set<ReleaseChecklistItem>(); public DbSet<ReleaseRequirement> ReleaseRequirements => Set<ReleaseRequirement>(); public DbSet<ReleaseKnownIssue> ReleaseKnownIssues => Set<ReleaseKnownIssue>();
    public DbSet<ScheduledReport> ScheduledReports => Set<ScheduledReport>();
    public DbSet<SavedSqlQuery> SavedSqlQueries => Set<SavedSqlQuery>();
    public DbSet<AutomationRun> AutomationRuns => Set<AutomationRun>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<Product>();
        product.ToTable("Products");
        product.HasKey(x => x.Id);
        product.Property(x => x.Code).HasMaxLength(32).IsRequired();
        product.Property(x => x.Name).HasMaxLength(200).IsRequired();
        product.Property(x => x.RowVersion).IsRowVersion();
        product.HasIndex(x => x.Code).IsUnique();

        var module = modelBuilder.Entity<ProductModule>();
        module.ToTable("ProductModules");
        module.HasKey(x => x.Id);
        module.Property(x => x.Code).HasMaxLength(32).IsRequired();
        module.Property(x => x.Name).HasMaxLength(200).IsRequired();
        module.HasIndex(x => new { x.ProductId, x.Code }).IsUnique();
        module.HasOne(x => x.Product)
            .WithMany(x => x.Modules)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var environment = modelBuilder.Entity<ProductEnvironment>();
        environment.ToTable("ProductEnvironments");
        environment.HasKey(x => x.Id);
        environment.Property(x => x.Code).HasMaxLength(32).IsRequired();
        environment.Property(x => x.Name).HasMaxLength(200).IsRequired();
        environment.HasIndex(x => new { x.ProductId, x.Code }).IsUnique();
        environment.HasOne(x => x.Product)
            .WithMany(x => x.Environments)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        var audit = modelBuilder.Entity<AuditEvent>();
        audit.ToTable("AuditEvents");
        audit.HasKey(x => x.Id);
        audit.Property(x => x.ActorId).HasMaxLength(200).IsRequired();
        audit.Property(x => x.Action).HasMaxLength(100).IsRequired();
        audit.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
        audit.Property(x => x.CorrelationId).HasMaxLength(200).IsRequired();
        audit.Property(x => x.ChangesJson).HasColumnType("nvarchar(max)").IsRequired();
        audit.HasIndex(x => new { x.EntityType, x.EntityId, x.OccurredAtUtc });
        audit.HasIndex(x => new { x.ProductId, x.OccurredAtUtc });

        var user = modelBuilder.Entity<UserAccount>();
        user.ToTable("Users"); user.HasKey(x => x.Id);
        user.Property(x => x.ExternalId).HasMaxLength(200).IsRequired();
        user.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
        user.Property(x => x.Email).HasMaxLength(320);
        user.HasIndex(x => x.ExternalId).IsUnique();

        var role = modelBuilder.Entity<AppRole>();
        role.ToTable("Roles"); role.HasKey(x => x.Id);
        role.Property(x => x.Code).HasMaxLength(100).IsRequired();
        role.Property(x => x.Name).HasMaxLength(200).IsRequired();
        role.HasIndex(x => x.Code).IsUnique();

        var userRole = modelBuilder.Entity<UserRole>();
        userRole.ToTable("UserRoles"); userRole.HasKey(x => new { x.UserId, x.RoleId });
        userRole.HasOne(x => x.User).WithMany(x => x.Roles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        userRole.HasOne(x => x.Role).WithMany(x => x.Users).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Restrict);

        var requirement = modelBuilder.Entity<Requirement>();
        requirement.ToTable("Requirements"); requirement.HasKey(x => x.Id);
        requirement.Property(x => x.JobNumber).HasMaxLength(50).IsRequired();
        requirement.Property(x => x.Title).HasMaxLength(250).IsRequired();
        requirement.Property(x => x.Description).HasColumnType("nvarchar(max)").IsRequired();
        requirement.Property(x => x.AcceptanceCriteria).HasColumnType("nvarchar(max)").IsRequired();
        requirement.Property(x => x.Assignee).HasMaxLength(200);
        requirement.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        requirement.Property(x => x.RowVersion).IsRowVersion();
        requirement.HasIndex(x => new { x.ProductId, x.JobNumber }).IsUnique();
        requirement.HasIndex(x => new { x.ProductId, x.Status, x.UpdatedAtUtc });
        requirement.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        requirement.HasOne<ProductModule>().WithMany().HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Restrict);

        var comment = modelBuilder.Entity<RequirementComment>();
        comment.ToTable("RequirementComments"); comment.HasKey(x => x.Id);
        comment.Property(x => x.AuthorId).HasMaxLength(200).IsRequired();
        comment.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        comment.HasIndex(x => new { x.RequirementId, x.CreatedAtUtc });
        comment.HasOne<Requirement>().WithMany().HasForeignKey(x => x.RequirementId).OnDelete(DeleteBehavior.Cascade);

        var attachment = modelBuilder.Entity<RequirementAttachment>();
        attachment.ToTable("RequirementAttachments"); attachment.HasKey(x => x.Id);
        attachment.Property(x => x.FileName).HasMaxLength(255).IsRequired();
        attachment.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        attachment.Property(x => x.Content).HasColumnType("varbinary(max)").IsRequired();
        attachment.Property(x => x.UploadedBy).HasMaxLength(200).IsRequired();
        attachment.HasIndex(x => new { x.RequirementId, x.UploadedAtUtc });
        attachment.HasOne<Requirement>().WithMany().HasForeignKey(x => x.RequirementId).OnDelete(DeleteBehavior.Cascade);

        var testCase = modelBuilder.Entity<TestCase>();
        testCase.ToTable("TestCases"); testCase.HasKey(x => x.Id);
        testCase.Property(x => x.Code).HasMaxLength(50).IsRequired(); testCase.Property(x => x.RowVersion).IsRowVersion();
        testCase.HasIndex(x => new { x.ProductId, x.Code }).IsUnique();
        testCase.HasOne<Product>().WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        testCase.HasOne<ProductModule>().WithMany().HasForeignKey(x => x.ModuleId).OnDelete(DeleteBehavior.Restrict);
        testCase.HasOne<Requirement>().WithMany().HasForeignKey(x => x.RequirementId).OnDelete(DeleteBehavior.Restrict);
        testCase.HasOne<TestCase>().WithMany().HasForeignKey(x => x.SourceTestCaseId).OnDelete(DeleteBehavior.NoAction);
        testCase.HasOne<TestCaseVersion>().WithMany().HasForeignKey(x => x.SourceVersionId).OnDelete(DeleteBehavior.NoAction);
        testCase.HasMany(x => x.Versions).WithOne().HasForeignKey(x => x.TestCaseId).OnDelete(DeleteBehavior.Cascade);

        var testCaseComment = modelBuilder.Entity<TestCaseComment>();
        testCaseComment.ToTable("TestCaseComments"); testCaseComment.HasKey(x => x.Id);
        testCaseComment.Property(x => x.AuthorId).HasMaxLength(200).IsRequired();
        testCaseComment.Property(x => x.Body).HasMaxLength(4000).IsRequired();
        testCaseComment.HasIndex(x => new { x.TestCaseId, x.CreatedAtUtc });
        testCaseComment.HasOne<TestCase>().WithMany().HasForeignKey(x => x.TestCaseId).OnDelete(DeleteBehavior.Cascade);

        var testVersion = modelBuilder.Entity<TestCaseVersion>();
        testVersion.ToTable("TestCaseVersions"); testVersion.HasKey(x => x.Id);
        testVersion.Property(x => x.Title).HasMaxLength(250).IsRequired();
        testVersion.Property(x => x.Scenario).HasColumnType("nvarchar(max)").IsRequired();
        testVersion.Property(x => x.Preconditions).HasColumnType("nvarchar(max)").IsRequired();
        testVersion.Property(x => x.Tags).HasMaxLength(500).IsRequired();
        testVersion.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        testVersion.HasIndex(x => new { x.TestCaseId, x.VersionNumber }).IsUnique();
        testVersion.HasMany(x => x.Steps).WithOne().HasForeignKey(x => x.TestCaseVersionId).OnDelete(DeleteBehavior.Cascade);
        testVersion.HasMany(x => x.History).WithOne().HasForeignKey(x => x.TestCaseVersionId).OnDelete(DeleteBehavior.Cascade);

        var testStep = modelBuilder.Entity<TestCaseStep>();
        testStep.ToTable("TestCaseSteps"); testStep.HasKey(x => x.Id);
        testStep.Property(x => x.Action).HasMaxLength(2000).IsRequired();
        testStep.Property(x => x.TestData).HasMaxLength(2000).IsRequired();
        testStep.Property(x => x.ExpectedResult).HasMaxLength(2000).IsRequired();
        testStep.HasIndex(x => new { x.TestCaseVersionId, x.Sequence }).IsUnique();

        var history = modelBuilder.Entity<TestCaseHistoryEntry>();
        history.ToTable("TestCaseHistory"); history.HasKey(x => x.Id);
        history.Property(x => x.ActorId).HasMaxLength(200).IsRequired();
        history.Property(x => x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
        var build=modelBuilder.Entity<ProductBuild>();build.ToTable("ProductBuilds");build.HasKey(x=>x.Id);build.Property(x=>x.Version).HasMaxLength(100).IsRequired();build.HasIndex(x=>new{x.ProductId,x.Version}).IsUnique();build.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
        var cycle=modelBuilder.Entity<TestCycle>();cycle.ToTable("TestCycles");cycle.HasKey(x=>x.Id);cycle.Property(x=>x.Name).HasMaxLength(250).IsRequired();cycle.Property(x=>x.Assignee).HasMaxLength(200);cycle.Property(x=>x.Status).HasConversion<string>().HasMaxLength(30).IsRequired();cycle.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);cycle.HasOne<ProductEnvironment>().WithMany().HasForeignKey(x=>x.EnvironmentId).OnDelete(DeleteBehavior.Restrict);cycle.HasOne<ProductBuild>().WithMany().HasForeignKey(x=>x.BuildId).OnDelete(DeleteBehavior.Restrict);cycle.HasMany(x=>x.Items).WithOne().HasForeignKey(x=>x.TestCycleId).OnDelete(DeleteBehavior.Cascade);
        var cycleItem=modelBuilder.Entity<TestCycleItem>();cycleItem.ToTable("TestCycleItems");cycleItem.HasKey(x=>x.Id);cycleItem.Property(x=>x.Assignee).HasMaxLength(200);cycleItem.HasIndex(x=>new{x.TestCycleId,x.TestCaseVersionId}).IsUnique();cycleItem.HasOne<TestCaseVersion>().WithMany().HasForeignKey(x=>x.TestCaseVersionId).OnDelete(DeleteBehavior.Restrict);cycleItem.HasMany(x=>x.Attempts).WithOne().HasForeignKey(x=>x.TestCycleItemId).OnDelete(DeleteBehavior.Cascade);
        var attempt=modelBuilder.Entity<TestRunAttempt>();attempt.ToTable("TestRunAttempts");attempt.HasKey(x=>x.Id);attempt.Property(x=>x.Result).HasConversion<string>().HasMaxLength(20);attempt.Property(x=>x.ActualResult).HasMaxLength(4000);attempt.Property(x=>x.Evidence).HasMaxLength(2000);attempt.Property(x=>x.Reason).HasMaxLength(2000);attempt.Property(x=>x.ExecutedBy).HasMaxLength(200);attempt.HasIndex(x=>new{x.TestCycleItemId,x.AttemptNumber}).IsUnique();attempt.HasMany(x=>x.EvidenceFiles).WithOne().HasForeignKey(x=>x.TestRunAttemptId).OnDelete(DeleteBehavior.Cascade);
        var runEvidence=modelBuilder.Entity<TestRunEvidence>();runEvidence.ToTable("TestRunEvidence");runEvidence.HasKey(x=>x.Id);runEvidence.Property(x=>x.FileName).HasMaxLength(255).IsRequired();runEvidence.Property(x=>x.ContentType).HasMaxLength(200).IsRequired();runEvidence.Property(x=>x.Content).HasColumnType("varbinary(max)").IsRequired();runEvidence.Property(x=>x.UploadedBy).HasMaxLength(200).IsRequired();runEvidence.HasIndex(x=>new{x.TestRunAttemptId,x.UploadedAtUtc});
        var bug=modelBuilder.Entity<Bug>();bug.ToTable("Bugs");bug.HasKey(x=>x.Id);bug.Property(x=>x.Code).HasMaxLength(50).IsRequired();bug.Property(x=>x.Title).HasMaxLength(250).IsRequired();bug.Property(x=>x.Description).HasMaxLength(4000);bug.Property(x=>x.StepsToReproduce).HasMaxLength(4000).IsRequired();bug.Property(x=>x.ExpectedResult).HasMaxLength(4000);bug.Property(x=>x.ActualResult).HasMaxLength(4000).IsRequired();bug.Property(x=>x.Severity).HasConversion<string>().HasMaxLength(20);bug.Property(x=>x.Priority).HasConversion<string>().HasMaxLength(20);bug.Property(x=>x.Status).HasConversion<string>().HasMaxLength(30);bug.Property(x=>x.Reporter).HasMaxLength(200);bug.Property(x=>x.Assignee).HasMaxLength(200);bug.HasIndex(x=>new{x.ProductId,x.Code}).IsUnique();bug.HasIndex(x=>new{x.ProductId,x.Status,x.CreatedAtUtc});bug.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);bug.HasOne<ProductBuild>().WithMany().HasForeignKey(x=>x.FixBuildId).OnDelete(DeleteBehavior.Restrict);bug.HasOne<Bug>().WithMany().HasForeignKey(x=>x.CanonicalBugId).OnDelete(DeleteBehavior.NoAction);bug.HasOne<TestRunAttempt>().WithMany().HasForeignKey(x=>x.VerificationAttemptId).OnDelete(DeleteBehavior.NoAction);bug.HasMany(x=>x.RunLinks).WithOne().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Cascade);bug.HasMany(x=>x.History).WithOne().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Cascade);bug.HasMany(x=>x.Comments).WithOne().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Cascade);bug.HasMany(x=>x.EvidenceFiles).WithOne().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Cascade);bug.HasMany(x=>x.Relations).WithOne().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Cascade);
        var bugRun=modelBuilder.Entity<BugRunLink>();bugRun.ToTable("BugRunLinks");bugRun.HasKey(x=>x.Id);bugRun.HasIndex(x=>new{x.BugId,x.TestRunAttemptId}).IsUnique();bugRun.HasOne<TestRunAttempt>().WithMany().HasForeignKey(x=>x.TestRunAttemptId).OnDelete(DeleteBehavior.Restrict);
        var bugHistory=modelBuilder.Entity<BugStatusHistory>();bugHistory.ToTable("BugStatusHistory");bugHistory.HasKey(x=>x.Id);bugHistory.Property(x=>x.FromStatus).HasConversion<string>().HasMaxLength(30);bugHistory.Property(x=>x.ToStatus).HasConversion<string>().HasMaxLength(30);bugHistory.Property(x=>x.ActorId).HasMaxLength(200);bugHistory.Property(x=>x.Reason).HasMaxLength(2000);bugHistory.HasIndex(x=>new{x.BugId,x.ChangedAtUtc});
        var bugComment=modelBuilder.Entity<BugComment>();bugComment.ToTable("BugComments");bugComment.HasKey(x=>x.Id);bugComment.Property(x=>x.AuthorId).HasMaxLength(200);bugComment.Property(x=>x.Body).HasMaxLength(4000);bugComment.HasIndex(x=>new{x.BugId,x.CreatedAtUtc});
        var bugEvidence=modelBuilder.Entity<BugEvidence>();bugEvidence.ToTable("BugEvidence");bugEvidence.HasKey(x=>x.Id);bugEvidence.Property(x=>x.FileName).HasMaxLength(255);bugEvidence.Property(x=>x.ContentType).HasMaxLength(200);bugEvidence.Property(x=>x.Content).HasColumnType("varbinary(max)");bugEvidence.Property(x=>x.UploadedBy).HasMaxLength(200);bugEvidence.HasIndex(x=>new{x.BugId,x.UploadedAtUtc});
        var bugRelation=modelBuilder.Entity<BugRelation>();bugRelation.ToTable("BugRelations");bugRelation.HasKey(x=>x.Id);bugRelation.Property(x=>x.CreatedBy).HasMaxLength(200);bugRelation.HasIndex(x=>new{x.BugId,x.RelatedBugId}).IsUnique();bugRelation.HasOne<Bug>().WithMany().HasForeignKey(x=>x.RelatedBugId).OnDelete(DeleteBehavior.NoAction);
        var release=modelBuilder.Entity<ReleaseCandidate>();release.ToTable("Releases");release.HasKey(x=>x.Id);release.Property(x=>x.Name).HasMaxLength(250);release.Property(x=>x.ReleaseNotes).HasColumnType("nvarchar(max)");release.Property(x=>x.RollbackPlan).HasColumnType("nvarchar(max)");release.Property(x=>x.Status).HasConversion<string>().HasMaxLength(30);release.Property(x=>x.Decision).HasConversion<string>().HasMaxLength(30);release.Property(x=>x.SignOffBy).HasMaxLength(200);release.Property(x=>x.SignOffReason).HasMaxLength(2000);release.Property(x=>x.DeploymentStatus).HasConversion<string>().HasMaxLength(30);release.Property(x=>x.DeploymentNotes).HasMaxLength(4000);release.Property(x=>x.PostReleaseValidatedBy).HasMaxLength(200);release.HasIndex(x=>new{x.ProductId,x.TargetDate});release.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);release.HasOne<ProductBuild>().WithMany().HasForeignKey(x=>x.BuildId).OnDelete(DeleteBehavior.Restrict);release.HasOne<ProductEnvironment>().WithMany().HasForeignKey(x=>x.EnvironmentId).OnDelete(DeleteBehavior.Restrict);release.HasOne<TestCycle>().WithMany().HasForeignKey(x=>x.TestCycleId).OnDelete(DeleteBehavior.Restrict);release.HasMany(x=>x.Checklist).WithOne().HasForeignKey(x=>x.ReleaseId).OnDelete(DeleteBehavior.Cascade);release.HasMany(x=>x.Requirements).WithOne().HasForeignKey(x=>x.ReleaseId).OnDelete(DeleteBehavior.Cascade);release.HasMany(x=>x.KnownIssues).WithOne().HasForeignKey(x=>x.ReleaseId).OnDelete(DeleteBehavior.Cascade);
        var releaseCheck=modelBuilder.Entity<ReleaseChecklistItem>();releaseCheck.ToTable("ReleaseChecklistItems");releaseCheck.HasKey(x=>x.Id);releaseCheck.Property(x=>x.Code).HasMaxLength(50);releaseCheck.Property(x=>x.Label).HasMaxLength(250);releaseCheck.Property(x=>x.CompletedBy).HasMaxLength(200);releaseCheck.HasIndex(x=>new{x.ReleaseId,x.Code}).IsUnique();
        var releaseRequirement=modelBuilder.Entity<ReleaseRequirement>();releaseRequirement.ToTable("ReleaseRequirements");releaseRequirement.HasKey(x=>x.Id);releaseRequirement.HasIndex(x=>new{x.ReleaseId,x.RequirementId}).IsUnique();releaseRequirement.HasOne<Requirement>().WithMany().HasForeignKey(x=>x.RequirementId).OnDelete(DeleteBehavior.Restrict);
        var knownIssue=modelBuilder.Entity<ReleaseKnownIssue>();knownIssue.ToTable("ReleaseKnownIssues");knownIssue.HasKey(x=>x.Id);knownIssue.Property(x=>x.Mitigation).HasMaxLength(2000);knownIssue.HasIndex(x=>new{x.ReleaseId,x.BugId}).IsUnique();knownIssue.HasOne<Bug>().WithMany().HasForeignKey(x=>x.BugId).OnDelete(DeleteBehavior.Restrict);
        var scheduledReport=modelBuilder.Entity<ScheduledReport>();scheduledReport.ToTable("ScheduledReports");scheduledReport.HasKey(x=>x.Id);scheduledReport.Property(x=>x.Name).HasMaxLength(200).IsRequired();scheduledReport.Property(x=>x.Frequency).HasConversion<string>().HasMaxLength(20).IsRequired();scheduledReport.Property(x=>x.Recipients).HasMaxLength(2000).IsRequired();scheduledReport.HasIndex(x=>new{x.IsEnabled,x.NextRunAtUtc});scheduledReport.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
        var savedQuery=modelBuilder.Entity<SavedSqlQuery>();savedQuery.ToTable("SavedSqlQueries");savedQuery.HasKey(x=>x.Id);savedQuery.Property(x=>x.Name).HasMaxLength(200).IsRequired();savedQuery.Property(x=>x.Statement).HasMaxLength(8000).IsRequired();savedQuery.Property(x=>x.CreatedBy).HasMaxLength(200).IsRequired();savedQuery.HasIndex(x=>new{x.ProductId,x.Name});savedQuery.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
        var automationRun=modelBuilder.Entity<AutomationRun>();automationRun.ToTable("AutomationRuns");automationRun.HasKey(x=>x.Id);automationRun.Property(x=>x.Provider).HasMaxLength(50).IsRequired();automationRun.Property(x=>x.ExternalRunId).HasMaxLength(200).IsRequired();automationRun.Property(x=>x.Branch).HasMaxLength(250);automationRun.Property(x=>x.CommitSha).HasMaxLength(100);automationRun.Property(x=>x.Status).HasMaxLength(30).IsRequired();automationRun.Property(x=>x.Owner).HasMaxLength(200).IsRequired();automationRun.Property(x=>x.Fingerprint).HasMaxLength(64).IsRequired();automationRun.HasIndex(x=>new{x.Provider,x.ExternalRunId}).IsUnique();automationRun.HasIndex(x=>new{x.ProductId,x.ReceivedAtUtc});automationRun.HasOne<Product>().WithMany().HasForeignKey(x=>x.ProductId).OnDelete(DeleteBehavior.Restrict);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var events = ChangeTracker.Entries()
            .Where(IsAuditableChange)
            .Select(CreateAuditEvent)
            .ToArray();
        AuditEvents.AddRange(events);
        return await base.SaveChangesAsync(cancellationToken);
    }

    private static bool IsAuditableChange(EntityEntry entry) =>
        entry.State is EntityState.Added or EntityState.Modified &&
            entry.Entity is Product or ProductModule or ProductEnvironment or UserAccount or AppRole or UserRole or Requirement or RequirementComment or RequirementAttachment or TestCase or TestCaseVersion or TestCaseStep or TestCaseComment or ProductBuild or TestCycle or TestCycleItem or TestRunAttempt or TestRunEvidence or Bug or BugRunLink or BugStatusHistory or BugComment or BugEvidence or BugRelation or ReleaseCandidate or ReleaseChecklistItem or ReleaseRequirement or ReleaseKnownIssue or ScheduledReport or SavedSqlQuery or AutomationRun;

    private AuditEvent CreateAuditEvent(EntityEntry entry)
    {
        var context = httpContextAccessor?.HttpContext;
        var actorId = context?.User.Identity?.Name ?? "anonymous";
        var correlationId = context?.TraceIdentifier ?? "background";
        var entityId = entry.Entity is UserRole assignment
            ? assignment.UserId
            : (Guid)(entry.Property("Id").CurrentValue ?? Guid.Empty);
        Guid? productId = entry.Entity switch
        {
            Product product => product.Id,
            ProductModule module => module.ProductId,
            ProductEnvironment environment => environment.ProductId,
            Requirement requirement => requirement.ProductId,
            RequirementComment comment => Requirements.Where(x => x.Id == comment.RequirementId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
            RequirementAttachment attachment => Requirements.Where(x => x.Id == attachment.RequirementId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
            TestCase testCase => testCase.ProductId,
            TestCaseVersion version => TestCases.Where(x => x.Id == version.TestCaseId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
            TestCaseStep step => TestCaseVersions.Where(x => x.Id == step.TestCaseVersionId).Select(x => TestCases.Where(tc => tc.Id == x.TestCaseId).Select(tc => (Guid?)tc.ProductId).FirstOrDefault()).FirstOrDefault(),
            TestCaseComment comment => TestCases.Where(x => x.Id == comment.TestCaseId).Select(x => (Guid?)x.ProductId).FirstOrDefault(),
            ProductBuild build => build.ProductId,
            TestCycle cycle => cycle.ProductId,
            Bug bug => bug.ProductId,
            ReleaseCandidate release => release.ProductId,
            ScheduledReport report => report.ProductId,
            SavedSqlQuery query => query.ProductId,
            AutomationRun run => run.ProductId,
            _ => (Guid?)null,
        };
        var changes = entry.Properties
            .Where(property => entry.State == EntityState.Added || property.IsModified)
            .Where(property => property.Metadata.Name is not "RowVersion")
            .ToDictionary(
                property => property.Metadata.Name,
                property => new { before = entry.State == EntityState.Added ? null : property.OriginalValue, after = property.CurrentValue });

        return new AuditEvent(
            actorId,
            entry.State == EntityState.Added ? "created" : "updated",
            entry.Metadata.ClrType.Name,
            entityId,
            productId,
            correlationId,
            JsonSerializer.Serialize(changes));
    }
}
