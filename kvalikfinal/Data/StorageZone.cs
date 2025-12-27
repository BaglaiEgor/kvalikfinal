using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class StorageZone
{
    public int ZoneId { get; set; }

    public int WarehouseId { get; set; }

    public string ZoneCode { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<StockBalance> StockBalances { get; set; } = new List<StockBalance>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
