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

namespace GARCA.Web.Components.Data.SplitsReminders
{
    public partial class SplitsReminder
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
        public int id { get; set; }

        protected bool errorVisible;

        protected GARCA.Models.SplitsReminders modelPage;

        protected IEnumerable<GARCA.Models.TransactionsReminders> transactionsRemindersForTransactionsId;

        protected IEnumerable<GARCA.Models.Tags> tagsForTagsId;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected override async Task OnInitializedAsync()
        {
            modelPage = await dataRepository.SplitsRemindersRepository.GetById(id);

            transactionsRemindersForTransactionsId = await dataRepository.TransactionsRemindersRepository.GetAll();

            tagsForTagsId = await dataRepository.TagsRepository.GetAll();

            categoriesForCategoryid = await dataRepository.CategoriesRepository.GetAll();
        }

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await dataRepository.SplitsRemindersRepository.Create(modelPage);
                }
                else
                {
                    await dataRepository.SplitsRemindersRepository.Update(modelPage);
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