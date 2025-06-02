namespace kolokwium_1.Models
{
    public class Something
    {
        public int id { get; set; }
        public DateTime date { get; set; }
    }

    public class Trip
    {
        public int IdClient { get; set; }
        public int IdTrip { get; set; }
        public int RegisteredAt { get; set; }
        public int PaymentDate { get; set; }
    }
}