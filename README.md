## :zap: Azure Durable Functions :zap:

> _Install `Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers` nuget package. It will guide you potential code violations when implementing Azure durable functions._
  
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

[Read more](FunctionChaining.md)

### :zap: Async HTTP APIs

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/feature/Demo/Images/asynchttpapi.png "async http api")

### :zap: Eternal (conditionally if required) orchestrators

Eternal orchestrations are orchestrator functions that never end. 
They are useful when you want to use Durable Functions for aggregators and any scenario that requires an infinite loop.

* Orchestration history and performance

Orchestration history
As explained in the orchestration history topic, the Durable Task Framework keeps track of the history of each function orchestration. 
This history grows continuously as long as the orchestrator function continues to schedule new work. 
If the orchestrator function goes into an infinite loop and continuously schedules work, this history could grow critically large and cause significant performance problems. 
The eternal orchestration concept was designed to mitigate these kinds of problems for applications that need infinite loops.


### :zap: Human interaction

[Read more](OTC.md)


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



  