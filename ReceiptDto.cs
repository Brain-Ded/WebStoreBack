namespace Web_StoreAPI.Dtos
{
    public class ReceiptDto
    {
        public int UserId { get; set; }
        public List<string> itemsNames { get; set; } = new List<string>();
    }
}
