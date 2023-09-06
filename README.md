[![EfAudit ci](https://github.com/saturn72/EfAuditTrail/actions/workflows/cont-delivery.yml/badge.svg)](https://github.com/saturn72/EfAuditTrail/actions/workflows/cont-delivery.yml)
# EfAuditTrail
`Entity Framework` AuditTrail package in your `C#` project


## Usage (see `Sample` directory)
### Setup
  1. Add `EfAudit` to you app's services:
  https://github.com/saturn72/EfAuditTrail/blob/7342af4b0051866c1387175fc56728ce4f198aac/Sample/Server/Program.cs#L9-L18
  3. Add `EfAuditInterceptor` to you `Dbcontext`
  https://github.com/saturn72/EfAuditTrail/blob/7342af4b0051866c1387175fc56728ce4f198aac/Sample/Server/Program.cs#L19-L27
  
  
## Define Audit Trail Handler
Audit Trail handler is an object of type `Func<IServiceProvider, AuditRecord, CancellationToken, Task>`.
In your code define a handler of the type, and add it to the EfAudit (see [Setup](#setup) ). 

### Example:
https://github.com/saturn72/EfAuditTrail/blob/614cc559896ca1a5d5af2ef857ecd0fda02f91ae/Sample/Server/Controllers/AuditTrailController.cs#L22-L33


# Undestand the problem
The problem that the `EfAudit` package tries to solve is how to monitor all databases changes (insert, update and delete operations). It utilizes the `interception` capabilities that `EFCore` provides to create standard way to expose data table changes.
Utilizing `EfCore` mechansims are not the only way this problem can be solved, and this section is dedicated to go over some other options to solve this issue.

## (Some) Auditing options

[Data Access Layer Interception](#Data-Access-Layer-Interception)

[Business Logic Explicit Audit Declaraion](#Business-Logic-Explicit-Audit-Declaraion)

[Database Built-in Functionality](#Database-Built-in-Functionality)

[Managed Service](#Managed-Service)


### Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception
Data Access Layer Interception

### Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion
Business Logic Explicit Audit Declaraion


### Database Built-in Functionality
Database Built-in Functionality
Database Built-in Functionality
Database Built-in Functionality
Database Built-in Functionality
Database Built-in Functionality

### Managed Service
Managed Service
Managed Service
Managed Service
Managed Service
Managed Service
Managed Service
