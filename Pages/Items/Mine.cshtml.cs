using MoiMarket.Models;
using MoiMarket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoiMarket.Pages.Items;

public class MineModel(StoreService store) : PageModel
{
    public List<Item> Items { get; set; } = [];
    public Dictionary<int, List<Order>> Orders { get; set; } = [];
    public void OnGet()
    {
        Items = store.GetItems().Where(x => x.OwnerId == (string)HttpContext.Items["Moi2Owner"]!).OrderByDescending(x => x.Id).ToList();
        Orders = Items.ToDictionary(x => x.Id, x => store.GetOrders(x.Id));
    }

    public IActionResult OnPostHide(int id) => Change(id, false);
    public IActionResult OnPostDelete(int id) => Change(id, true);

    private IActionResult Change(int id, bool delete)
    {
        if (!store.Manage(id, (string)HttpContext.Items["Moi2Owner"]!, delete)) return NotFound();
        TempData["Message"] = delete ? "Объявление удалено." : "Объявление снято с публикации.";
        return RedirectToPage();
    }
}
