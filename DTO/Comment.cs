namespace DTO
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public DateTime CommentTime {  get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

    }
}
