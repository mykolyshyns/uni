using System;
using System.Collections.Generic;

namespace DALEF.Models;

public partial class TblOrder
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<TblOrderProduct> TblOrderProducts { get; set; } = new List<TblOrderProduct>();

    public virtual TblUser User { get; set; } = null!;
}
