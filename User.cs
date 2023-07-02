using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Web_StoreAPI.DataModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }

        public ICollection<Receipt> Receipt { get; } = new List<Receipt>();

        public int UserTypeId { get; set; }

        [JsonIgnore]
        public UserType UserType { get; set; } = null!;

        [JsonIgnore]
        public List<Item>? cartItems { get; set; } = new List<Item>();
    }
}
