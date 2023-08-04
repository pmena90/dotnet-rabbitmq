using EasyNetQ;

namespace Domain.Common
{
    [Queue("CardPaymentQueue", ExchangeName = "CardPaymentExchange")]
    public interface ICustomMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}