using Microsoft.EntityFrameworkCore;
using PleasanterAPIBridge.src.PleasanterRepository.Configuration;

namespace PleasanterAPIBridge.src.PleasanterRepository.Data.Implementation
{
    public class DBContext : DbContext, IDBContext
    {
        private readonly string connectionString;
        private readonly IConnectionSettings _settings;

        public DBContext(IConnectionSettings settings)
        {
            connectionString = @"Integrated Security=False" +
                ";TrustServerCertificate=True" +
                ";Server=" + settings.Server +
                ";Database=" + settings.Database +
                ";User Id=" + settings.UserId +
                ";Password=" + settings.Password +
                ";";
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}
