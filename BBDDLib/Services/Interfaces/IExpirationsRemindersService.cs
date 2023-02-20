using BBDDLib.Models;
using System;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface IExpirationsRemindersService
    {
        public List<ExpirationsReminders>? getAll();

        public List<ExpirationsReminders>? getAllWithGeneration();

        public bool existsExpiration(TransactionsReminders? transactionsReminder, DateTime? date);

        public List<ExpirationsReminders>? getAllPendingWithGeneration();

        public List<ExpirationsReminders>? getAllPendingWithoutFutureWithGeneration();

        public void GenerationAllExpirations();

        public void GenerationExpirations(TransactionsReminders? transactionsReminders);

        public void registerTransactionfromReminder(int? id);

        public ExpirationsReminders? getByID(int? id);

        public List<ExpirationsReminders>? getByTransactionReminderid(int? id);

        public void update(ExpirationsReminders expirationsReminders);

        public void delete(ExpirationsReminders expirationsReminders);

        public void deleteByTransactionReminderid(int id);
    }
}
