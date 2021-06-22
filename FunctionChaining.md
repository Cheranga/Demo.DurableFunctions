## Function chaining, fan-in and fan out with durable functions

### Function chaining

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/functionchaining.png "Function Chaining")

---

### Fan-out/fan-in

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/master/Images/fanoutfanin.png "Fan-out and Fan-in")

---

### Example 01 (Without sub-orchestrators)

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/feature/Demo/Images/demo1.png "Example 01")

<details>
<summary>Mermaid diagram</summary>
```mermaid
sequenceDiagram
autoNumber

user -->> API(client): register account request
API(client) -->> Orchestrator: register account request
note right of Orchestrator: Fan out
Orchestrator -->> CheckVisa: check visa request
Orchestrator -->> CheckLicense: check license request
note right of Orchestrator: Fan in
Orchestrator -->> Orchestrator: wait for validity checks
alt Is eligible?
note right of Orchestrator: function chaining
Orchestrator -->> RegisterCustomer: register customer request
RegisterCustomer -->> Orchestrator: register customer response
alt is valid?
Orchestrator -->> RegisterBankAccount: customer id + register bank account request
RegisterBankAccount -->> Orchestrator: register bank acocunt response
alt is valid?
Orchestrator -->> Orchestrator: Set register customer account response
else
Orchestrator -->> Orchestrator: set error response (register bank account)
end
else
Orchestrator -->> Orchestrator: set error response (register customer)
end
else
Orchestrator -->> Orchestrator: Set error response (eligibility)    
end
\```
</details>



### Example 02

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/feature/Demo/Images/demo2.png "Example 02")

<details>
<summary>Mermaid diagram</summary>
```mermaid
sequenceDiagram
autoNumber

user -->> API(client): register account request
API(client) -->> Orchestrator: register account request
Orchestrator -->> **CheckEligibilityOrchestrator: check eligibility request
note right of **CheckEligibilityOrchestrator: fan-out/fan-in
**CheckEligibilityOrchestrator -->> Orchestrator: check eligiblity response
alt Is eligible?
Orchestrator -->> RegisterCustomer: register customer request
RegisterCustomer -->> Orchestrator: register customer response
alt is valid?
note right of Orchestrator: function chaining
Orchestrator -->> RegisterBankAccount: customer id + register bank account request
RegisterBankAccount -->> Orchestrator: register bank acocunt response
alt is valid?
Orchestrator -->> Orchestrator: Set register customer account response
else
Orchestrator -->> Orchestrator: set error response (register bank account)
end
else
Orchestrator -->> Orchestrator: set error response (register customer)
end
else
Orchestrator -->> Orchestrator: Set error response (eligibility)    
end
\```
</details>

