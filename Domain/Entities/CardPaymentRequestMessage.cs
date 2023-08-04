using Domain.Common;

namespace Domain.Entities
{
    public class CardPaymentRequestMessage : ICustomMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public string ExpiryDate { get; set; }
        public decimal Amount { get; set; }
    }
}