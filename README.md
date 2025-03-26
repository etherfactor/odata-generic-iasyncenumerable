# odata-generic-iasyncenumerable

This issue was reproduced against Microsoft.AspNetCore.OData version 9.2.1.

## Issue Replication

1. Run the application.
2. Perform a GET request to https://localhost:7162/api/v1/entities?variant=Typed
3. An error will be returned in the console, and the response will terminate.
4. Perform a GET request to https://localhost:7162/api/v1/entities?variant=Generic
5. An error will be returned in the console, and the response will terminate.
6. Perform a GET request to https://localhost:7162/api/v1/entities?variant=None
7. No error will be returned. However, the serializer will process the response as an `IEnumerable`, not an `IAsyncEnumerable`.
