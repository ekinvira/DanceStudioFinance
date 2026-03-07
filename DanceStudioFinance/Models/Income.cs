using System;

namespace DanceStudioFinance.Models
{
    public class Income
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        // K-pop, Serbest Stil vs.
        public string Branch { get; set; } = "";

        // Yetişkin, Çocuk vs.
        public string ClassType { get; set; } = "";

        public decimal Amount { get; set; }

        public string? Note { get; set; }
    }
}