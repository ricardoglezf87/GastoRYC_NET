using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYCLib.Manager
{
    public class RYCContextFactory
    {
        private static volatile RYCContextFactory _dbContextFactory;
        private static readonly object SyncRoot = new Object();
        public RYCContext Context;


        public static RYCContextFactory Instance
        {
            get
            {
                if (_dbContextFactory == null)
                {
                    //lock (SyncRoot)
                    //{
                        if (_dbContextFactory == null)
                            _dbContextFactory = new RYCContextFactory();
                    //}
                }
                return _dbContextFactory;
            }
        }

        public RYCContext GetOrCreateContext()
        {
            if (this.Context == null)
            {
                this.Context = new RYCContext();
                this.Context.accountsTypes?.Load();
                this.Context.categories?.Load();
                this.Context.accounts?.Load();
                this.Context.persons?.Load();
                this.Context.transactions?.Load();
            }

            return Context;
        }
    }
}
