namespace GiftLab.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string ImagePath { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int SoldCount { get; set; }
    }

}
