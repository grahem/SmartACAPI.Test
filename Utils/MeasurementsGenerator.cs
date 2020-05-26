using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Unicode;
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

                var random = new Random(Guid.NewGuid().ToString().GetHashCode());
                Measurement m = new Measurement();
                m.AirHumidity = random.Next(30, 60);
                m.CarbonMonoxide = random.Next(1, 15);
                m.DeviceSerialNumber = serialNumber;
                m.Id = Guid.NewGuid().ToString();
                m.RecordedTime = DateTime.UtcNow.ToString("s");
                m.Temperature = random.Next(10, 50);
                measurements.Add(m);
            }
            return measurements;
        }

        [Fact, Trait("Category", "Generate")]
        public void Generate_28_321_Measurements()
        {
            var measurements = Generate("321", 28);
            var file = @"../../../Data/321_Measurements.json";
            File.Delete(file);
            WriteDataFile.WriteData(file, measurements);
            Assert.True(WriteDataFile.FileExisits(file));
        }
    }

}