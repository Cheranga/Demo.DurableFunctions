## :zap: Azure Durable Functions :zap:

## :zap: Setting up your IDE

### Rider
  
:white_check_mark:]  Install `Azure Toolkit for Rider` extension


### VSCode

Please refer the documentation in below.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp

Installing below extensions will be very helpful as well

:white_check_mark:]  Azure Functions

Create, debug and deploy Azure functions from VSCode itself.

:white_check_mark:]  Azure Account

> Can work in the Azure cloud shell from your VSCode.

:white_check_mark:]  Azure CLI Tools

> Azure CLI intellisense! Create a file with the extension `.azcli` and you can write and execute AZ CLI commands from VS code itself.

:white_check_mark:]  Azure Resource Manager Tools

> ARM template snippets and ARM template validations. This has been a life saver!

- Other
  - [x] Install `Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers` nuget package. It will guide you potential code violations when implementing Azure durable functions.
  
## :zap: What are Azure durable functions?

Durable Functions is an extension of Azure Functions that lets you write stateful functions in a serverless compute environment. The extension lets you define stateful workflows by writing orchestrator functions and stateful entities by writing entity functions using the Azure Functions programming model. Behind the scenes, the extension manages state, checkpoints, and restarts for you, allowing you to focus on your business logic.

## :zap: Durable function types
There are four different durable function types.

### :zap: Client

Both `orchestrator` and `entity` functions are triggered when messages are enqueued into a `task hub`.
The primary way to do that is through a `client` function. Any function with a `durable client` output binding can be a client function.

Most of the time the client functions are regular trigger functions such as `HTTP, BLOB, Service Bus, Event Hub`

### :zap: Orchestrator [OrchestrationTrigger]
  
Orchestrator functions describe how actions are executed and the order in which actions are executed. Orchestrator functions describe the orchestration in code.
An orchestration can have many different types of actions, including activity functions, sub-orchestrations, waiting for external events, HTTP, and timers. Orchestrator functions can also interact with entity functions.

* Constraints

:point_up: Orchestrator functions must be deterministic.

:point_up: Dates and times (current date time).

:point_up: GUID generations and random number generations.

:point_up: Bindings.

:point_up: Network calls.

:point_up: Async APIs.

:point_up: Threading APIs

Never use `ConfigureAwait(false)`. The orchestrator functions must run in it's original thread.








### :zap: Activity [ActivityTrigger]
Activity functions are the basic unit of work in a durable function orchestration. Activity functions are the functions and tasks that are orchestrated in the process.
Unlike orchestrator functions, activity functions aren't restricted in the type of work you can do in them. Activity functions are frequently used to make network calls or run CPU intensive operations. An activity function can also return data back to the orchestrator function.

### :zap: Entity [EntityTrigger]

Entity functions define operations for reading and updating small pieces of state. We often refer to these stateful entities as durable entities. Like orchestrator functions, entity functions are functions with a special trigger type, entity trigger. They can also be invoked from client functions or from orchestrator functions. Unlike orchestrator functions, entity functions do not have any specific code constraints. Entity functions also manage state explicitly rather than implicitly representing state via control flow.

## :zap: Comparing Azure Durable Functions

https://docs.microsoft.com/en-us/azure/azure-functions/functions-compare-logic-apps-ms-flow-webjobs#compare-azure-functions-and-azure-logic-apps


## :zap: Azure durable function patterns

### :zap: Function chaining

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/functionchaining.png "Function Chaining")

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/chainingfifo.png "Function Chaining and fifo")


* Asynchronously calling HTTP endpoint

