﻿namespace Web_StoreAPI.DataModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Item> Items { get; } = new List<Item>();
    }
}
