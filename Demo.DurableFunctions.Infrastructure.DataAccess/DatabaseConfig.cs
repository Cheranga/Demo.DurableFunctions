namespace Demo.DurableFunctions.Infrastructure.DataAccess
{
    public class DatabaseConfig
    {
        public string ConnectionString { get; set; }
        public string CustomersTable { get; set; }
        public string BankAccountsTable { get; set; }
    }
}