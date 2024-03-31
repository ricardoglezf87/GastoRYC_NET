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
        
        public PersonsRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            person = await repository.GetById(id);

            categoriesForCategoryid = await new CategoriesRepository().GetAll();
        }
        protected bool errorVisible;
        protected GARCA.Models.Persons person;

        protected IEnumerable<GARCA.Models.Categories> categoriesForCategoryid;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(person);
                }
                else
                {
                    await repository.Update(person);
                }

                DialogService.Close(person);
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