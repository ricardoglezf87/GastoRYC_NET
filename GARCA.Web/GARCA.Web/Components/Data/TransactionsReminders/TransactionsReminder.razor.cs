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

        [Inject]
        public DataRepositories dataRepository { get; set; }

        [Parameter]
        public int? id { get; set; }

        protected bool errorVisible;

        protected GARCA.Models.TransactionsReminders modelPage;

        protected IEnumerable<GARCA.Models.Accounts> accountsForAccountsId;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected IEnumerable<GARCA.Models.PeriodsReminders> periodsRemindersForPeriodsRemindersId;

        protected IEnumerable<GARCA.Models.Persons> peopleForPersonsId;

        protected IEnumerable<GARCA.Models.Tags> tagsForTagsId;

        protected IEnumerable<GARCA.Models.TransactionsStatus> transactionsStatusesForTransactionsStatusId;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (id != null && id != 0)
                {
                    modelPage = await dataRepository.TransactionsRemindersRepository.GetById(id.Value);
                }
                else
                {
                    modelPage = new();
                }

                accountsForAccountsId = await dataRepository.AccountsRepository.GetAll();

                categoriesForCategoryid = await dataRepository.CategoriesRepository.GetAll();

                periodsRemindersForPeriodsRemindersId = await dataRepository.PeriodsRemindersRepository.GetAll();

                peopleForPersonsId = await dataRepository.PersonsRepository.GetAll();

                tagsForTagsId = await dataRepository.TagsRepository.GetAll();

                transactionsStatusesForTransactionsStatusId = await dataRepository.TransactionsStatusRepository.GetAll();
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
                if (id == null || id == 0)
                {
                    await dataRepository.TransactionsRemindersRepository.Create(modelPage);
                }
                else
                {
                    await dataRepository.TransactionsRemindersRepository.Update(modelPage);
                }

                DialogService.Close(modelPage);
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