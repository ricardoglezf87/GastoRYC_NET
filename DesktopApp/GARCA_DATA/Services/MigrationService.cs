using GARCA.Data.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Data.Services
{
    public class MigrationService
    {
        private readonly MigrationManager migrationManager;

        public MigrationService() 
        {
            migrationManager = new();
        }

        public async Task Migrate() 
        { 
            await migrationManager.Migrate(); 
        }
    }
}
