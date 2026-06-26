namespace MiniStationeryManagement.Mvc.ViewModels;

public class CategoryRelationshipViewModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string ProductsList { get; set; } = string.Empty;
    public string RelationshipType { get; set; } = "1 - Many";
    public string DbSetName { get; set; } = "StationeryCategories";
}
