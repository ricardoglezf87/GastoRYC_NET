using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class VPortfolioService
    {
        private readonly VPortfolioManager portfolioManager;
        private static VPortfolioService? _instance;
        private static readonly object _lock = new();

        public static VPortfolioService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new VPortfolioService();
                    }
                }
                return _instance;
            }
        }

        private VPortfolioService()
        {
            portfolioManager = new();
        }

        public async Task<List<VPortfolio?>?> getAllAsync()
        {
            return await Task.Run(() => portfolioManager.getAll()?.toListBO());
        }       
    }
}
