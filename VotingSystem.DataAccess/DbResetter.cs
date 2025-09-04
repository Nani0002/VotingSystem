using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Respawn;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess
{
    public class DbResetter
    {
        private readonly string _connectionString;
        private readonly VotingSystemDbContext _context;
        private readonly UserManager<User> _userManager;

        public DbResetter(IConfiguration config, VotingSystemDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        public async Task ResetAsync()
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                TablesToIgnore = ["__EFMigrationsHistory"],
            });
            await respawner.ResetAsync(_connectionString);

            DbInitializer.Initialize(_context, _userManager);
        }
    }
}
