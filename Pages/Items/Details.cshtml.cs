using MoiMarket.Models;
using MoiMarket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoiMarket.Pages.Items;

public class DetailsModel(StoreService store) : PageModel
{
    public Item Item { get; set; } = null!;
    public bool IsOwner { get; set; }
    public IActionResult OnGet(int id)
    {
        var item = store.GetItem(id);
        if (item == null) return NotFound();
        IsOwner = item.OwnerId == (string)HttpContext.Items["Moi2Owner"]!;
        if (!item.IsPublished && !IsOwner) return NotFound();
        Item = item;
        return Page();
    }
}
