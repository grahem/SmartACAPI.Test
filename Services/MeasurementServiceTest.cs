using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Moq;
using SmartACDeviceAPI.Services;
using Xunit;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using SmartACDeviceAPI.Models;
using System.IO;
using System.Text.Json;
using System.Runtime.Serialization;

namespace SmartACDeviceAPI.Test
{

    public class MeasurementServiceTest
    {

        private readonly Mock<IDynamoDBContext> context;
        private readonly Mock<IAmazonDynamoDB> db;
        private readonly Mock<DeviceAlarmRecorder> alarmRecorder;
        private readonly MeasurementService service;

        public MeasurementServiceTest()
        {
            context = new Mock<IDynamoDBContext>();
            db = new Mock<IAmazonDynamoDB>();
            alarmRecorder = new Mock<DeviceAlarmRecorder>();
            var logger = new Mock<ILogger<MeasurementService>>();

            service = new MeasurementService(context.Object, db.Object, alarmRecorder.Object, logger.Object);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Get_Bad_Serial_Number()
        {
            var emptyQueryResponse = new QueryResponse();
            db.Setup(expression => expression.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<QueryResponse>(emptyQueryResponse));

            var response = await service.GetMeasurements("Bad Serial");
            Assert.True(response.Count == 0);

        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Get_Good_Serial_Number()
        {
            var queryResponse = new QueryResponse();
            queryResponse.Items = Enumerable.Repeat<Dictionary<string, AttributeValue>>(new Dictionary<string, AttributeValue>(), 1).ToList();

            db.Setup(expression => expression.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<QueryResponse>(queryResponse));

            var response = await service.GetMeasurements("Good Serial");
            Assert.True(response.Count == 1);
        }

        [Fact, Trait("Category", "Test")]
        public async void Test_Exception()
        {
            db.Setup(expression => expression.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<QueryResponse>(null));

            await Assert.ThrowsAsync<NullReferenceException>(() => service.GetMeasurements("Good Serial"));
        }

        [Fact, Trait("Category", "Test")]
        public void Test_Registration()
        {

            var logger = new Mock<ILogger<DeviceAlarmRecorder>>();
            DeviceAlarmRecorder recorder = new DeviceAlarmRecorder(context.Object, logger.Object);

                        List<Measurement> measurements = null;
            var file = string.Format(@"../../../Data/321_Measurements.json");
            if (File.Exists(file))
            {
                var lines = File.ReadAllText(file);
                measurements = JsonSerializer.Deserialize<List<Measurement>>(lines);

                long carbonMonoxideCounter = measurements.LongCount(measurements => measurements.CarbonMonoxide > DeviceAlarmRecorder.CARBON_MONOXIDE_ALARM_LEVEL);
                if (carbonMonoxideCounter > 0) {
                    var device = new Device();
                    context.Setup(expression => expression.LoadAsync<Device>("123", It.IsAny<CancellationToken>())).Returns(Task.FromResult<Device>(device));
                    recorder.RecordAlarms("321", measurements);
                    context.Verify(expression => expression.SaveAsync<Device>(device, It.IsAny<CancellationToken>()), Times.AtMost(1));
                } 
            }
        }

    }
}
