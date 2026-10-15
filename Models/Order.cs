using System.ComponentModel.DataAnnotations;

namespace MoiMarket.Models;

public class Order
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    [Required(ErrorMessage = "Введите имя."), StringLength(60, ErrorMessage = "Не больше 60 символов.")]
    [Display(Name = "Ваше имя")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "Введите телефон.")]
    [RegularExpression(@"\+?[0-9][0-9 ()-]{8,19}", ErrorMessage = "Введите телефон, например +7 900 123-45-67.")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = "";
    [StringLength(500, ErrorMessage = "Не больше 500 символов.")]
    [Display(Name = "Комментарий")]
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
