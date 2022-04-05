using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Saga.Orchistrator.DTO
{
    public sealed class SagaStateInfo
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        
        public DateTimeOffset TimeStamp { get; set; }
        
        public string State { get; set; }
        
        public string? Info { get; set; }
    }
}