using RR.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RR.Service
{
    public interface IStoreShopifyService : IDisposable
    {
       
        Task<SCustomer> AddEditCustomerAsync(SCustomerDto entity);
        Task<SCustomer> GetCustomerAsync(string id);
        Task<IEnumerable<SCustomer>> GetCustomersAsync();
        Task<bool> DeleteCustomerAsync(string id);

        Task<SWebhook> AddEditWebhookAsync(SWebhook entity);
        Task<IEnumerable<SWebhook>> GetWebhookAsync();
        Task<SWebhook> GetWebhookAsync(long id);

    }
}
