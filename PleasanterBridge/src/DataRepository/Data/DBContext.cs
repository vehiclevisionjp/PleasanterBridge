using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;

namespace PleasanterBridge.src.DataRepository.Data
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
