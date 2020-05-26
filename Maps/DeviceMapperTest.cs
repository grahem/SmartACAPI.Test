using System;
using SmartACDeviceAPI.Models;
using System.Linq;
using Xunit;

namespace SmartACDeviceAPI.Maps
{
    public class DeviceMapperTest
    {
        public DeviceMapperTest()
        {
        }

        [Fact, Trait("Category", "Test")]
        public void Test_Map(){

            var device = new Device();
            device.FirmwareVersion = "123";
            device.InAlarm = true;
            device.RegistrationDate = DateTime.UtcNow.ToString("s");
            device.SerialNumber = "123";
            device.Status = "healthy";

            var mappedDevice = DeviceMapper.MapDevice(device);

            var type = mappedDevice.GetType();
            type.GetProperties().ToList().ForEach(property => 
            {
                var deviceProp = device.GetType().GetProperty(property.Name);
                Assert.True(property.GetValue(mappedDevice).Equals(deviceProp.GetValue(device)));
            });

        }

    }

}