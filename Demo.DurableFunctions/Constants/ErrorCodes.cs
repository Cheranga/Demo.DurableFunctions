namespace Demo.DurableFunctions.Constants
{
    public class ErrorCodes
    {
        public const string InvalidVisa = nameof(InvalidVisa);
        public const string DriverLicenseError = nameof(DriverLicenseError);
        public const string CreateCustomerError = nameof(CreateCustomerError);
        public const string CreateBankAccountError = nameof(CreateBankAccountError);
        public const string RegisterAccountError = nameof(RegisterAccountError);
        public const string MaxOTCAttemptsReached = nameof(MaxOTCAttemptsReached);
        public const string OTCCodeGeneration = nameof(OTCCodeGeneration);
        public const string OTCTimeout = nameof(OTCTimeout);
        public const string SendSmsFailure = nameof(SendSmsFailure);
    }

    public class ErrorMessages
    {
        public const string InvalidVisa = "error occurred when checking for VISA status";
        public const string DriverLicenseError = "error occurred when checking for driver license status";
        public const string CreateCustomerError = "error occurred when creating the customer";
        public const string CreateBankAccountError = "error occurred when creating the bank account";
        public const string RegisterAccountError = "error occurred when registering the account";
        public const string MaxOTCAttemptsReached = "maximum number of OTC attempts have been reached";
        public const string OTCCodeGeneration = "error occurred when generating the OTC";
        public const string OTCTimeout = "timeout occurred when confirming the OTC";
        public const string SendSmsFailure = "error occurred when sending SMS";
    }
}