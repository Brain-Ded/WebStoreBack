using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web_StoreAPI.DataModels
{
    public class Item
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageURL { get; set; }

        public double ActualPrice { get; set; }

        public double DiscountPrice { get; set; }

        public string Tags { get; set; }

        public string? Description { get; set; }

        public bool Status { get; set; }

        public int Advertise { get; set; } = 0;

        public int Views { get; set; }

        public int? ReceiptId { get; set; }
        [JsonIgnore]
        public Receipt? Receipt { get; set; }
        
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; } = null!;

        public int VendorCompanyId { get; set; }
        [JsonIgnore]
        public VendorCompany VendorCompany { get; set; } = null!;

    }
}
