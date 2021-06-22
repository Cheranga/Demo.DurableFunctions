using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Demo.DurableFunctions.Extensions
{
    public static class Retry
    {
        public static RetryOptions For<TException>(int firstRetryInSeconds = 5, int numOfRetries = 3) where TException:Exception
        {
            firstRetryInSeconds = firstRetryInSeconds <= 0 ? 5 : firstRetryInSeconds;
            numOfRetries = numOfRetries <= 0 ? 3 : numOfRetries;
            
            var retryOptions = new RetryOptions(TimeSpan.FromSeconds(firstRetryInSeconds), numOfRetries)
            {
                Handle = exception => exception?.InnerException?.GetType() == typeof(TException)
            };
            return retryOptions;
        }
    }
}