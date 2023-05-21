using DAOLib.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class RYCContextService
    {
        private readonly RYCContextManager contextManager;
        private static RYCContextService? _instance;
        private static readonly object _lock = new object();

        public static RYCContextService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new RYCContextService();
                    }
                }
                return _instance;
            }
        }

        private RYCContextService() 
        {
            contextManager = new();
        }

        public void migrateDataBase()
        {
            contextManager.migrateDataBase();
        }

        public void loadContext()
        {
            contextManager.loadContext();
        }

        public void makeBackup()
        {
            contextManager.makeBackup();
        }
    }
}
