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

namespace GARCA.Web.Components.Data.AccountsTypes
{
    public partial class AccountsType
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

        [Inject]
        public DataRepositories dataRepository { get; set; }

        protected bool errorVisible;

        protected GARCA.Models.AccountsTypes accountsTypes;

        [Parameter]
        public int? id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (id != null && id != 0)
                {
                    accountsTypes = await dataRepository.AccountsTypesRepository.GetById(id.Value);
                }
                else
                {
                    accountsTypes = new();
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task FormSubmit()
        {
            try
            {
                if(id == null || id == 0)
                {
                    await dataRepository.AccountsTypesRepository.Create(accountsTypes);
                }
                else
                {
                    await dataRepository.AccountsTypesRepository.Update(accountsTypes);
                }

                DialogService.Close(accountsTypes);
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