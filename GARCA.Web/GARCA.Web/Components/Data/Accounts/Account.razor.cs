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
using GARCA.Utils.Enums;

namespace GARCA.Web.Components.Data.Accounts
{
    public partial class Account
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
        public int? id { get; set; }

        protected bool errorVisible;

        protected GARCA.Models.Accounts modelPage;

        protected IEnumerable<GARCA.Models.AccountsTypes> accountsTypesForAccountsTypesId;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (id != null && id != 0)
                {
                    modelPage = await dataRepository.AccountsRepository.GetById(id.Value);
                }
                else
                {
                    modelPage = new();
                }
                accountsTypesForAccountsTypesId = await dataRepository.AccountsTypesRepository.GetAll();
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task FormSubmit()
        {
            bool insert = false;
            Models.Categories cat = null;

            try
            {                
                if (id == null || id == 0)
                {
                    insert = true;

                    cat = new Models.Categories
                    {
                        Description = "[" + modelPage.Description + "]",
                        CategoriesTypesId = (int?)EnumCategories.ECategoriesTypes.Transfers
                    };

                    cat = await dataRepository.CategoriesRepository.Create(cat);
                    modelPage.Categoryid = cat.Id;

                    if(modelPage.Closed == null)
                    {
                        modelPage.Closed = false;
                    }

                    modelPage = await dataRepository.AccountsRepository.Create(modelPage);
                    
                }
                else
                {
                    cat = await dataRepository.CategoriesRepository.GetById(modelPage.Categoryid ?? (int)EnumComun.Id.Empty);
                    if (cat != null)
                    {
                        if (cat.Description != "[" + modelPage.Description + "]")
                        {
                            cat.Description = "[" + modelPage.Description + "]";
                            await dataRepository.CategoriesRepository.Update(cat);
                        }
                    }
                    else
                    {
                        throw new Exception("Ocurrio un eror la categoria de la cuenta");
                    }
                }
                
                await dataRepository.AccountsRepository.Update(modelPage);
                
                DialogService.Close(modelPage);
            }
            catch (Exception ex)
            {
                if (insert)
                {
                    if(modelPage.Id == 0)
                    {
                        await dataRepository.AccountsRepository.Delete(modelPage.Id);
                    }

                    if (cat != null)
                    {
                        await dataRepository.CategoriesRepository.Delete(cat.Id);
                    }
                }

                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}