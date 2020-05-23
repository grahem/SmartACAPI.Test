using Moq;
using SmartACDeviceAPI.Services;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartACDeviceAPI.Options;
using SmartACDeviceAPI.Models;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using SmartACDeviceAPI.Exceptions;

namespace SmartACDeviceAPI.Test.Services
{

    public class UserAuthZServiceTest
    {

        private readonly UserAuthZService service;

        private readonly Mock<IDynamoDBContext> dbContext;
        private readonly User badUser = new User { UserName = "Bad", Password = "User" };
        private readonly User tomHanks = new User { UserName = "Tom", Password = "Hanks" };
        public UserAuthZServiceTest()
        {
            this.dbContext = new Mock<IDynamoDBContext>();

            var logger = new Mock<ILogger<UserAuthZService>>();
            var options = new Mock<IOptionsMonitor<AuthZOptions>>();
            options.Setup(options => options.CurrentValue).Returns(new AuthZOptions { SecretKey = "1234567891011121314" });

            this.service = new UserAuthZService(dbContext.Object, logger.Object, options.Object);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Bad_User_Can_Not_Authenticate()
        {

            dbContext.Setup(context => context.LoadAsync<User>(badUser.UserName, It.IsAny<CancellationToken>()))
                     .Returns(() => Task.FromResult<User>(null));
            var val = await service.Authorize(badUser.UserName, badUser.Password);
            Assert.True(val == null);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Bad_Password_Throws_AuthZExcption()
        {
            dbContext.Setup(context => context.LoadAsync<User>(tomHanks.UserName, CancellationToken.None))
                     .Returns(() => Task.FromResult<User>(tomHanks));
            await Assert.ThrowsAsync<AuthZException>(() => service.Authorize(tomHanks.UserName, tomHanks.Password));
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Tom_Hanks_Can_Authorize()
        {
            //ensure good password returns string.
            dbContext.Setup(context => context.LoadAsync<User>(tomHanks.UserName, CancellationToken.None))
                     .Returns(() => Task.FromResult<User>(new User { Password = "7Eaoc/SBvEBg/EdidTDq0moFyG5ER6G3MCgtIxDltKM=" }));
            var val = await service.Authorize(tomHanks.UserName, tomHanks.Password);
            Assert.True(val.Length == 165);
        }

    }
}