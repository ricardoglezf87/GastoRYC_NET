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

namespace GARCA.Web.Components.Data.TransactionsStatuses
{
    public partial class TransactionsStatus
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
        
        public TransactionsStatusRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected bool errorVisible;

        protected GARCA.Models.TransactionsStatus transactionsStatus;

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            transactionsStatus = await repository.GetById(id);
        }
       
        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(transactionsStatus);
                }
                else
                {
                    await repository.Update(transactionsStatus);
                }

                DialogService.Close(transactionsStatus);
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