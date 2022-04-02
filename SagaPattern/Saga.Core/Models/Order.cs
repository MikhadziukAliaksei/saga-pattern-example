using Saga.Core.Extensions;

namespace Saga.Core.Models;

public class Order : CommonInfo
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime? ActivationDate { get; set; }
    public int CompletionPercentage { get; set; }
    
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}