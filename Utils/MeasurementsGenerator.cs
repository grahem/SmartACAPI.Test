using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SmartACDeviceAPI.Models;
using Xunit;
using Xunit.Abstractions;

namespace SmartACDeviceAPI.Test.Utils
{

    public class MeasurementsGenerator
    {

        private readonly ITestOutputHelper _output;

        public MeasurementsGenerator(ITestOutputHelper output)
        {
            _output = output;
        }
        public static List<Measurement> Generate(string serialNumber, int count)
        {


            var measurements = new List<Measurement>();
            for (int i = 0; i < count; i++)
            {
                var random = new Random(7);
                Measurement m = new Measurement();
                m.AirHumidity = random.Next(30, 60);
                m.CarbonMonoxide = random.Next(50, 100);
                m.DeviceSerialNumber = serialNumber;
                m.Id = Guid.NewGuid().ToString();
                m.RecordedTime = DateTime.UtcNow.ToString("s");
                m.Temperature = m.CarbonMonoxide = random.Next(10, 50);
                measurements.Add(m);
            }
            return measurements;
        }

        [Fact, Trait("Category", "Generate")]
        public void Generate_28_123_Measurements()
        {
            var measurements = Generate("123", 28);
            var file = @"../../../Data/Measurements.json";
            WriteDataFile.WriteData(file, measurements);
            Assert.True(WriteDataFile.FileExisits(file));
        }
    }

}