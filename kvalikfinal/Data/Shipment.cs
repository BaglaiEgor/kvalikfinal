using System;
using System.Collections.Generic;

namespace kvalikfinal.Data;

public partial class Shipment
{
    public int ShipmentId { get; set; }

    public int WarehouseId { get; set; }

    public DateOnly ShipmentDate { get; set; }

    public string Recipient { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
