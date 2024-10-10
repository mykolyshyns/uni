using System;
using System.Collections.Generic;

namespace DALEF.Models;

public partial class TblUser
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<TblComment> TblComments { get; set; } = new List<TblComment>();

    public virtual ICollection<TblOrder> TblOrders { get; set; } = new List<TblOrder>();
}
