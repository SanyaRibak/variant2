using MoiMarket.Models;
using MoiMarket.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MoiMarket.Pages.Items;

[RequestSizeLimit(3 * 1024 * 1024)]
[RequestFormLimits(MultipartBodyLengthLimit = 3 * 1024 * 1024)]
public class CreateModel(StoreService store, IWebHostEnvironment environment) : PageModel
{
    [BindProperty] public Item Input { get; set; } = new();
    [BindProperty, Required(ErrorMessage = "Выберите изображение.")]
    [Display(Name = "Фото (JPEG или PNG, до 2 МБ)")]
    public IFormFile? Photo { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        string? extension = null;
        if (Photo != null)
        {
            if (Photo.Length == 0 || Photo.Length > 2 * 1024 * 1024)
                ModelState.AddModelError("Photo", "Выберите непустой файл размером до 2 МБ.");
            else
            {
                var header = new byte[8];
                await using var stream = Photo.OpenReadStream();
                var count = await stream.ReadAtLeastAsync(header, 8, false);
                var declared = Path.GetExtension(Photo.FileName).ToLowerInvariant();
                if (count >= 8 && header.SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) && declared == ".png") extension = ".png";
                if (count >= 3 && header[0] == 255 && header[1] == 216 && header[2] == 255 && declared is ".jpg" or ".jpeg") extension = ".jpg";
                if (extension == null) ModelState.AddModelError("Photo", "Допускаются только изображения JPEG и PNG.");
            }
        }
        if (!ModelState.IsValid) return Page();
        var folder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(folder);
        var fileName = Guid.NewGuid() + extension;
        var path = Path.Combine(folder, fileName);
        await using (var output = System.IO.File.Create(path)) await Photo!.CopyToAsync(output);
        var item = new Item
        {
            Title = Input.Title.Trim(), Description = Input.Description.Trim(), Category = Input.Category,
            Price = Input.Price, Seller = Input.Seller.Trim(), Phone = Input.Phone.Trim(),
            ImagePath = "/uploads/" + fileName, OwnerId = (string)HttpContext.Items["Moi2Owner"]!
        };
        try
        {
            var id = store.Add(item);
            TempData["Message"] = "Объявление опубликовано.";
            return RedirectToPage("Details", new { id });
        }
        catch
        {
            System.IO.File.Delete(path);
            throw;
        }
    }
}
