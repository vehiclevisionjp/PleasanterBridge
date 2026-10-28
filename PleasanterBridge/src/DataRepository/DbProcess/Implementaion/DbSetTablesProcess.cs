using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;
using PleasanterBridge.src.DataRepository.Data;
using PleasanterBridge.src.DataRepository.Entity;

namespace PleasanterBridge.src.DataRepository.DbProcess.Implementaion
{
    internal class DbSetTablesProcess : DBContext
    {
        public DbSet<ResultsEntity> Results { get; set; }
        public DbSet<IssuesEntity> Issues { get; set; }
        public DbSet<WikisEntity> Wikis { get; set; }
        public DbSet<ItemsEntity> Items { get; set; }

        public DbSetTablesProcess(IConnectionSettings settings) : base(settings) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResultsEntity>().ToTable("Results");
            modelBuilder.Entity<IssuesEntity>().ToTable("Issues");
            modelBuilder.Entity<WikisEntity>().ToTable("Wikis");
            modelBuilder.Entity<ItemsEntity>().ToTable("Items");

            base.OnModelCreating(modelBuilder);
        }
    }
}
