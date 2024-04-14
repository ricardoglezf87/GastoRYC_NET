using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;

namespace GARCA.Utils.Helpers
{
    public static class IJsHelper
    {
        public static async ValueTask SetSessionVariable(this IJSRuntime JSRuntime, string name, string value)
        {
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", name, value);
        }

        public static async ValueTask<string> GetSessionVariable(this IJSRuntime JSRuntime,string name)
        {
            return await JSRuntime.InvokeAsync<string>("localStorage.getItem", name);            
        }

        public static async ValueTask<bool> SessionEmpty(this IJSRuntime JSRuntime, string name)
        {
            return string.IsNullOrEmpty(await GetSessionVariable(JSRuntime,name));
        }

        public static async ValueTask RemoveSessionVariable(this IJSRuntime JSRuntime, string name)
        {
            await JSRuntime.InvokeAsync<string>("localStorage.removeItem", name);
        }
    }
}
