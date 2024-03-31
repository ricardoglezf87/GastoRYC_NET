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
        
        public SplitsRemindersRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            splitsReminder = await repository.GetById(id);

            transactionsRemindersForTransactionsId = await new TransactionsRemindersRepository().GetAll();

            tagsForTagsId = await new TagsRepository().GetAll();

            categoriesForCategoryid = await new CategoriesRepository().GetAll();
        }
        protected bool errorVisible;
        protected GARCA.Models.SplitsReminders splitsReminder;

        protected IEnumerable<GARCA.Models.TransactionsReminders> transactionsRemindersForTransactionsId;

        protected IEnumerable<GARCA.Models.Tags> tagsForTagsId;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(splitsReminder);
                }
                else
                {
                    await repository.Update(splitsReminder);
                }

                DialogService.Close(splitsReminder);
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