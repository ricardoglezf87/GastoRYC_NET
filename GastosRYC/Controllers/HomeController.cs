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

            foreach(Accounts a in rycContext.accounts.Include(p => p.accountsTypes))
            {
                listdata.Add(new
                {
                    id = a.id,
                    text = a.description,
                    value = a.balance,
                    type = a.accountsTypes?.description
                });
            }

            ViewBag.groupData = listdata;

            return View();
        }
    }
}
