
namespace DeliveryService
{
    abstract class Transport
    {
        public string Name { get; }
        public decimal MaxWeight { get; }
        public Order? CurrentOrder { get; private set; }
        public bool IsBusy => CurrentOrder != null;

        protected Transport(string name, decimal maxWeight)
        {
            Name = name;
            MaxWeight = maxWeight;
        }

        public abstract decimal CalculateCost(Order order);

        public virtual bool CanTake(Order order)
        {
            return order.Weight > 0 && order.Weight <= MaxWeight &&
                   order.Distance > 0;
        }

        public bool TakeOrder(Order order)
        {
            if (IsBusy || !CanTake(order)) return false;

            CurrentOrder = order;
            return true;
        }

        public void Release()
        {
            CurrentOrder = null;
        }
    }

    class Bike : Transport
    {
        public Bike(decimal weight) : base("Велосипед", weight) { }

        public override decimal CalculateCost(Order order)
        {
            return 20 + order.Distance * 5;
        }

        public override bool CanTake(Order order)
        {
            return base.CanTake(order) && order.Distance <= 10;
        }
    }

    class Car : Transport
    {
        public Car(decimal weight) : base("Автомобіль", weight) { }

        public override decimal CalculateCost(Order order)
        {
            return 50 + order.Distance * 8 + order.Weight;
        }
    }

    class Truck : Transport
    {
        public Truck(decimal weight) : base("Вантажівка", weight) { }

        public override decimal CalculateCost(Order order)
        {
            return 200 + order.Distance * 15 + order.Weight * 0.5m;
        }
    }
}
