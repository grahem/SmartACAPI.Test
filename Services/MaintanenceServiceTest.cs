using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Moq;
using SmartACDeviceAPI.Models;
using SmartACDeviceAPI.Services;
using Xunit;

namespace SmartACDeviceAPI.Test
{
    public class MaintenanceServiceTest {
        private readonly Mock<IDynamoDBContext> context;
        private readonly MaintenanceService service;

        public MaintenanceServiceTest() {
            context = new Mock<IDynamoDBContext>();
            service = new MaintenanceService(context.Object);

        }

        [Fact, Trait("Category", "Test")]
        public async void Check_Matainence_Mode() {

            context.Setup(expression => expression.LoadAsync<SystemConfig>("InMaintenance", It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<SystemConfig>(null));

            Assert.False(await service.IsInMaintananceMode());

            context.Setup(expression => expression.LoadAsync<SystemConfig>("InMaintenance", It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<SystemConfig>(new SystemConfig { ConfigKey = "InMaintenance", ConfigValue = "true" }));

            Assert.True(await service.IsInMaintananceMode());
        }
    }

}