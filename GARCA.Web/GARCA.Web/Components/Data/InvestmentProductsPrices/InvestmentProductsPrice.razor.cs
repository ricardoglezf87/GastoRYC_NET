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
using GARCA.Web.Components.Data.InvestmentProducts;

namespace GARCA.Web.Components.Data.InvestmentProductsPrices
{
    public partial class InvestmentProductsPrice
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
        
        protected GARCA.Models.InvestmentProductsPrices modelPage;

        protected override async Task OnInitializedAsync()
        {
            modelPage = await dataRepository.InvestmentProductsPricesRepository.GetById(id);
        }

        protected async Task FormSubmit()
        {
            try
            {
                if (id == null || id == 0)
                {
                    await dataRepository.InvestmentProductsPricesRepository.Create(modelPage);
                }
                else
                {
                    await dataRepository.InvestmentProductsPricesRepository.Update(modelPage);
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