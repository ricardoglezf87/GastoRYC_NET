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
        public RYCContextService() 
        {
            contextManager = InstanceBase<RYCContextManager>.Instance;
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
