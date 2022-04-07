using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace BenchmarkException
{
    public class ExceptionTransformationVsNull
    {
        private readonly UserUseCase _useCaseException = new UserUseCase(new RepositoryWithException());
        private readonly UserUseCase _useCaseNull = new UserUseCase(new RepositoryWithNull());
        
        public class DataBase
        {
            public object Find(int id) => throw new NotFoundException();

            public object? FindOrDefault(int id) => null;
        }

        public interface IUserRepository
        {
            public object FindUser(int userId);
        }
        

        public class RepositoryWithException : IUserRepository
        {
            private readonly DataBase _db = new DataBase();

            public object FindUser(int userId)
            {
                try
                {
                    return _db.Find(userId);
                }
                catch (NotFoundException)
                {
                    throw new UnknownUserException();
                }
            }
        }

        public class RepositoryWithNull : IUserRepository
        {
            private readonly DataBase _db = new DataBase();

            public object FindUser(int userId)
            {
                var user = _db.FindOrDefault(userId);
                if(user == null)
                {
                    throw new UnknownUserException();
                }
                return user;
            }
        }

        public class UserUseCase
        {
            private readonly IUserRepository _repo;

            public UserUseCase(IUserRepository repo)
            {
                _repo = repo;
            }

            public void DoUserRelatedAction(int userId)
            {
                try
                {
                    _repo.FindUser(userId);
                }
                catch (NotFoundException)
                {
                    // catch it for the benchmark
                }
            }
        }

        [Benchmark]
        public void ExceptionHandling() => _useCaseException.DoUserRelatedAction(1);

        [Benchmark]
        public void NullHandling() => _useCaseNull.DoUserRelatedAction(1);

        // technical Exception
        internal class NotFoundException : Exception
        {

        }

        // domain Exception
        internal class UnknownUserException : Exception
        {

        }
    }

    public class BenchmarckDotNet
    {
        //public static void Main(string[] args)
        //{
        //    var config = new ManualConfig()
        //        .WithOptions(ConfigOptions.DisableOptimizationsValidator)
        //        .AddValidator(JitOptimizationsValidator.DontFailOnError)
        //        .AddLogger(ConsoleLogger.Default)
        //        .AddColumnProvider(DefaultColumnProviders.Instance);

        //    var summary = BenchmarkRunner.Run<ExceptionTransformationVsNull>(config);
        //}
    }
}