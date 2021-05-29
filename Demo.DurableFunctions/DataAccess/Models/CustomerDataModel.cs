using Microsoft.WindowsAzure.Storage.Table;

namespace Demo.DurableFunctions.DataAccess.Models
{
    public class CustomerDataModel : TableEntity
    {
        public string CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}