
using Microsoft.Azure.Cosmos.Table;

namespace Demo.DurableFunctions.DataAccess.Models
{
    public class CustomerDataModel : TableEntity
    {
        public string CustomerId { get; set; }
        public string PassportNo { get; set; }
        public string DriverLicenseNo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool MobileVerified { get; set; }
    }
}