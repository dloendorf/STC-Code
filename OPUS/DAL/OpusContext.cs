using OPUS.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace OPUS.DAL
{
    public class OpusContext : DbContext
    {

        public OpusContext() : base("OpusContext")
        {
        }

        public DbSet<OpusPlayer> OpusPlayers { get; set; }
        public DbSet<Scoring> Scores { get; set; }
        public DbSet<CourtAssignment> Assignments { get; set; }
        public DbSet<Settings> OpusSettings { get; set; }
        public DbSet<OPUS.Models.PastCourtAssignment> PastCourtAssignments { get; set; }
        public DbSet<OPUS.Models.PastScoring> PastScorings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public System.Data.Entity.DbSet<OPUS.Models.PastOpusPlayer> PastOpusPlayers { get; set; }

        public System.Data.Entity.DbSet<OPUS.Models.STCPlayers> STCPlayers { get; set; }
    }
}