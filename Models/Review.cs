namespace GiftLab.Models
{
    public class Review
    {
        public string Name { get; set; } = "";
        public string Content { get; set; } = "";
        public int Stars { get; set; } = 5; // 1..5
    }
}
