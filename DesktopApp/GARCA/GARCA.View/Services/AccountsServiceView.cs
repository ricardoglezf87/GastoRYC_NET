using GARCA.BO.Services;
using GARCA.View.Utils.Extensions;
using GARCA.View.ViewModels;
using System.Collections.Generic;

namespace GARCA.View.Services
{
    public class AccountsServiceView : AccountsService
    {
        public HashSet<AccountsView>? GetAllOpenedListView()
        {
            return accountsManager.GetAllOpened()?.ToHashSetViewBo();
        }
    }
}
