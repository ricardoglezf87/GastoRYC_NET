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
            //var transactions = ((ICollectionManager<Transactions>)new TransactionManager()).getAll();
            RYCContext rycContext = new RYCContext();
            
            foreach (AccountsTypes a in rycContext.accountsTypes.Include(p=>p.accounts))
            {                
                int i = 0;
                i = 3;
            }


            return View();
        }
    }
}
