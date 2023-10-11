using GARCA.BO.Services;
using GARCA.DAO.Managers;
using GARCA.View.ViewModels;
using System;
using System.Collections.Generic;
using GARCA.View.Utils.Extensions;

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