```c#
[FunctionName(nameof(AsyncFunctionChainingClient))]
public async Task<IActionResult> FunctionChainingAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/async")]
    HttpRequestMessage request, [DurableClient] IDurableOrchestrationClient client)
{
    var registerBankCustomerRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);
    var validationResult = await validator.ValidateAsync(registerBankCustomerRequest);
    if (!validationResult.IsValid)
    {
        return new BadRequestObjectResult(validationResult);
    }
    
    var instanceId = await client.StartNewAsync(Orchestrator, registerBankCustomerRequest);
    return new OkObjectResult(instanceId);
}
```
* Synchronously calling HTTP endpoint
```c#
[FunctionName(nameof(SyncFunctionChainingClient))]
public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chaining/sync")]
    HttpRequestMessage request,
    [DurableClient]IDurableClient client)
{
    var operation = await registerBankAccountService.RegisterAsync(request, client);
    var response = responseFormatter.GetResponse(operation);
    return response;
}

// In `RegisterBankAccountService` class
public async Task<Result<RegisterAccountResponse>> RegisterAsync(HttpRequestMessage request, IDurableClient client)
{
    var registerAccountRequest = await requestBodyReader.GetModelAsync<RegisterAccountRequest>(request);
    var validationResult = await validator.ValidateAsync(registerAccountRequest);
    if (!validationResult.IsValid)
    {
        return Result<RegisterAccountResponse>.Failure("InvalidRequest", validationResult);
    }

    // Create a timeout
    var timeout = TimeSpan.FromSeconds(TimeoutInSeconds);
    
    // Call the orchestrator
    var instanceId = await client.StartNewAsync(nameof(RegisterAccountOrchestrator), registerAccountRequest);
    
    // Wait for completion or for the timeout to happen
    await client.WaitForCompletionOrCreateCheckStatusResponseAsync(request, instanceId, timeout);
    
    // Get the status of the orchestrator
    var orchestratorResult = await client.GetStatusAsync(instanceId, false, false, false);
    
    // If the orchestrator completed successfully return the response
    if (orchestratorResult.RuntimeStatus == OrchestrationRuntimeStatus.Completed)
    {
        var operation = orchestratorResult.Output.ToObject<Result<RegisterAccountResponse>>();
        return operation;
    }

    // Else, return a timeout response
    await client.TerminateAsync(instanceId, "Timeout occured");
    return Result<RegisterAccountResponse>.Failure("Timeout");
}

```

### :zap: Fan-out / Fan-in pattern

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/fanoutfanin.png "Fan-out and Fan-in")

Carry multiple activities in parallel and wait for them to complete so that a decision can be made.

_Example code_

```c#
// Fanout
var checkVisaTask = context.CallActivityAsync<Result>(CheckVisa, checkVisaRequest);
var checkDriverLicenseTask = context.CallActivityAsync<Result>(CheckDriverLicense, checkDriverLicenseRequest);

await Task.WhenAll(checkVisaTask, checkDriverLicenseTask);

// Fan-in
var checkVisaOperation = checkVisaTask.Result;
var checkDriverLicenseOperation = checkDriverLicenseTask.Result;
if (!checkVisaOperation.Status)
{
    return Result<RegisterAccountResponse>.Failure("InvalidVisa");
}

if (!checkDriverLicenseOperation.Status)
{
    return Result<RegisterAccountResponse>.Failure("InvalidDriverLicense");
}
```

### :zap: Monitors

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/monitor.png "Monitor")

The monitor pattern refers to a flexible, recurring process in a workflow. An example is polling until specific conditions are met. 
A timer trigger function interval is static and managing instance lifetimes becomes complex. You can use Durable Functions to create flexible recurrence intervals, manage task lifetimes, and create multiple monitor processes from a single orchestration.

```c#
[FunctionName(nameof(MonitorDocumentsOrchestrator))]
public async Task<Result> MonitorAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var timeout = context.CurrentUtcDateTime.AddSeconds(10);

    while (context.CurrentUtcDateTime < timeout)
    {
        await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
        
        var operation = await context.CallActivityAsync<Result>(nameof(CheckVisaActivity), new CheckVisaRequest
        {
            PassportNo = "ABC123456"
        });

        if (operation.Status)
        {
            return operation;
        }

        await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
    }

    return Result.Failure("LateDocumentSubmission");
}
```

### :zap: Eternal (conditionally if required) orchestrators

Eternal orchestrations are orchestrator functions that never end. 
They are useful when you want to use Durable Functions for aggregators and any scenario that requires an infinite loop.

* Orchestration history and performance

