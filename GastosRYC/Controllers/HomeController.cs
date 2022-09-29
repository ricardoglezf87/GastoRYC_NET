using GastosRYCLib.Manager;
using GastosRYCLib.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
//using TransactionManager = GastosRYCLib.Manager.TransactionManager;

namespace GastosRYC.Controllers
{
    public class HomeController : Controller
    {
 
        public ActionResult Index()
        {            
            List<object> listdata = new List<object>();
            RYCContext rycContext = new RYCContext();

            rycContext.accounts?.Include(p => p.accountsTypes);
            rycContext.categories?.Load();
            //rycContext.persons?.Load();

            if (rycContext.accounts != null)
            {
                foreach (Accounts a in rycContext.accounts)
                {
                    listdata.Add(new
                    {
                        id = a.id,
                        text = a.description,
                        value = a.balance,
                        type = a.accountsTypes?.description
                    });
                }
            }

            ViewBag.groupData = listdata;
            ViewBag.cbAccount = rycContext.accounts;
            ViewBag.cbPerson = rycContext.persons;
            ViewBag.cbCategory = rycContext.categories;
            ViewBag.datasource = rycContext.transactions;

            return View();
        }
    }
}

