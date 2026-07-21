using QAHub.Api.Domain.Reporting;

namespace QAHub.Api.Features.Reporting;

public sealed record CreateScheduledReportRequest(Guid? ProductId, string Name, ReportFrequency Frequency, TimeOnly DeliveryTimeUtc, string Recipients);
public sealed record SetScheduledReportEnabledRequest(bool IsEnabled);
public sealed record ScheduledReportResponse(Guid Id, Guid? ProductId, string Name, ReportFrequency Frequency, TimeOnly DeliveryTimeUtc, string Recipients, bool IsEnabled, DateTimeOffset NextRunAtUtc, DateTimeOffset? LastRunAtUtc);
