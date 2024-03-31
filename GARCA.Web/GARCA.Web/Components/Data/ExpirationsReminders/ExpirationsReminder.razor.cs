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

namespace GARCA.Web.Components.Data.ExpirationsReminders
{
    public partial class ExpirationsReminder
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
        
        public ExpirationsRemindersRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            expirationsReminder = await repository.GetById(id);

            transactionsRemindersForTransactionsRemindersId = await new TransactionsRemindersRepository().GetAll();
        }
        protected bool errorVisible;
        protected GARCA.Models.ExpirationsReminders expirationsReminder;

        protected IEnumerable<GARCA.Models.TransactionsReminders> transactionsRemindersForTransactionsRemindersId;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(expirationsReminder);
                }
                else
                {
                    await repository.Update(expirationsReminder);
                }

                DialogService.Close(expirationsReminder);
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