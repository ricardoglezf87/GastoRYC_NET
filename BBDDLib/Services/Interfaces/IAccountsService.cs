using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Services.Interfaces
{
    public interface IAccountsService
    {
        public List<Accounts>? getAll();
        public List<Accounts>? getAllOrderByAccountsTypesId();
        public Accounts? getByID(int? id);
        public void update(Accounts accounts);
        public void delete(Accounts accounts);
    }
}
