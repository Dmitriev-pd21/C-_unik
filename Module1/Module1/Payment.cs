
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
}
