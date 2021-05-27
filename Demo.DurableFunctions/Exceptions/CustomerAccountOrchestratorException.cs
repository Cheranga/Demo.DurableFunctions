using System;

namespace Demo.DurableFunctions.Exceptions
{
    public class CustomerAccountOrchestratorException : Exception
    {
        public CustomerAccountOrchestratorException(string instanceId)
        {
            InstanceId = instanceId;
        }

        private string InstanceId { get; }
        public override string Message => $"{InstanceId} failed";
    }
}