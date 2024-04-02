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

namespace GARCA.Web.Components.Data.People
{
    public partial class Person
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
        
        protected GARCA.Models.Persons modelPage;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected override async Task OnInitializedAsync()
        {
            modelPage = await dataRepository.PersonsRepository.GetById(id);

            categoriesForCategoryid = await dataRepository.CategoriesRepository.GetAll();
        }
        
        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await dataRepository.PersonsRepository.Create(modelPage);
                }
                else
                {
                    await dataRepository.PersonsRepository.Update(modelPage);
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