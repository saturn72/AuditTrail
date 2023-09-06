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


# Discussion

The problem that the `EfAudit` package tries to solve is how to monitor all databases changes (insert, update and delete operations). It utilizes the `interception` capabilities that `EFCore` provides to create standard way to expose data table changes.
Utilizing `EfCore` mechansims are not the only way this problem can be solved, and this section is dedicated to go over some other options to solve this issue.

## Requirements
We would like to set guideline for auditing all data changes in database. 

Once application data-table changes (added or modified) we would like to record the change and eventually delivered the entity' lifecycle (audit-trail).

We would like to gain access to the change metadata as well (who perfomed the change, when, which client was used etc.)

## Inspected Auditing options

[Data Access Layer Wrapping](#Data-Access-Layer-Wrapping)

[Business Logic Explicit Audit Declaraion](#Business-Logic-Explicit-Audit-Declaraion)

[Database Built-in Functionality](#Database-Built-in-Functionality)

[Managed Service](#Managed-Service)


### Data Access Layer Wrapping 
see: [N-Tier Architecture](https://en.wikipedia.org/wiki/Multitier_architecture)

This option wraps the `DAL`'s unit-of-work with the audit logic.

It is executed on application runtime, and is not part of the functional aspects of the database
As such, it is easier to add runtime execution aspects to the audit trail (identity aspects, hosting, ip, etc).

Using this option locates the audit-trail creation management at the lowest _runtime_ layer of data management.

```
_Pseudo code: DAL_
  1. insert_new_entity(values-to-insert, execution-context)
  2.   new-entry = unit_of_work.insert(values-to-insert)
  3.   add_audit_trail_entrye(new-entry, execution-context)
```
The pseudo above describes the concept of wrapping the unit-of-work for insertion operation

If execution ceased after line #2 was successfully performed, the audit trail entry is not recorded 

For this there are 2 possible solutions:
* Wrap both functions in same atomic context (transaction)
    _Notes:_
    * While garuntees atomicity, we may not want to bind runtime dependecy for both functions
    * May not be supported by runtime/database engine
* Add another audit-trail mechanism as a fallback option (one that is not executed in runtime)

```
_Pseudo code: DAL with transaction_
  1.  insert_new_entity(values-to-insert, execution-context)
  2.    open_transaction
  3.    new-entry = unit_of_work.insert(values-to-insert)
  4.    add_audit_trail_entrye(new-entry, execution-context)
  5.    close_transaction
```
__`EfAudit` Package uses this concept by utilizing the built-in interception capabilities `EfCore` provides for relational databases__

### Business Logic Explicit Audit Declaraion
see: [N-Tier Architecture](https://en.wikipedia.org/wiki/Multitier_architecture)

This option declares persistency layer changes within the `Business-Logic` layer. 

It is executed on application runtime, and is not part of the functional aspects of the database.

As such, it is easier to add runtime execution aspects to the audit trail, include business logic specifics

Using this option locates the audit-trail creation management at the middle/highest _runtime_ layer of data management (depends on implementation)

```
_Pseudo code: Business Logic_
  1.  create_new(values-to-insert)
  2.    new-entry = dal.insert(values-to-insert)
  3.    execution_context = get_execution_context()
  4.    declare_created(new-entry, execution-context)
```
The pseudo above describes the concept of adding insertion operation to the business logic

If execution ceased after line #2 was successfully performed, the audit trail entry is not recorded.
Adding another audit-trail mechanism as a fallback option solves this issue

### Database Built-in Functionality
Some db engines have built-in mechanism for reflecting data-table/indexes changes.

This feature may require configuration and may adds some performance issues and/or limitations. Please consult your db provider documentation.

This option monitors data changes in the lowest layer possible and is strongly binded to database features.

It is executed "inside" the database engine and is "pure" functional outcome.

As such, it provides 100% reflection of the changes, but misses any app-level contextual aspects

The research of this aspect is focused on `Microsoft SqlServer`.

`SqlServer` provides built in audit mechanism, see: [Change Data Capture Sql Server](https://learn.microsoft.com/en-us/sql/relational-databases/track-changes/about-change-data-capture-sql-server)

In general, once `CDC` is enabled and configured, tables for holding the audit info are created. From this point these tables needs to querid to fetch the data.

As mentioned prior, the data contained in these tables contains the diff in data, and __may__ contain sql transactional contextual info only (transaction info, timestamps, etc).

_Important:_ The `CDC` feature comes with performance price as it wraps transactions the same concept described in [Data Access Layer Wrapping](#Data-Access-Layer-Wrapping).

### Managed Service
This option refer to managed audit service provided by the persistency layer host. Please consult the host documentation regard implementation and pricing.
