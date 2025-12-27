using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class Delivery
{
    public int DeliveryId { get; set; }

    public int SupplierId { get; set; }

    public int WarehouseId { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<DeliveryItem> DeliveryItems { get; set; } = new List<DeliveryItem>();

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
