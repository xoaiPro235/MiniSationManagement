namespace MiniStationeryManagement.Mvc.Models;

public class StationeryOrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public StationeryOrder? Order { get; set; }
    public int StationeryItemId { get; set; }
    public StationeryItem? StationeryItem { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
