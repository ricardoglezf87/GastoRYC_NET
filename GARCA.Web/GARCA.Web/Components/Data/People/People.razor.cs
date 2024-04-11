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
    public partial class People
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

        protected IEnumerable<GARCA.Models.Persons> modelPage;

        protected RadzenDataGrid<GARCA.Models.Persons> grid0;

        protected int count;

        protected async Task Grid0LoadData(LoadDataArgs args)
        {
            try
            {
                var result = await dataRepository.PersonsRepository.GetAll();
                //(filter: $"{args.Filter}", orderby: $"{args.OrderBy}", top: args.Top, skip: args.Skip, count:args.Top != null && args.Skip != null);
                //modelPage = result.Value.AsODataEnumerable();                
                modelPage = result;
                count = result.Count();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage() { Severity = NotificationSeverity.Error, Summary = $"Error", Detail = $"No se ha podido cargar la lista:" + ex.Message });
            }
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<Person>("Nueva persona", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<GARCA.Models.Persons> args)
        {
            await DialogService.OpenAsync<Person>("Editar persona", new Dictionary<string, object> { {"Id", args.Data.Id} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, GARCA.Models.Persons person)
        {
            try
            {
                if (await DialogService.Confirm("¿Está seguro de querer borrar este registro?") == true)
                {
                    await dataRepository.PersonsRepository.Delete(person.Id);
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Person"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
//            if (args?.Value == "csv")
//            {
//                await dataRepository.ExportPeopleToCSV(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "Category",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "People");
//            }

//            if (args == null || args.Value == "xlsx")
//            {
//                await dataRepository.ExportPeopleToExcel(new Query
//{
//    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
//    OrderBy = $"{grid0.Query.OrderBy}",
//    Expand = "Category",
//    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
//}, "People");
//            }
        }
    }
}