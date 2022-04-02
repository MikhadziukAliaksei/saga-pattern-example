using Saga.Core.Extensions;

namespace Saga.Core.Models;

public class TrackSubmission : CommonInfo
{
    public Guid TrackSubmissionId { get; set; }
    public string Url { get; set; }
    public double Budget { get; set; }
    public double Budget2 { get; set; }
    public string? Notes { get; set; }
    public TrackSubmissionStatus Status { get; set; }
    public string Feedback { get; set; }
    
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}