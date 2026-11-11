
using System;
using System.Text;

namespace DeliveryService
{
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

