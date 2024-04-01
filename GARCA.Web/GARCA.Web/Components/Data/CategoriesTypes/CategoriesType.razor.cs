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

namespace GARCA.Web.Components.Data.CategoriesTypes
{
    public partial class CategoriesType
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
        
        public CategoriesTypesRepository repository { get; set; }

        [Parameter]
        public int? id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            try
            {
                if (id != null && id != 0)
                {
                    modelPage = await repository.GetById(id.Value);
                }
                else
                {
                    modelPage = new();
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }
        protected bool errorVisible;
        protected GARCA.Models.CategoriesTypes modelPage;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(modelPage);
                }
                else
                {
                    await repository.Update(modelPage);
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