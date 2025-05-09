
using Microsoft.EntityFrameworkCore;


namespace PatientTrackingSite.Models

{
    public class PTSDBContext : DbContext

    {

        public PTSDBContext(DbContextOptions<PTSDBContext> options) : base(options)
        {

        }


        public DbSet<Deneme> Deneme { get; set; }
    }
}
