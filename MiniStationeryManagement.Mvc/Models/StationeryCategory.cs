namespace MiniStationeryManagement.Mvc.Models;

public class StationeryCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<StationeryItem> StationeryItems { get; set; } = new List<StationeryItem>();
}
