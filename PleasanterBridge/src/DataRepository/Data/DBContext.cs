using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;

namespace PleasanterBridge.src.DataRepository.Data
{
    public class DBContext : DbContext
    {
        private readonly string ConnectionString;

        public DBContext(IConnectionSettings settings): base()
        {
            ConnectionString = 
                @"Integrated Security=False" +
                ";TrustServerCertificate=True" +
                ";Server=" + settings.Server +
                ";Database=" + settings.Database +
                ";User Id=" + settings.UserId +
                ";Password=" + settings.Password +
                ";";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }
}