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

        public readonly static AccountsService accountsService = new AccountsService();
        public readonly static CategoriesService categoriesService = new CategoriesService();
        public readonly static PersonsService personsService = new PersonsService();
        public readonly static TransactionsService transactionsService = new TransactionsService();
        public readonly static SplitsService splitsService = new SplitsService();
        public readonly static TransactionsRemindersService transactionsRemindersService = new TransactionsRemindersService();
        public readonly static ExpirationsRemindersService expirationsRemindersService = new ExpirationsRemindersService();

        private readonly static RYCContextService _instance = new RYCContextService();
        private readonly RYCContext context = new RYCContext();

        private RYCContextService()
        {
            loadContext();
        }

        private void loadContext()
        {
            context.categoriesTypes?.Load();
            context.categories?.Load();
            context.persons?.Load();
            context.accountsTypes?.Load();
            context.accounts?.Load();
            context.transactionsStatus?.Load();
            context.tags?.Load();
            context.splits?.Load();
            context.transactions?.Load();
            context.periodsReminders?.Load();
            context.splitsReminders?.Load();
            context.transactionsReminders?.Load();
            context.expirationsReminders?.Load();
        }

        public RYCContext BBDD { get { return context; } }

        public static RYCContextService getInstance() {
                return _instance;           
        }
    }
}
