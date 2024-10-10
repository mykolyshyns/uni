using System;
using System.Collections.Generic;

namespace DALEF.Models;

public partial class TblComment
{
    public int CommentId { get; set; }

    public string CommentText { get; set; } = null!;

    public DateTime CommentTime { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public virtual TblProduct Product { get; set; } = null!;

    public virtual TblUser User { get; set; } = null!;
}
