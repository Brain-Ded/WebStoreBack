namespace Web_StoreAPI.DataModels
{
    public class VendorCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string? Adress { get; set; }     

        //Додати категорії та товари

        public ICollection<Item> Items { get;} = new List<Item>();
    }
}
