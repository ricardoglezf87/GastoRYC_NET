using BBDDLib.Manager;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public sealed class RYCContextService
    {
        private readonly static RYCContextService _instance = new RYCContextService();
        private readonly RYCContext context = new RYCContext();

        private RYCContextService()
        {
            loadContext();
        }

        private void loadContext()
        {        
            context.categories?.Load();
            context.persons?.Load();
            context.accountsTypes?.Load();
            context.accounts?.Load();
            context.transactionsStatus?.Load();
            context.transactions?.Load();
        }

        public RYCContext BBDD { get { return context; } }

        public static RYCContextService getInstance() {
                return _instance;           
        }
    }
}
