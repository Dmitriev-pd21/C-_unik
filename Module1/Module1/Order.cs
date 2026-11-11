
namespace DeliveryService
{
    class Order
    {
        public int Number { get; }
        public string Description { get; }
        public decimal Weight { get; }
        public decimal Distance { get; }

        public Order(int number, string description, decimal weight, decimal distance)
        {
            Number = number;
            Description = description;
            Weight = weight;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"№{Number} | {Description} | {Weight} кг | {Distance} км";
        }
    }
}
