
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryService
{
    interface IPaymentMethod
    {
        void Pay(decimal amount);
    }

    class CardPayment : IPaymentMethod
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Оплата карткою: {amount} грн");
        }
    }

    class CashPayment : IPaymentMethod
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Оплата готівкою: {amount} грн");
        }
    }

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

    class DeliveryService
    {
        private List<Transport> transports = new();
        private List<Order> activeOrders = new();
        private List<Order> history = new();

        public void AddTransport(Transport transport)
        {
            transports.Add(transport);
        }

        public void AddOrder(Order order)
        {
            if (order.Number <= 0 || string.IsNullOrWhiteSpace(order.Description) ||
                order.Weight <= 0 || order.Distance <= 0 ||
                activeOrders.Exists(o => o.Number == order.Number) ||
                history.Exists(o => o.Number == order.Number))
            {
                Console.WriteLine("Некоректне або повторне замовлення!");
                return;
            }

            activeOrders.Add(order);
            Console.WriteLine($"Створено: {order}");
        }

        public void ShowCosts(Order order)
        {
            Console.WriteLine($"\nВартість замовлення №{order.Number}:");

            foreach (Transport t in transports)
                Console.WriteLine(t.CanTake(order)
                    ? $"{t.Name}: {t.CalculateCost(order)} грн"
                    : $"{t.Name}: не може доставити");
        }

        public void AssignOrder(Order order, Transport transport)
        {
            if (!activeOrders.Contains(order) ||
                !transports.Contains(transport) ||
                transports.Exists(t => t.CurrentOrder == order) ||
                !transport.TakeOrder(order))
            {
                Console.WriteLine($"Не вдалося призначити транспорт для №{order.Number}!");
                return;
            }

            Console.WriteLine($"Замовлення №{order.Number} призначено: {transport.Name}");
        }

        public void DeliverOrder(Order order, IPaymentMethod payment)
        {
            Transport? transport = transports.Find(t => t.CurrentOrder == order);

            if (!activeOrders.Contains(order) || transport == null)
            {
                Console.WriteLine($"Замовлення №{order.Number} не готове до доставки!");
                return;
            }

            decimal cost = transport.CalculateCost(order);

            Console.WriteLine($"\nДоставка №{order.Number}: {transport.Name}");
            payment.Pay(cost);

            transport.Release();
            activeOrders.Remove(order);
            history.Add(order);

            Console.WriteLine("Доставку завершено, транспорт звільнено.");
        }

        public void ShowActiveOrders()
        {
            Console.WriteLine("\nАктивні замовлення:");

            foreach (Order order in activeOrders)
            {
                Console.WriteLine(order);
                Transport? t = transports.Find(x => x.CurrentOrder == order);
                Console.WriteLine(t == null ? "Транспорт не призначений" : t.Name);
            }

            if (activeOrders.Count == 0)
                Console.WriteLine("Немає активних замовлень.");
        }

        public void ShowHistory()
        {
            Console.WriteLine("\nІсторія доставок:");

            foreach (Order order in history)
                Console.WriteLine(order);

            if (history.Count == 0)
                Console.WriteLine("Історія порожня.");
        }

        public void ShowTransports()
        {
            Console.WriteLine("\nСтан транспорту:");

            foreach (Transport t in transports)
                Console.WriteLine($"{t.Name}: {(t.IsBusy ? "Зайнятий" : "Вільний")}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            DeliveryService service = new DeliveryService();

            Bike bike = new Bike(20);
            Car car = new Car(500);
            Truck truck = new Truck(5000);

            service.AddTransport(bike);
            service.AddTransport(car);
            service.AddTransport(truck);

            Order o1 = new Order(1, "Документи", 2, 5);
            Order o2 = new Order(2, "Побутова техніка", 100, 15);
            Order o3 = new Order(3, "Будівельні матеріали", 1000, 30);

            service.AddOrder(o1);
            service.AddOrder(o2);
            service.AddOrder(o3);

            service.ShowCosts(o1);
            service.ShowCosts(o2);
            service.ShowCosts(o3);

            service.AssignOrder(o1, bike);
            service.AssignOrder(o2, car);
            service.AssignOrder(o3, truck);

            service.ShowActiveOrders();
            service.ShowTransports();

            service.DeliverOrder(o1, new CardPayment());
            service.DeliverOrder(o2, new CashPayment());
            service.DeliverOrder(o3, new CardPayment());

            service.ShowActiveOrders();
            service.ShowHistory();
            service.ShowTransports();

            Console.WriteLine("\nПрограму завершено.");
        }
    }
}
