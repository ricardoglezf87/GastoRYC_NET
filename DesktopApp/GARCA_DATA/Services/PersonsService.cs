using Dapper;
using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class PersonsService : ServiceBase<PersonsManager, Persons>
    {
        public async Task SetCategoryDefault(int id)
        {
            var categoryid = await iRycContextService.getConnection().ExecuteScalarAsync<int?>(@$"
                SELECT categoryid, COUNT(categoryid) AS repetition_count
                FROM (
                    SELECT categoryid FROM transactions WHERE personid = {id}
                    UNION ALL
                    SELECT categoryid FROM TransactionsArchived WHERE personid = {id}
                ) AS A
                GROUP BY categoryid
                ORDER BY repetition_count DESC
                LIMIT 1;
            ");

            if (categoryid != null)
            {
                Persons? persons = await iPersonsService.GetById(id);
                persons.Categoryid = categoryid;
                await Save(persons);
            }
        }
    }
}
