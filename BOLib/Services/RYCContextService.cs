using DAOLib.Managers;
using Microsoft.EntityFrameworkCore;

namespace BOLib.Services
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
            context.dateCalendar?.Load();
            context.investmentProducts?.Load();
            context.investmentProductsPrices?.Load();
            context.splitsReminders?.Load();
            context.vBalancebyCategory?.Load();
        }

        public RYCContext BBDD { get { return context; } }

        public static RYCContextService getInstance()
        {
            return _instance;
        }
    }
}
