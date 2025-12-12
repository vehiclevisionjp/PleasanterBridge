using Microsoft.EntityFrameworkCore;
using PleasanterBridge.src.Configuration;

namespace PleasanterBridge.src.DataRepository.Data
{
    public class DBContext : DbContext
    {
        private readonly IConnectionSettings _settings;

        public DBContext(IConnectionSettings settings): base()
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

            switch (_settings.DatabaseType)
            {
                case DatabaseType.SqlServer:
                    ConfigureSqlServer(optionsBuilder);
                    break;

                case DatabaseType.MySQL:
                    ConfigureMySQL(optionsBuilder);
                    break;

                case DatabaseType.PostgreSQL:
                    ConfigurePostgreSQL(optionsBuilder);
                    break;

                default:
                    throw new NotSupportedException($"Database type '{_settings.DatabaseType}' is not supported.");
            }
        }

        private void ConfigureSqlServer(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = 
                @"Integrated Security=False" +
                ";TrustServerCertificate=True" +
                ";Server=" + _settings.Server +
                ";Database=" + _settings.Database +
                ";User Id=" + _settings.UserId +
                ";Password=" + _settings.Password +
                ";";

            optionsBuilder.UseSqlServer(connectionString);
        }

        private void ConfigureMySQL(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = 
                "Server=" + _settings.Server +
                ";Database=" + _settings.Database +
                ";Uid=" + _settings.UserId +
                ";Pwd=" + _settings.Password +
                ";";

            optionsBuilder.UseMySQL(connectionString);
        }

        private void ConfigurePostgreSQL(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = 
                "Host=" + _settings.Server +
                ";Database=" + _settings.Database +
                ";Username=" + _settings.UserId +
                ";Password=" + _settings.Password +
                ";";

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}