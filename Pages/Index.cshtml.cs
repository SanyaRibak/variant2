using MoiMarket.Models;
using MoiMarket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoiMarket.Pages;

public class IndexModel(StoreService store) : PageModel
{
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string? CategoryName { get; set; }
    public List<Item> Items { get; set; } = [];

    public void OnGet()
    {
        var items = store.GetItems().Where(x => x.IsPublished);
        if (!string.IsNullOrWhiteSpace(CategoryName)) items = items.Where(x => x.Category == CategoryName);
        if (!string.IsNullOrWhiteSpace(Search))
            items = items.Where(x => x.Title.Contains(Search.Trim(), StringComparison.OrdinalIgnoreCase));
        Items = items.OrderByDescending(x => x.Id).ToList();
    }
}
