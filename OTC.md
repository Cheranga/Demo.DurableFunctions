### :zap: Human interaction

Let's design an OTC sending requirement where the system sends a code to the user and waits until the user verifies it.
- The system will wait for 60 seconds for the user to respond back.
- User will be given 3 attempts to enter the correct code.

![alt text](https://github.com/Cheranga/Demo.DurableFunctions/blob/feature/Demo/Images/otcflow.png "otc")

<details>
<summary>Mermaid diagram</summary>
```mermaid
sequenceDiagram
autoNumber

system -->> user: sends OTC
system -->> system: waits for user input
note over system: waits for an event
user -->> system: verifies OTC
alt is it expired?
system -->> user: send SMS (expired)
else
alt are the codes matching?
system -->> user: send sms (thanks)
system -->> database: update user (set verified flag)    
else
alt maximum attempts reached?
system -->> user: send sms (sorry)
else
system -->> user: send sms (resend OTC)
note right of system: repeat from step 2
end
end
end
\```
</details>
