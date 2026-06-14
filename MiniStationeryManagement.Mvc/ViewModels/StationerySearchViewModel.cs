namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationerySearchViewModel
{
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public List<StationeryListItemViewModel> StationeryItems { get; set; } = new();
}
