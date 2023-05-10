using DAOLib.Managers;
using Microsoft.EntityFrameworkCore;

namespace DAOLib.Services
{
    public sealed class RYCContextServiceDAO
    {
        private readonly static RYCContextServiceDAO _instance = new RYCContextServiceDAO();
        private readonly RYCContext context = new RYCContext();

        private RYCContextServiceDAO()
        {
            loadContext();
        }

        private void loadContext()
        {
            context.dateCalendar?.Load();
            context.categoriesTypes?.Load();
            context.categories?.Load();
            context.persons?.Load();
            context.accountsTypes?.Load();
            context.accounts?.Load();
            context.transactionsStatus?.Load();
            context.tags?.Load();
            context.investmentProducts?.Load();
            context.investmentProductsPrices?.Load();
            context.splits?.Load();
            context.transactions?.Load();
            context.periodsReminders?.Load();
            context.splitsReminders?.Load();
            context.transactionsReminders?.Load();
            context.expirationsReminders?.Load();
            context.vBalancebyCategory?.Load();
        }

        public RYCContext BBDD { get { return context; } }

        public static RYCContextServiceDAO getInstance()
        {
            return _instance;
        }
    }
}
