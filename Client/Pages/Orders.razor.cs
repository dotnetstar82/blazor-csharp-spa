using BlazorSPA.Client.Models;
using BlazorSPA.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorSPA.Client.Pages
{
    public partial class OrdersModel : ComponentBase
    {

        [Inject]
        protected HttpClient Http { get; set; }
        [Inject]
        protected NavigationManager NavManager { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Parameter]
        public int Id { get; set; }
        public string Query { get; set; }

        public string PageTitle { get; set; }

        public List<Order> Orders { get; set; }
        public Order Editing { get; set; } = new Order();
        public bool Loading { get; set; }
        public string DescriptionError { get; set; }
        public string QuantityError { get; set; }
        public string SaveError { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("query", out var query))
            {
                this.Query = query.FirstOrDefault();
            }

            this.Loading = true;
            this.Orders = await LoadOrders(this.Query);
            this.Loading = false;
        }

        protected override async Task OnParametersSetAsync()
        {
            if (this.Id < 1)
            {
                await this.OnNewOrder();
            }
            else
            {
                await this.OnLoadOrder(this.Id);
            }
        }

        public async Task OnClearQuery()
        {
            this.Query = null;
            this.Orders = await this.LoadOrders(this.Query);
            await this.UpdateUrl("/orders");
        }

        public async Task OnNewOrder()
        {
            this.ClearErrors();
            this.BuildNewOrder();
            await this.UpdateUrl("/orders");
        }

        public async Task OnLoadOrder(int id)
        {
            this.ClearErrors();
            PageTitle = "Edit Order";
            this.Id = id;
            var order = await GetOrder(this.Id);
            if (order == null)
            {
                this.SaveError = "Error loading order id " + id;
            }
            else
            {
                this.Editing = order;
                await this.UpdateUrl("/order/" + id);
            }
        }

        public async Task OnSubmitForm()
        {
            this.ClearErrors();
            await this.SaveOrder();
            this.Orders = await this.LoadOrders(this.Query);
            this.BuildNewOrder();
            await this.UpdateUrl("/orders");
        }

        public async Task OnDeleteOrder(int id)
        {
            this.ClearErrors();
            await this.DeleteOrder(id);
            this.Orders = await this.LoadOrders(this.Query);
        }


        private void ClearErrors()
        {
            this.SaveError = null;
            this.DescriptionError = null;
            this.QuantityError = null;
        }

        private async Task UpdateUrl(string url)
        {
            if (!string.IsNullOrEmpty(this.Query))
            {
                url += "?query=" + Uri.EscapeUriString(this.Query ?? "");
            }
            await JSRuntime.InvokeAsync<string>("history.pushState", new object[] { null, "", url });
        }

        private async Task<List<Order>> LoadOrders(string query)
        {
            string url = "/api/order";
            if (!string.IsNullOrEmpty(query))
            {
                url += "?query=" + Uri.EscapeUriString(query ?? "");
            }
            return await Http.GetFromJsonAsync<List<Order>>(url);
        }

        private async Task<Order> GetOrder(int id) =>
            // TODO: handle 404
            await Http.GetFromJsonAsync<Order>("/api/order/" + id);

        private void BuildNewOrder()
        {
            this.Id = 0;
            PageTitle = "Add Order";
            this.Editing = new Order();
        }
        private async Task SaveOrder()
        {
            HttpResponseMessage res;
            if (this.Id > 0)
            {
                // update
                res = await Http.PutAsJsonAsync("/api/order/" + this.Id, this.Editing);
            }
            else
            {
                // create
                res = await Http.PostAsJsonAsync("/api/order", this.Editing);
            }
            if (res.IsSuccessStatusCode)
            {
                // success
                Order order = await res.Content.ReadFromJsonAsync<Order>();
                this.Id = order.Id;
            }
            else
            {
                // TODO: separate 4xx & 5xx
                // handle validation errors
                var validationError = await res.Content.ReadFromJsonAsync<ValidationError>();
                foreach (var error in validationError.Errors)
                {
                    if (error.Key == nameof(Order.Description))
                    {
                        this.DescriptionError = string.Join(", ", error.Value);
                    }
                    else if (error.Key == nameof(Order.Quantity))
                    {
                        this.QuantityError = string.Join(", ", error.Value);
                    }
                    else
                    {
                        this.SaveError = string.Join(", ", error.Value);
                    }
                }
            }
        }

        private async Task DeleteOrder(int id) =>
            await Http.DeleteAsync("/api/order/" + id);

    }

}
