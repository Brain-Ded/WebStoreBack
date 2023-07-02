namespace Web_StoreAPI.DataModels
{
    public class UserType
    {
        public int Id { get; set; }
        public string Type { get; set; }


        public List<User> Users { get; set; } = new List<User>();
    }
}
