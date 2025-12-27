using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class DeliveryItem
{
    public int DeliveryItemId { get; set; }

    public int DeliveryId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal PurchasePrice { get; set; }

    public virtual Delivery Delivery { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
