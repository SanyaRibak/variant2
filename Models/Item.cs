using System.ComponentModel.DataAnnotations;

namespace MoiMarket.Models;

public class Item
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Введите название."), StringLength(80, ErrorMessage = "Не больше 80 символов.")]
    [Display(Name = "Название")]
    public string Title { get; set; } = "";
    [Required(ErrorMessage = "Введите описание."), StringLength(2000, ErrorMessage = "Не больше 2000 символов.")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = "";
    [Required(ErrorMessage = "Выберите категорию.")]
    [RegularExpression("^(вентиляторы|мультиварки|мантоварки)$", ErrorMessage = "Выберите категорию из списка.")]
    [Display(Name = "Категория")]
    public string Category { get; set; } = "";
    [Range(typeof(decimal), "0.01", "100000000", ParseLimitsInInvariantCulture = true, ErrorMessage = "Цена должна быть от 0,01 до 100 000 000 рублей.")]
    [Display(Name = "Цена, руб.")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Введите имя продавца."), StringLength(60, ErrorMessage = "Не больше 60 символов.")]
    [Display(Name = "Имя продавца")]
    public string Seller { get; set; } = "";
    [Required(ErrorMessage = "Введите телефон.")]
    [RegularExpression(@"\+?[0-9][0-9 ()-]{8,19}", ErrorMessage = "Введите телефон, например +7 900 123-45-67.")]
    [Display(Name = "Телефон")]
    public string Phone { get; set; } = "";
    public string ImagePath { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public bool IsPublished { get; set; } = true;
    public bool IsReserved { get; set; }
}
