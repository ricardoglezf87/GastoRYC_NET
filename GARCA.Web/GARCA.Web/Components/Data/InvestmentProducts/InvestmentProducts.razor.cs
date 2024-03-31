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

namespace GARCA.Web.Components.Data.InvestmentProducts
{
    public partial class InvestmentProducts
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

        
        public InvestmentProductsRepository repository { get; set; }

        protected IEnumerable<GARCA.Models.InvestmentProducts> investmentProducts;

        protected RadzenDataGrid<GARCA.Models.InvestmentProducts> grid0;
        protected override async Task OnInitializedAsync()
        {
            repository = new();
            investmentProducts = await repository.GetAll();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<InvestmentProduct>("Nuevo InvestmentProduct", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<GARCA.Models.InvestmentProducts> args)
        {
            await DialogService.OpenAsync<InvestmentProduct>("Editar InvestmentProduct", new Dictionary<string, object> { {"Id", args.Data.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, GARCA.Models.InvestmentProducts investmentProduct)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    await repository.Delete(investmentProduct.Id);
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete InvestmentProduct"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
//            if (args?.Value == "csv")
//            {
//                await repository.ExportInvestmentProductsToCSV(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "InvestmentProducts");
//            }

//            if (args == null || args.Value == "xlsx")
//            {
//                await repository.ExportInvestmentProductsToExcel(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "InvestmentProducts");
//            }
        }
    }
}