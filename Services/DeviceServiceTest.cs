using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using Moq;
using SmartACDeviceAPI.Models;
using SmartACDeviceAPI.Services;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using Amazon.DynamoDBv2.Model;

namespace SmartACDeviceAPI.Test
{

    public class DeviceServiceTest{

        readonly DeviceService service;
        private readonly Mock<IDynamoDBContext> context;

        private readonly Mock<IAmazonDynamoDB> db;

        private readonly Device goodDevice = new Device{SerialNumber = "Good"};

        private readonly Device badDevice = new Device{SerialNumber = "Bad"};

        public DeviceServiceTest() {
            context = new Mock<IDynamoDBContext>();
            db = new Mock<IAmazonDynamoDB>();
            var logger = new Mock<ILogger<DeviceService>>(); 

            service = new DeviceService(context.Object, db.Object, logger.Object);

            
        } 

        [Fact, Trait("Category", "Test")]
        public async void Test_Device_Not_Exists() {
            context.Setup(expression => expression.LoadAsync<Device>(badDevice.SerialNumber, It.IsAny<CancellationToken>()))
                   .Returns(() => Task.FromResult<Device>(null));

            var device = await service.GetDeviceBySerialNumber(badDevice.SerialNumber);
            Assert.True(device == null);
        }

        [Fact, Trait("Category", "Test")]
        public void Test_Device_Exists() {
            context.Setup(expression => expression.LoadAsync<Device>(goodDevice.SerialNumber, It.IsAny<CancellationToken>()))
                   .Returns(() => Task.FromResult<Device>(goodDevice));
            var response = new DeviceServiceResponse { SerialNumber = goodDevice.SerialNumber };
            Assert.True(service.GetDeviceBySerialNumber(goodDevice.SerialNumber).Result.SerialNumber == response.SerialNumber);        
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Get_Devices_Returns_Empty_Devices()
        {
            context.Setup(expression => expression.QueryAsync<Device>(It.IsAny<string>(), It.IsAny<DynamoDBOperationConfig>()))
                .Returns(() => null);
            var response = await service.GetDevices(100);
            Assert.True(response.Count == 0);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Get_Devices_Returns_100() {
            var queryResponse = new QueryResponse();
            queryResponse.Items = Enumerable.Repeat<Dictionary<string, AttributeValue>>(new Dictionary<string, AttributeValue>(), 50).ToList(); 
            new List<Dictionary<string, AttributeValue>>();
            db.Setup(expression => expression.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                   .Returns(() => Task.FromResult(queryResponse));
            var result = await service.GetDevices(100);
            Assert.True(result.Count == 100);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Get_Devices_Limits_Return_200() {
            var queryResponse = new QueryResponse();
            queryResponse.Items = Enumerable.Repeat<Dictionary<string, AttributeValue>>(new Dictionary<string, AttributeValue>(), 5000).ToList(); 
            new List<Dictionary<string, AttributeValue>>();
            db.Setup(expression => expression.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                   .Returns(() => Task.FromResult(queryResponse));
            var result = await service.GetDevices(5000);
            Assert.True(result.Count == 200);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Register_Bad_Device_Null()
        {
            Device badDevice = new Device();
            context.Setup(expression => expression.SaveAsync<Device>(badDevice, It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<Device>(null));
            var response = await service.RegisterDevice(badDevice);
            Assert.True(response == null);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Register_Good_Device()
        {
            Device badDevice = new Device();
            
            context.Setup(expression => expression.SaveAsync<Device>(goodDevice, It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<Device>(goodDevice));
            context.Setup(Expression => Expression.LoadAsync<Device>(goodDevice.SerialNumber, It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<Device>(goodDevice));
            
            var response = await service.RegisterDevice(goodDevice);
            Assert.True(!String.IsNullOrEmpty(response.SerialNumber));
        }
    }


}