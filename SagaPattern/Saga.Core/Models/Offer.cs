using Newtonsoft.Json;
using Saga.Core.Extensions;

namespace Saga.Core.Models;

public class Offer : CommonInfo
{
    public Guid OfferId { get; set; }
    public Guid TrackSubmissionId { get; set; }
    public int Streams { get; set; }
    public double Price { get; set; }
    public int TimeEstimate { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime CreationDate { get; set; }

    [JsonIgnore]
    public TrackSubmission TrackSubmission { get; set; }
    
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}