Orchestration history
As explained in the orchestration history topic, the Durable Task Framework keeps track of the history of each function orchestration. 
This history grows continuously as long as the orchestrator function continues to schedule new work. 
If the orchestrator function goes into an infinite loop and continuously schedules work, this history could grow critically large and cause significant performance problems. 
The eternal orchestration concept was designed to mitigate these kinds of problems for applications that need infinite loops.

```c#
[FunctionName(nameof(CheckDocumentsOrchestrator))]
public async Task<Result> CheckDocumentsAsync([OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var request = context.GetInput<CheckDocumentsRequest>();
    var operation = await context.CallActivityAsync<Result<int>>(nameof(CheckDocumentsActivity), request);
    if (!operation.Status)
    {
        await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(2), CancellationToken.None);
        context.ContinueAsNew(request);
        return Result.Failure(operation.ErrorCode);
    }

    var remainingDocumentCount = operation.Data;
    if (remainingDocumentCount != 0)
    {
        var RemainingCheckDocumentRequest = new CheckDocumentsRequest
        {
            DocumentCount = remainingDocumentCount
        };
        
        await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(5), CancellationToken.None);
        context.ContinueAsNew(RemainingCheckDocumentRequest);
        return Result.Success();
    }

    return Result.Success();
}
```

### :zap: Human interaction

The name speaks for itself! :trollface:

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/approval.png "Human interaction")

### :zap: Durable functions behind the scenes

* The default configuration for Durable Functions stores this runtime state in an Azure storage account.
* All function execution is driven by `Azure Storage queues`.
* Orchestration and entity status and history is stored in `Azure table storage`.
* Azure Blobs and blob leases are used to distribute orchestration instances and entities across multiple app instances.

:bulb: __History Table__

* The History table is an Azure Storage table that contains the history events for all orchestration instances within a task hub.
  

* Partition key is the orchestrator instance id and the row key is a sequence number.

  
* When an orchestration instance needs to run, the corresponding rows of the History table are loaded into memory.
  Then these history events are then replayed into the orchestrator function code to get it back into its previously check-pointed state.

:bulb: __Instances Table__

* The Instances table contains the statuses of all orchestration and entity instances within a task hub. 
  
* The partition key of this table is the orchestration instance ID or entity key and the row key is an empty string. There is one row per orchestration or entity instance.

* This table is used to satisfy instance query requests from code as well as status query HTTP API calls. 
  It is kept eventually consistent with the contents of the History table mentioned previously. It's influenced by CQRS principles.


:bulb: __Work item queue__

* There is one work-item queue per task hub in Durable Functions. 
  
* This queue is used to trigger stateless activity functions by dequeueing a single message at a time. 
  
* Each of these messages contains activity function inputs and additional metadata, such as which function to execute. 
  
* When a Durable Functions application scales out to multiple VMs, these VMs all compete to acquire tasks from the work-item queue.

:bulb: __Control queue__

* There are multiple control queues per task hub in Durable Functions. A control queue is more sophisticated than the simpler work-item queue. 
  
* Control queues are used to trigger the stateful orchestrator and entity functions. 
  
* Because the orchestrator and entity function instances are stateful singletons, it's important that each orchestration or entity is only processed by one worker at a time. 
  To achieve this constraint, each orchestration instance or entity is assigned to a single control queue. 
  These control queues are load balanced across workers to ensure that each queue is only processed by one worker at a time.


## :zap: Azure durable function runtime status

These are the Azure durable orchestrator function status

* Cancelled
* Completed
* ContinuedAsNew
* Failed
* Pending
* Running
* Terminated

https://docs.microsoft.com/en-us/javascript/api/durable-functions/orchestrationruntimestatus?view=azure-node-latest
  
### :zap: References
:white_check_mark:]  Azure durable functions overview
  
https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp

:white_check_mark:]  Azure durable functions concepts

https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-types-features-overview

:white_check_mark:]  Azure durable functions performance and scale

https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-perf-and-scale

:white_check_mark:]  Durable functions HTTP API reference

https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-http-api

:white_check_mark:]  Azure functions best practices

https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices



  