namespace MiniStationeryManagement.Mvc.ViewModels;

public class StationerySearchViewModel
{
    public string Keyword { get; set; } = "";
    public decimal? MinPrice { get; set; }
    public string Supplier { get; set; } = "";
    public List<StationeryListItemViewModel> StationeryItems { get; set; } = new();
}
