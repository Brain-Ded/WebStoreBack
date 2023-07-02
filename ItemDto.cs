namespace Web_StoreAPI.Dtos
{
    public class ItemDto
    {
        public string Name { get; set; }

        public string ImageURL { get; set; }

        public double ActualPrice { get; set; }

        public double DiscountPrice { get; set; }

        public string Description { get; set; }

        public int Amount { get; set; }

        public string Tags { get; set; }

        public int VendorCompanyId { get; set; }

        public int CategoryId { get; set; }
    }
}
