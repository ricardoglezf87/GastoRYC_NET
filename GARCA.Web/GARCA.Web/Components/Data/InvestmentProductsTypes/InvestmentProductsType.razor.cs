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
using GARCA.Web.Components.Data.InvestmentProductsPrices;

namespace GARCA.Web.Components.Data.InvestmentProductsTypes
{
    public partial class InvestmentProductsType
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
        
        public InvestmentProductsTypesRepository repository { get; set; }

        [Parameter]
        public int id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            repository = new();
            investmentProductsType = await repository.GetById(id);
        }
        protected bool errorVisible;
        protected GARCA.Models.InvestmentProductsTypes investmentProductsType;

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await repository.Create(investmentProductsType);
                }
                else
                {
                    await repository.Update(investmentProductsType);
                }

                DialogService.Close(investmentProductsType);
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