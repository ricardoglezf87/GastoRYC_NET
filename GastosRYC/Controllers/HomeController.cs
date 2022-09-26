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
            //RYCContext rycContext = new RYCContext();

            //foreach (AccountsTypes a in rycContext.accountsTypes.Include(p=>p.accounts))
            //{                
            //    int i = 0;
            //    i = 3;
            //}

            List<object> listdata = new List<object>();
            listdata.Add(new
            {
                text = "Audi A4",
                id = "9bdb",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "Audi A4",
                id = "9bdb",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "Audi A5",
                id = "4589",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "Audi A6",
                id = "e807",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "Audi A7",
                id = "a0cc",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "Audi A8",
                id = "5e26",
                category = "Audi"
            });
            listdata.Add(new
            {
                text = "BMW 501",
                id = "f849",
                category = "BMW"
            });
            listdata.Add(new
            {
                text = "BMW 502",
                id = "7aff",
                category = "BMW"
            });
            listdata.Add(new
            {
                text = "BMW 503",
                id = "b1da",
                category = "BMW"
            });
            listdata.Add(new
            {
                text = "BMW 507",
                id = "de2f",
                category = "BMW"
            });
            listdata.Add(new
            {
                text = "BMW 3200",
                id = "b2b1",
                category = "BMW"
            });
            ViewBag.groupData = listdata;


            return View();
        }
    }
}
