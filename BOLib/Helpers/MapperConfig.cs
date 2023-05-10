using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOLib.Helpers
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BOLib.Models.Accounts, DAOLib.Models.Accounts>().ReverseMap();                
                cfg.CreateMap<BOLib.Models.AccountsTypes, DAOLib.Models.AccountsTypes>().ReverseMap();                
                cfg.CreateMap<BOLib.Models.Categories, DAOLib.Models.Categories>().ReverseMap();                
                cfg.CreateMap<BOLib.Models.CategoriesTypes, DAOLib.Models.CategoriesTypes>().ReverseMap();
                cfg.CreateMap<BOLib.Models.DateCalendar, DAOLib.Models.DateCalendar>().ReverseMap();
                cfg.CreateMap<BOLib.Models.ExpirationsReminders, DAOLib.Models.ExpirationsReminders>().ReverseMap();
                cfg.CreateMap<BOLib.Models.InvestmentProducts, DAOLib.Models.InvestmentProducts>().ReverseMap();
                cfg.CreateMap<BOLib.Models.InvestmentProductsPrices, DAOLib.Models.InvestmentProductsPrices>().ReverseMap();
                cfg.CreateMap<BOLib.Models.PeriodsReminders, DAOLib.Models.PeriodsReminders>().ReverseMap();
                cfg.CreateMap<BOLib.Models.Persons, DAOLib.Models.Persons>().ReverseMap();
                cfg.CreateMap<BOLib.Models.Splits, DAOLib.Models.Splits>().ReverseMap();
                cfg.CreateMap<BOLib.Models.SplitsReminders, DAOLib.Models.SplitsReminders>().ReverseMap();
                cfg.CreateMap<BOLib.Models.Tags, DAOLib.Models.Tags>().ReverseMap();
                cfg.CreateMap<BOLib.Models.Transactions, DAOLib.Models.Transactions>().ReverseMap();
                cfg.CreateMap<BOLib.Models.TransactionsReminders, DAOLib.Models.TransactionsReminders>().ReverseMap();
                cfg.CreateMap<BOLib.Models.TransactionsStatus, DAOLib.Models.TransactionsStatus>().ReverseMap();
                cfg.CreateMap<BOLib.Models.VBalancebyCategory, DAOLib.Models.VBalancebyCategory>().ReverseMap();
            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
