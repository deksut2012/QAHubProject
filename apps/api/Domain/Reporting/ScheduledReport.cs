namespace QAHub.Api.Domain.Reporting;

public sealed class ScheduledReport
{
    private ScheduledReport() { }

    public ScheduledReport(Guid? productId, string name, ReportFrequency frequency, TimeOnly deliveryTimeUtc, string recipients, DateTimeOffset now)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(recipients);
        Id = Guid.NewGuid(); ProductId = productId; Name = name.Trim(); Frequency = frequency;
        DeliveryTimeUtc = deliveryTimeUtc; Recipients = recipients.Trim(); IsEnabled = true;
        CreatedAtUtc = now; NextRunAtUtc = CalculateNextRun(frequency, deliveryTimeUtc, now);
    }

    public Guid Id { get; private set; }
    public Guid? ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ReportFrequency Frequency { get; private set; }
    public TimeOnly DeliveryTimeUtc { get; private set; }
    public string Recipients { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public DateTimeOffset NextRunAtUtc { get; private set; }
    public DateTimeOffset? LastRunAtUtc { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? UpdatedAtUtc { get; private set; }

    public void SetEnabled(bool enabled, DateTimeOffset now)
    {
        IsEnabled = enabled; UpdatedAtUtc = now;
        if (enabled) NextRunAtUtc = CalculateNextRun(Frequency, DeliveryTimeUtc, now);
    }

    public static DateTimeOffset CalculateNextRun(ReportFrequency frequency, TimeOnly time, DateTimeOffset now)
    {
        var candidate = new DateTimeOffset(now.Year, now.Month, now.Day, time.Hour, time.Minute, 0, TimeSpan.Zero);
        if (candidate <= now) candidate = candidate.AddDays(1);
        return frequency switch
        {
            ReportFrequency.Daily => candidate,
            ReportFrequency.Weekly => candidate.AddDays((7 - (int)candidate.DayOfWeek) % 7),
            ReportFrequency.Monthly => new DateTimeOffset(candidate.Year, candidate.Month, 1, time.Hour, time.Minute, 0, TimeSpan.Zero).AddMonths(1),
            _ => candidate
        };
    }
}
