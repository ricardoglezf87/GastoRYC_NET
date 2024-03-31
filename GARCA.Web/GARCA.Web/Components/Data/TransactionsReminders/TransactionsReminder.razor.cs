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

namespace GARCA.Web.Components.Data.TransactionsReminders
{
    public partial class TransactionsReminder
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
        
        public TransactionsRemindersRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            transactionsReminder = await repository.GetById(id);

            accountsForAccountsId = await new AccountsRepository().GetAll();

            categoriesForCategoryid = await new CategoriesRepository().GetAll();

            periodsRemindersForPeriodsRemindersId = await new PeriodsRemindersRepository().GetAll();

            peopleForPersonsId = await new PersonsRepository().GetAll();

            tagsForTagsId = await new TagsRepository().GetAll();

            transactionsStatusesForTransactionsStatusId = await new TransactionsStatusRepository().GetAll();
        }
        protected bool errorVisible;
        protected GARCA.Models.TransactionsReminders transactionsReminder;

        protected IEnumerable<GARCA.Models.Accounts> accountsForAccountsId;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected IEnumerable<GARCA.Models.PeriodsReminders> periodsRemindersForPeriodsRemindersId;

        protected IEnumerable<GARCA.Models.Persons> peopleForPersonsId;

        protected IEnumerable<GARCA.Models.Tags> tagsForTagsId;

        protected IEnumerable<GARCA.Models.TransactionsStatus> transactionsStatusesForTransactionsStatusId;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(transactionsReminder);
                }
                else
                {
                    await repository.Update(transactionsReminder);
                }

                DialogService.Close(transactionsReminder);
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