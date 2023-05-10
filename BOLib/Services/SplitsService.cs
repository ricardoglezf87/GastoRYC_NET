using BOLib.Helpers;
using BOLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class SplitsService
    {

        private readonly SimpleInjector.Container servicesContainer;

        public SplitsService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        public List<Splits>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<Splits>>(RYCContextService.getInstance().BBDD.splits?.ToList());
        }

        public List<Splits>? getbyTransactionidNull()
        {
            return MapperConfig.InitializeAutomapper().Map<List<Splits>>(RYCContextService.getInstance().BBDD.splits?.Where(x => x.transactionid == null).ToList());
        }

        public List<Splits>? getbyTransactionid(int transactionid)
        {
            return MapperConfig.InitializeAutomapper().Map<List<Splits>>(RYCContextService.getInstance().BBDD.splits?.Where(x => x.transactionid == transactionid).ToList());
        }

        public Splits? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<Splits>(RYCContextService.getInstance().BBDD.splits?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public void update(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Update(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Splits splits)
        {
            RYCContextService.getInstance().BBDD.Remove(splits);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public Decimal? getAmountTotal(Transactions transactions)
        {
            Decimal? total = 0;
            if (transactions.splits != null && transactions.splits.Count != 0)
            {
                foreach (Splits splits in transactions.splits)
                {
                    total += (splits.amountIn == null ? 0 : splits.amountIn);
                    total -= (splits.amountOut == null ? 0 : splits.amountOut);
                }
            }

            return total;
        }

        public void saveChanges(Transactions? transactions, Splits splits)
        {
            if (splits.category == null && splits.categoryid != null)
            {
                splits.category = servicesContainer.GetInstance<CategoriesService>().getByID(splits.categoryid);
            }

            if (splits.amountIn == null)
                splits.amountIn = 0;

            if (splits.amountOut == null)
                splits.amountOut = 0;

            update(splits);
        }

        public int getNextID()
        {
            var cmd = RYCContextService.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splits';";

            RYCContextService.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }
}
