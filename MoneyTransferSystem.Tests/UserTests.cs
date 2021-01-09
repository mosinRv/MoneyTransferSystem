using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using MoneyTransferSystem.Database.DbModels;
using NUnit.Framework;

namespace MoneyTransferSystem.Tests
{
    public class UserTests: MyTestBase
    {
        
        
        [Test]
        public void GetUsers()
        {
            // Arrange
            // Act
            HttpResponseMessage response =AdminClient.GetUsers().GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Assert.IsInstanceOf<List<User>>(response.Content);
        }

        [Test]
        public void GetUserById()
        {
            // Arrange
            // Act
            HttpResponseMessage response =AdminClient.GetUser(8).GetAwaiter().GetResult();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK,response.StatusCode);
            // Assert.IsInstanceOf<User>(response.Content);
            // something with jsonconverter deserialize
        }
        [Test]
        public void GetNonExistentUserById()
        {
            // Arrange
            const int nonExistentUserId = 10;
            // Act
            HttpResponseMessage response =AdminClient.GetUser(nonExistentUserId).GetAwaiter().GetResult();
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
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