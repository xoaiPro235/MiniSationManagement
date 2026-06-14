namespace MiniStationeryManagement.Mvc.Models;

public class StationeryOrder
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public ICollection<StationeryOrderItem> OrderItems { get; set; } =
        new List<StationeryOrderItem>();
}
