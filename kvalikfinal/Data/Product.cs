using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public string Article { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public int? MinStock { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    public virtual ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();
}
