namespace Demo.DurableFunctions.DTO.Requests
{
    public class RegisterBankCustomerRequest
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountName { get; set; }
        public BankAccountType BankAccountType { get; set; }
        public decimal 
            Deposit { get; set; }
    }

    public enum BankAccountType
    {
        Savings,
        Cheque
    }
}