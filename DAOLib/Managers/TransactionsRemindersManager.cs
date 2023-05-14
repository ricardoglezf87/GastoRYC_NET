using DAOLib.Models;
using DAOLib.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsRemindersDAO>
    {
        public int getNextID()
        {
            var cmd = RYCContextServiceDAO.getInstance().BBDD.Database.
                GetDbConnection().CreateCommand();
            cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactionsReminders';";

            RYCContextServiceDAO.getInstance().BBDD.Database.OpenConnection();
            var result = cmd.ExecuteReader();
            result.Read();
            int id = Convert.ToInt32(result[0]);
            result.Close();

            return id;
        }
    }   
}
