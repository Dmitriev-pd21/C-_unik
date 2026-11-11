
using System;
using System.Collections.Generic;

namespace DeliveryService
{
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
}
