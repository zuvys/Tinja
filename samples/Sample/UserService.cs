﻿using Tinja;
using Tinja.Interception;

namespace ConsoleApp
{
    [Interceptor(typeof(UserServiceDataAnnotationInterceptor), Inherited = true, Priority = -1)]
    public interface IUserService
    {
        string GetUserName(int id);
    }

    public class UserService1 : IUserService
    {
        public UserService1()
        {

        }

        public string GetUserName(int id)
        {
            return "UserService1:Name:" + id;
        }
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        [Inject]
        public IRepository<IUserRepository> Repository { get; set; }

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public string GetUserName(int id)
        {
            return "UserService:Name:" + id;
        }
    }

    public interface IUserRepository
    {

    }

    public class UserRepository : IUserRepository
    {

    }

    public class IRepository<T>
    {

    }

    public class Repository<T> : IRepository<T>
    {
        public T Value { get; }

        public Repository(T value)
        {
            Value = value;
        }
    }
}