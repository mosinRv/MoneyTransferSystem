using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MoneyTransferSystem.Database.DbModels;
using MoneyTransferSystem.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class UserTests: MyTestBase
    {
        public override async Task Init()
        {
            await base.Init();
            UserClient = CreateAuthorizedClientAsync("TestUser", "123").GetAwaiter().GetResult();
            AdminClient = CreateAuthorizedClientAsync("TestAdmin", "123").GetAwaiter().GetResult();

        }
        
        [Test]
        public async Task GetUsers()
        {
            // Arrange
            // Act
            HttpResponseMessage response =AdminClient.GetUsers().GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode); 
            var content = JsonConvert.DeserializeObject<IEnumerable<UserDto>>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
        }

        [Test]
        public async Task GetUserById()
        {
            // Arrange
            int id = TestUser1.Id;
            // Act
            HttpResponseMessage response =AdminClient.GetUser(id).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK,response.StatusCode);
            var content = JsonConvert.DeserializeObject<UserDto>(await response.Content.ReadAsStringAsync());
            Assert.IsNotNull(content);
            Assert.AreEqual(TestUser1.Login, content.User);
        }

        [Test]
        public void CreateValidUser()
        {
            // Arrange
            var newUser = new User {Login = "ValidUser", Pass = "123", isAdmin = false};
            // Act
            HttpResponseMessage response =AdminClient.CreateUser(newUser).GetAwaiter().GetResult();
            //var createdResult = response as CreatedAtRouteNegotiatedContentResult<T>
            
            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            //Assert.IsInstanceOf<User>(response.Content);
        }
        
    }
}