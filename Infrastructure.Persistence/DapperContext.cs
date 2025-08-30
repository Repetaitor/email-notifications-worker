using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence;

public class DapperDbContext(IConfiguration configuration)
{
    private readonly string _connectionString = configuration["ConnectionStrings:ProductionConnection"]
                                                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' is not configured.");

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}