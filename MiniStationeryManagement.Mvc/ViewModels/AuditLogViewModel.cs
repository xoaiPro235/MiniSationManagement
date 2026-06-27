namespace MiniStationeryManagement.Mvc.ViewModels;

public class AuditLogViewModel
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
}
