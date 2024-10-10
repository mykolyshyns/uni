using System;
using System.Collections.Generic;

namespace DALEF.Models;

public partial class TblOrderProduct
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public virtual TblOrder Order { get; set; } = null!;

    public virtual TblProduct Product { get; set; } = null!;
}
