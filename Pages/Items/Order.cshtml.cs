using MoiMarket.Models;
using MoiMarket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoiMarket.Pages.Items;

public class OrderModel(StoreService store) : PageModel
{
    [BindProperty] public Order Input { get; set; } = new();
    public Item Item { get; set; } = null!;

    public IActionResult OnGet(int id)
    {
        var item = store.GetItem(id);
        if (item == null || !item.IsPublished) return NotFound();
        if (item.IsReserved) return RedirectToPage("Details", new { id });
        Item = item;
        return Page();
    }

    public IActionResult OnPost(int id)
    {
        var item = store.GetItem(id);
        if (item == null || !item.IsPublished) return NotFound();
        Item = item;
        if (!ModelState.IsValid) return Page();
        var order = new Order { Name = Input.Name.Trim(), Phone = Input.Phone.Trim(), Comment = Input.Comment?.Trim() };
        if (!store.Order(id, order))
        {
            ModelState.AddModelError("", "Товар уже забронирован или снят с публикации.");
            return Page();
        }
        TempData["Message"] = "Заказ сохранён. Товар забронирован, продавец увидит ваши контакты.";
        return RedirectToPage("Details", new { id });
    }
}
