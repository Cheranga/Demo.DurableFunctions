## Azure Durable Functions

## TODO

- [x] Deploy the function app -> 1 hour
- [x] Fan-in/fan-out -> 1 hour
- [x] Monitor -> 1 hour  
- [ ] Human interaction -> 1 hour
- [ ] Aggregator -> 1.5 hours
- [x] Sub orchestrator -> 1 hour
- [ ] Documentation -> 2 hours
- Eternal (conditional or not) orchestrators -> 1 hour



- Prerequisites
  - [x] Install `Microsoft.Azure.WebJobs.Extensions.DurableTask.Analyzers`
  

- Prerequisites for Rider  
  - [x] Install `Azure Toolkit for Rider` extension


- Prerequisites for VS Code
  - [x] MENTION THE EXTENSIONS

## Azure durable function patterns

### Function chaining
* Asynchronously calling HTTP endpoint

* Synchronously calling HTTP endpoint

* Fan-out / Fan-in pattern

```mermaid
sequenceDiagram
autoNumber
Blob -->> BlobContainer: BLOB
BlobContainer -->> RegisterCustomerAccountClient: triggers
RegisterCustomerAccountClient -->> Orchestrator: start
Orchestrator -->> ReadBlobActivity: read BLOB
ReadBlobActivity -->> Orchestrator: customer list
Orchestrator -->> SubOrchestrator: customer list (portion)
SubOrchestrator -->> ValidateActivity: validate (fan-out)
ValidateActivity -->> SubOrchestrator: validation results (fan-in)
SubOrchestrator -->> PublishRegisterCustomerEvents: publish to ASB (fan-out)
```


## Azure durable function concepts

## Azure durable function runtime status

https://docs.microsoft.com/en-us/javascript/api/durable-functions/orchestrationruntimestatus?view=azure-node-latest


## Comparing Azure Durable Functions

https://docs.microsoft.com/en-us/azure/azure-functions/functions-compare-logic-apps-ms-flow-webjobs#compare-azure-functions-and-azure-logic-apps


  
### References
- [x] Azure durable functions overview
  
https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp

- [x] Azure durable functions concepts

https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-types-features-overview


- [x] Durable functions HTTP API reference

https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-http-api

- [x] Azure functions best practices

https://docs.microsoft.com/en-us/azure/azure-functions/functions-best-practices


  