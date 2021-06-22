namespace Demo.DurableFunctions.Core.Application.DataAccess
{
    public class UpdateCustomerCommand : ICommand
    {
        public string CustomerId { get; set; }
        public bool MobileVerified { get; set; }
    }
}