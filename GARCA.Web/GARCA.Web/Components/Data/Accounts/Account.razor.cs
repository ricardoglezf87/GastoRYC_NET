using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using GARCA.Web.Data.Repositories;

namespace GARCA.Web.Components.Data.Accounts
{
    public partial class Account
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
       
        public AccountsRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            account = await repository.GetById(id);

            accountsTypesForAccountsTypesId = await new AccountsTypesRepository().GetAll();

            categoriesForCategoryid = await new CategoriesRepository().GetAll();
        }
        protected bool errorVisible;
        protected GARCA.Models.Accounts account;

        protected IEnumerable<GARCA.Models.AccountsTypes> accountsTypesForAccountsTypesId;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(account);
                }
                else
                {
                    await repository.Update(account);
                }

                DialogService.Close(account);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}