using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class StockBalance
{
    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int ZoneId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual StorageZone Zone { get; set; } = null!;
}
