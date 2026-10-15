using System.Text.Json;
using System.Text.Encodings.Web;
using MoiMarket.Models;

namespace MoiMarket.Services;

public class StoreService
{
    private readonly string itemsPath;
    private readonly string ordersPath;
    private readonly object sync = new();
    private readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public StoreService(IWebHostEnvironment environment)
    {
        var folder = Path.Combine(environment.ContentRootPath, "Data");
        Directory.CreateDirectory(folder);
        itemsPath = Path.Combine(folder, "items.json");
        ordersPath = Path.Combine(folder, "orders.json");
        if (!File.Exists(itemsPath)) File.WriteAllText(itemsPath, "[]");
        if (!File.Exists(ordersPath)) File.WriteAllText(ordersPath, "[]");
    }

    private List<T> Read<T>(string path) =>
        JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path)) ?? [];

    private void Write<T>(string path, List<T> data)
    {
        File.WriteAllText(path + ".tmp", JsonSerializer.Serialize(data, options));
        File.Move(path + ".tmp", path, true);
    }

    public List<Item> GetItems()
    {
        lock (sync) return Read<Item>(itemsPath);
    }

    public Item? GetItem(int id) => GetItems().FirstOrDefault(x => x.Id == id);

    public int Add(Item item)
    {
        lock (sync)
        {
            var items = Read<Item>(itemsPath);
            var orders = Read<Order>(ordersPath);
            item.Id = Math.Max(items.Select(x => x.Id).DefaultIfEmpty().Max(),
                orders.Select(x => x.ItemId).DefaultIfEmpty().Max()) + 1;
            items.Add(item);
            Write(itemsPath, items);
            return item.Id;
        }
    }

    public bool Order(int id, Order order)
    {
        lock (sync)
        {
            var items = Read<Item>(itemsPath);
            var item = items.FirstOrDefault(x => x.Id == id && x.IsPublished && !x.IsReserved);
            if (item == null) return false;
            var orders = Read<Order>(ordersPath);
            order.Id = orders.Select(x => x.Id).DefaultIfEmpty().Max() + 1;
            order.ItemId = id;
            order.CreatedAt = DateTime.Now;
            orders.Add(order);
            item.IsReserved = true;
            Write(itemsPath, items);
            try { Write(ordersPath, orders); }
            catch
            {
                item.IsReserved = false;
                Write(itemsPath, items);
                throw;
            }
            return true;
        }
    }

    public bool Manage(int id, string owner, bool delete)
    {
        lock (sync)
        {
            var items = Read<Item>(itemsPath);
            var item = items.FirstOrDefault(x => x.Id == id && x.OwnerId == owner);
            if (item == null) return false;
            if (delete) items.Remove(item);
            else item.IsPublished = false;
            Write(itemsPath, items);
            return true;
        }
    }

    public List<Order> GetOrders(int itemId)
    {
        lock (sync) return Read<Order>(ordersPath).Where(x => x.ItemId == itemId).ToList();
    }
}
