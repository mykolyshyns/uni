using System;
using System.Collections.Generic;

namespace DALEF.Models;

public partial class TblProduct
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    public virtual ICollection<TblOrderProduct> TblOrderProducts { get; set; } = new List<TblOrderProduct>();
}
