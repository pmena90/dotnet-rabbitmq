using Domain.Common;

namespace Domain.Entities
{
    public class TextMessage : ICustomMessage
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}