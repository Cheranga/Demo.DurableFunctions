namespace Demo.DurableFunctions.Core.Application.DataAccess
{
    public class CreateCustomerCommand : ICommand
    {
        public string CustomerId { get; set; }
        public string DriverLicenseNo { get; set; }
        public string Email { get; set; }
        public bool MobileVerified { get; set; }
        public string Name { get; set; }
        public string PassportNo { get; set; }
    }
}