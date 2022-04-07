# Benchmark Exception
Small test designed to see the impact, in term of performance, of exception handling in .net versus simple error code / invalid value return.

### How to use
```bash
Starting 1,000,000 iterations...
Just launch the application, preferably not in debug mode. The execution may take a few minutes.
+---------------------------------------------------------+-------------+----------------------+
|                         Scenario                        | Direct (ms) | Nested 10 times (ms) |
+---------------------------------------------------------+-------------+----------------------+
| Standard exceptions                                     | 4,925       | 16,207               |
| Reusing same exception                                  | 4,283       | 15,108               |
| Custom exception overriding the stackTrace              | 5,875       | 15,571               |
| Reusing same custom exception overriding the stackTrace | 4,635       | 15,421               |
| Using error code return                                 | 3           | 48                   |
+---------------------------------------------------------+-------------+----------------------+

Finished.
```

### How to read the results
#### Scenario
* **Standard exceptions**: just create and throw an exception in a try catch 
* **Reusing same exception**: create an exception before running the numerous iterations, always throwing the same object
* **Custom exception overriding the stackTrace**: create and thow a custome exception. This exception override the StackTrace getter in an hope to limit the cost of collecting the stack trace, since I did not find any option to prevent this, like [you can do in Java](https://www.baeldung.com/java-exceptions-performance#4-throwing-an-exception-without-adding-the-stack-trace).
* **Reusing same custom exception overriding the stackTrace**: same idea than previously, created a unique exception an throws it multiple times
* **Using error code return**: finally, a simple method that return an error code instead of throwing an Exception, in order to do the comparaison

#### Direct / Nested
* **Direct**: means that the called method for the benchmark directly throw the exception / return the error code
* **Nested**: means that 10 calls are linked before any exceptions or error code are sent. This an aptempt to populate the stack trace, in order to see its impact

### Why

First tried to reproduce an original problem of tranformation from a technical exception to a domain exception, to see the performance difference between handling two exeptions vs one + null verification.

Tried it with [Benchmark DotNet](https://github.com/dotnet/BenchmarkDotNet) (left the BenchmarckDotNet file as an archive of this try), but it appears that this lib does [not support the usage of exceptions](https://github.com/dotnet/BenchmarkDotNet/issues/373), even if they are catched.

Finaly I just settled on running a small test with a code sample from [stack overflow](https://stackoverflow.com/questions/891217/how-expensive-are-exceptions-in-c) and expended it.

As a note, here are some official documentations about exception in C#:
* [Best practices for exceptions](https://docs.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)
* [Exceptions and Performance](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/exceptions-and-performance#try-parse-pattern)