using Iot.Device.Common;
using Iot.Device.DHTxx.Esp32;
using nanoFramework.Device.OneWire;
using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace nf_DHT11
{
    public class Program
    {
        public static void Main()
        {
            // Memo:
            // To install the firmaware flash tool :
            //              dotnet tool install --global nanoff 
            //              dotnet tool install nanoff --tool-path c:\a-plain-simple-path-to-install-the-tool
            //
            // list the COM ports : 
            //              nanoff --listports
            //
            // update firmware:
            //              nanoff --update --platform esp32 --serialport COM7 --baud 115200
            //              nanoff --update --target ESP32_REV0 --serialport COM7 --baud 115200

            // Chip is ESP32-D0WDQ6 (revision 1)
            Debug.WriteLine("let'read how hot it is!");

            // The DHT11 is an inexpensive 4-pin (one pin is unused) temperature and humidity sensor with a unique characteristic,
            // it communicates on one wire.
            // The sensor can operate between 3 and 5.5V DC and communicates using its own proprietary OneWire protocol.
            // This protocol requires very precise timing to get the data from the sensor.
            // The LOW and HIGH bits are coded on the wire by the length of time the signal is HIGH.
            // The total time to take a reading is at most 23.4 ms.
            // This includes an 18 ms delay required to start the data transfer and a window of up to 5.4 ms for the data.
            // Individual signals can be as short as 20 μs and as long as 80 μs.
            // Since nanoframework is not a real-time operating system it is incapable of reading this sensor.
            // The timing mechanism limitations (timing is not precise enough)
            // and the existence of multi-threading (your thread can be pre-empted at any time during the read operation)
            // prevents the sensor from being read.



            // 1-Wire : https://github.com/nanoframework/nanoFramework.Device.OneWire
            // 1 - Wire Protocol Circuit
            // Simply connect your DHTxx data pin to GPIO12 and GPIO14, the ground to the ground and the VCC to +3.3V.
            //    Dht11's  constructor parameters:
            //        Int32 pinEcho : The pin number which is used as echo (GPIO number)
            //        Int32 pinTrigger : The pin number which is used as trigger (GPIO number)
            //        PinNumberingScheme pinNumberingScheme : The GPIO pin numbering scheme
            //        GpioController  gpioController : GpioController related with operations on pins
            //        Boolean     shouldDispose :  true to dispose the GpioController


            Int32 pinEcho = Gpio.IO12; //12
            Int32 pinTrigger = Gpio.IO14; //or 24


            // https://github.com/nanoframework/nanoFramework.Device.OneWire
            // Important: If you're using an ESP32 device it's mandatory to configure the UART2 pins
            // before creating the OneWireHost.
            // To do that, you have to add a reference to nanoFramework.Hardware.ESP32.
            // In the code snnipet below we're assigning GPIOs 16 and 17 to UART2.
            // Configure pins 12 and 14 to be used in UART2
            Configuration.SetPinFunction(pinEcho, DeviceFunction.COM2_RX); //input/read => high state
            Configuration.SetPinFunction(pinTrigger, DeviceFunction.COM2_TX); //output => low state

            OneWireHost _OneWireHost = new OneWireHost();

            // To get a list with the serial number of all the 1 - Wire devices connected to the bus:
            // ArrayList with the serial numbers of all devices found.
            ArrayList deviceList = _OneWireHost.FindAllDevices();

            //List and display all the devices
            foreach (byte[] device in deviceList)
            {
                string serial = "";

                foreach (byte b in device)
                {
                    serial += b.ToString("X2");
                }

                Console.WriteLine($"{serial}");
            }

            // let's take the first device
            _OneWireHost.FindFirstDevice(true, false);

            // schema : http://itelitesblog.com/hejiale010426/p/15848574.html
            // ref: https://www.nuget.org/packages/nanoFramework.IoT.Device.Dhtxx.Esp32/
            // https://github.com/nanoframework/nanoFramework.IoT.Device/blob/develop/devices/Dhtxx.Esp32/README.md
            // The DHT sensors are very sensitive, avoid too long cables, electromagnetic perturbations and
            // compile the code as release not debug to increase the quality of measurement.
            // 1-Wire Protocol Circuit:
            //  Simply connect your DHTxx data pin to GPIO12 and GPIO14, the ground to the ground and the VCC to +3.3V.


            using (Dht11 dht = new Dht11(pinEcho, pinTrigger))
            {

                while (true)
                {
                    var temperature = dht.Temperature;// Get temperature
                    var humidity = dht.Humidity;// Get humidity percentage

                    if (dht.IsLastReadSuccessful) // is it successful
                    {
                        Debug.WriteLine($"temperature: {temperature.DegreesCelsius} \u00b0C , humidity percentage: {humidity.Percent}% ");
                        //Debug.WriteLine($"temperature: {temperature.DegreesCelsius} \u00b0C ({temperature.Value} {temperature.Unit}), humidity percentage: {humidity.Percent}% ({humidity.Value } {humidity.Unit})");

                        // WeatherHelper supports more calculations, such as saturated vapor pressure, actual vapor pressure and absolute humidity.
                        Debug.WriteLine(
                            $"Heat index: {WeatherHelper.CalculateHeatIndex(temperature, humidity).DegreesCelsius:0.#} \u00B0C");
                        Debug.WriteLine(
                            $"Dew point: {WeatherHelper.CalculateDewPoint(temperature, humidity).DegreesCelsius:0.#} \u00B0C");

                    }
                    else
                    {
                        Debug.WriteLine("reading DHT sensor error");
                    }

                    //Don't try to get more measures than once every 2 seconds.
                    Thread.Sleep(2525);
                }


            }

        }
    }
}
