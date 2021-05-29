namespace Demo.DurableFunctions.DTO.Requests
{
    public class CreateBankAccountRequest
    {
        public string CustomerId { get; set; }
        public BankAccountType BankAccountType { get; set; }
        public string AccountName { get; set; }
        public decimal Deposit { get; set; }
    }
}