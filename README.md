[![EfAudit ci](https://github.com/saturn72/EfAuditTrail/actions/workflows/cont-delivery.yml/badge.svg)](https://github.com/saturn72/EfAuditTrail/actions/workflows/cont-delivery.yml)
# EfAuditTrail
`Entity Framework` AuditTrail solution in your `C#` project


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
