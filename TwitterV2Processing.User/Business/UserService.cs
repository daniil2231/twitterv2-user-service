﻿using Confluent.Kafka;
using MongoDB.Driver;
using PasswordHashing;
using TwitterV2Processing.User.Models;
using TwitterV2Processing.User.Persistence;

namespace TwitterV2Processing.User.Business
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ProducerConfig producerConfig;

        public UserService(IUserRepository userRepository)
        {
            producerConfig = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
            };
            _userRepository = userRepository;
        }

        public async void DeleteUserAsync(string username)
        {
            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                var message = new { Username = username };
                var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);

                await producer.ProduceAsync("user_deletion", new Message<string, string> { Value = messageJson });
            }
        }

        public Task<DeleteResult> DeleteUser(string username) {
            DeleteUserAsync(username);
            return _userRepository.DeleteUser(username);
        }

        public async void CreateUserAsync(string username, string password, string role)
        {
            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                var message = new { Username = username, Password = password, Role = role };
                var messageJson = Newtonsoft.Json.JsonConvert.SerializeObject(message);

                await producer.ProduceAsync("user_creation", new Message<string, string> { Value = messageJson });
            }
        }

        public Task<UserModel> CreateUser(UserModel user)
        {
            UserModel userWithEncryptedPass = new UserModel(user.Id, user.Username, PasswordHasher.Hash(user.Password), user.Role, user.Followers, user.Following);
            CreateUserAsync(userWithEncryptedPass.Username, userWithEncryptedPass.Password, userWithEncryptedPass.Role);
            return _userRepository.CreateUser(userWithEncryptedPass);
        }

        public Task<UserModel> GetByUsername(string username)
        {
            return _userRepository.GetByUsername(username);
        }

        public Task<List<UserModel>> GetUsers()
        {
            return _userRepository.GetAllUsers();
        }
    }
}
