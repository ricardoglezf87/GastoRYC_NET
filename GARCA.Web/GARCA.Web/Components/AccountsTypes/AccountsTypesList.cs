using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using GARCA.Models;
using GARCA.Web.Data.Repositories;

namespace GARCA.Web.Components.AccountsTypes
{
    public partial class AccountsTypesList
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

        protected AccountsTypesRepository accountsTypesRepository { get; set; } = new ();

        protected IEnumerable<Models.AccountsTypes> accountsTypes;

        protected RadzenDataGrid<Models.AccountsTypes> grid0;

        protected int count;

        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                var result = await accountsTypesRepository.GetAll();
                //(filter: $"{args.Filter}", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                //accountsTypes = result.Value.AsODataEnumerable();                
                accountsTypes = result;
                count = result.Count();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"No se ha podido cargar la lista:" + ex.Message });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            //    await DialogService.OpenAsync<AddAccountsType>("Add AccountsType", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<GARCA.Models.AccountsTypes> args)
        {
            //    await DialogService.OpenAsync<EditAccountsType>("Edit AccountsType", new Dictionary<string, object> { {"id", args.Data.id} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AccountsTypes accountsType)
        {
            try
            {
                if (await DialogService.Confirm("¿Está seguro de querer borrar este registro?") == true)
                {
                    await accountsTypesRepository.Delete(accountsType.Id);
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"No se la podido borrar el elemento: " + ex.Message });

            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            //            if (args?.Value == "csv")
            //            {
            //                await GARCA_PREService.ExportAccountsTypesToCSV(new Query
            //{
            //    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
            //    OrderBy = $"{grid0.Query.OrderBy}",
            //    Expand = "",
            //    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
            //}, "AccountsTypes");
            //            }

            //            if (args == null || args.Value == "xlsx")
            //            {
            //                await GARCA_PREService.ExportAccountsTypesToExcel(new Query
            //{
            //    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
            //    OrderBy = $"{grid0.Query.OrderBy}",
            //    Expand = "",
            //    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
            //}, "AccountsTypes");
            //            }
        }
    }
}