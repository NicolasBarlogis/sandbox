using System.Diagnostics;

namespace BenchmarkException
{
    public class CustomBenchmark
    {
        public static readonly int Iterations = 1000000;
        // https://stackoverflow.com/questions/891217/how-expensive-are-exceptions-in-c
        public static void Main(string[] args)
        {
            IList<Tuple<string, long, long>> results = new List<Tuple<string, long, long>>();
            Console.WriteLine("Starting " + $"{Iterations:N0}" + " iterations...\n");

            var stopwatch = new Stopwatch();

            // Exceptions standard
            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestExceptions();
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            var simpleTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestExceptionsNested(10);
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            results.Add(new Tuple<string, long, long>("Standard exceptions", simpleTime, stopwatch.ElapsedMilliseconds));

            // Reusing same exception
            stopwatch.Reset();
            stopwatch.Start();
            var e = new Exception();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    throw e;
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            simpleTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            Exception exception = new Exception();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestSameExceptionNested(10, exception);
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            results.Add(new Tuple<string, long, long>("Reusing same exception", simpleTime, stopwatch.ElapsedMilliseconds));


            // Exception personalized stack trace
            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestExceptionNoStackTrace();
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            simpleTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestExceptionNoStackTraceNested(10);
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            results.Add(new Tuple<string, long, long>("Custom exception overriding the stackTrace", simpleTime, stopwatch.ElapsedMilliseconds));

            // reuse same custom exception
            stopwatch.Reset();
            stopwatch.Start();
            Exception noStackException = new NoStackException();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    throw noStackException;
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            simpleTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                try
                {
                    TestSameExceptionNested(10, noStackException);
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            results.Add(new Tuple<string, long, long>("Reusing same custom exception overriding the stackTrace", simpleTime, stopwatch.ElapsedMilliseconds));


            // basic return codes (for comparison)
            stopwatch.Reset();
            stopwatch.Start();
            int returnCode;
            for (var i = 1; i <= Iterations; i++)
            {
                returnCode = TestReturnCodes();
                if (returnCode == 1)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            simpleTime = stopwatch.ElapsedMilliseconds;


            stopwatch.Reset();
            stopwatch.Start();
            for (var i = 1; i <= Iterations; i++)
            {
                returnCode = TestReturnCodesNested(10);
                if (returnCode == 1)
                {
                    // Do nothing
                }
            }
            stopwatch.Stop();
            results.Add(new Tuple<string, long, long>("Using error code return", simpleTime, stopwatch.ElapsedMilliseconds));
            OutputBeautifully(results);

            Console.WriteLine("\nFinished.");
        }

        private static void TestExceptions()
        {
            throw new Exception("Failed");
        }

        private static void TestExceptionsNested(int loopNumber)
        {
            if(loopNumber == 0)
                throw new Exception("Failed");
            TestExceptionsNested(loopNumber-1);
        }

        private static void TestExceptionNoStackTrace()
        {
            throw new NoStackException();
        }
        private static void TestExceptionNoStackTraceNested(int loopNumber)
        {
            if (loopNumber == 0)
                throw new NoStackException();
            TestExceptionNoStackTraceNested(loopNumber - 1);
        }

        private static void TestSameExceptionNested(int loopNumber, Exception e)
        {
            if (loopNumber == 0)
                throw e;
            TestSameExceptionNested(loopNumber - 1, e);
        }

        private static int TestReturnCodes()
        {
            return 1;
        }

        private static int TestReturnCodesNested(int loopNumber)
        {
            return loopNumber == 0 ? 1 : TestReturnCodesNested(loopNumber - 1);
        }

        private static void OutputBeautifully(IList<Tuple<string, long, long>> results)
        {
            Console.WriteLine("+---------------------------------------------------------+-------------+----------------------+");
            Console.WriteLine("|                         Scenario                        | Direct (ms) | Nested 10 times (ms) |");
            Console.WriteLine("+---------------------------------------------------------+-------------+----------------------+");
            foreach (var result in results)
            {
                var directResult = $"{(result.Item2):N0}";
                var nestedResult = $"{(result.Item3):N0}";
                Console.WriteLine("| "+result.Item1.PadRight(55)+" | "+directResult.PadRight(11) + " | " + nestedResult.PadRight(20) + " |");
            }
            Console.WriteLine("+---------------------------------------------------------+-------------+----------------------+");
        }
    }

    internal class NoStackException : Exception
    {
        public NoStackException() : base() { }

        public override string? StackTrace => "";
    }
}
