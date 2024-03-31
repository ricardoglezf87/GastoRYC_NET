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

namespace GARCA.Web.Components.Data.Splits
{
    public partial class Splits
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

        
        public SplitsRepository repository { get; set; }

        protected IEnumerable<GARCA.Models.Splits> splits;

        protected RadzenDataGrid<GARCA.Models.Splits> grid0;
        protected override async Task OnInitializedAsync()
        {
            repository = new();
            splits = await repository.GetAll();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<Split>("Nuevo Split", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<GARCA.Models.Splits> args)
        {
            await DialogService.OpenAsync<Split>("Editar Split", new Dictionary<string, object> { {"Id", args.Data.Id} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, GARCA.Models.Splits split)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    await repository.Delete(split.Id);
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Split"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
//            if (args?.Value == "csv")
//            {
//                await repository.ExportSplitsToCSV(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "Transaction,Tag,Category",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "Splits");
//            }

//            if (args == null || args.Value == "xlsx")
//            {
//                await repository.ExportSplitsToExcel(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "Transaction,Tag,Category",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "Splits");
//            }
        }
    }
}