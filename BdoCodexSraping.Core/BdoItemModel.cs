namespace BdoCodexSraping.Core
{
    public class BdoItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public string Category { get; set; }
        public ItemGrade Grade { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public string Knowledge { get; set; }
        public string SellPrice { get; set; }
        public string BuyPrice { get; set; }
    }
}