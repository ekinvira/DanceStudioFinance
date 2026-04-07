using System;

namespace VRFinanceApp.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        public string ItemName { get; set; } = "";

        public string Category { get; set; } = "";

        public int Quantity { get; set; }

        public string Unit { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Today;

        public string? Note { get; set; }

        public decimal TotalValue => Quantity * UnitPrice;
    }
}