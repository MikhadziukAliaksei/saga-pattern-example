using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Saga.Core.Models;

namespace Saga.Submission.DataAccess.Contextes
{
    public class SubmissionDbContext : DbContext
    {
        public IConfiguration Configuration { get; }

        public DbSet<TrackSubmission> TrackSubmissions { get; set; }
        
        public SubmissionDbContext(DbContextOptions<SubmissionDbContext> options) : base(options)
        {

        }
        
    }
}
