using System.Text.Json.Serialization;

namespace Web_StoreAPI.DataModels
{
    public class Receipt
    {
        public int Id { get; set; }
        public double Sum { get; set; }
        public DateTime Date { get; set; }
        public int ItemsAmount { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = null!;

        public ICollection<Item> Items { get; } = new List<Item>();
    }
}
