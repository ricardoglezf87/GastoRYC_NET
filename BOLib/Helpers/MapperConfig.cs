using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAOLib.Models;
using BOLib.Models;

namespace BOLib.Helpers
{
    public class MapperConfig
    {
        public static Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<Accounts, AccountsDAO>().ReverseMap();                
                //cfg.CreateMap<AccountsTypes, AccountsTypesDAO>().ReverseMap();                
                //cfg.CreateMap<Categories, CategoriesDAO>().ReverseMap();                
                //cfg.CreateMap<CategoriesTypes, CategoriesTypesDAO>().ReverseMap();
                cfg.CreateMap<DateCalendar, DateCalendarDAO>().ReverseMap();
                //cfg.CreateMap<ExpirationsReminders, ExpirationsRemindersDAO>().ReverseMap();
                //cfg.CreateMap<InvestmentProducts, InvestmentProductsDAO>().ReverseMap();
                cfg.CreateMap<InvestmentProductsPrices, InvestmentProductsPricesDAO>().ReverseMap();
                //cfg.CreateMap<PeriodsReminders, PeriodsRemindersDAO>().ReverseMap();
                //cfg.CreateMap<Persons, PersonsDAO>().ReverseMap();
                //cfg.CreateMap<Splits, SplitsDAO>().ReverseMap();
                cfg.CreateMap<SplitsReminders, SplitsRemindersDAO>().ReverseMap();
                //cfg.CreateMap<Tags, TagsDAO>().ReverseMap();
                //cfg.CreateMap<Transactions, TransactionsDAO>().ReverseMap();
                cfg.CreateMap<TransactionsReminders, TransactionsRemindersDAO>().ReverseMap();
                //cfg.CreateMap<TransactionsStatus, TransactionsStatusDAO>().ReverseMap();
                cfg.CreateMap<VBalancebyCategory, VBalancebyCategoryDAO>().ReverseMap();
            });
            var mapper = new Mapper(config);
            return mapper;
        }
    }
}
