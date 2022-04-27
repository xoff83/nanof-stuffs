using Iot.Device.Common;
using Iot.Device.DHTxx.Esp32;
using System;
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
            Debug.WriteLine("Hello from nanoFramework!");
            while (true)
            {

                // 1-Wire
                // 12, 24 represent needle angle
                //    Dht11's  constructor parameters:
                //        Int32 pinEcho : The pin number which is used as echo (GPIO number)
                //        Int32 pinTrigger : The pin number which is used as trigger (GPIO number)
                //        PinNumberingScheme pinNumberingScheme : The GPIO pin numbering scheme
                //        GpioController  gpioController : GpioController related with operations on pins
                //        Boolean     shouldDispose :  true to dispose the GpioController
                Int32 pinEcho = 14;
                Int32 pinTrigger = 12;

                // schema : http://itelitesblog.com/hejiale010426/p/15848574.html
                // ref: https://www.nuget.org/packages/nanoFramework.IoT.Device.Dhtxx.Esp32/
                // https://github.com/nanoframework/nanoFramework.IoT.Device/blob/develop/devices/Dhtxx.Esp32/README.md
                // The DHT sensors are very sensitive, avoid too long cables, electromagnetic perturbations and
                // compile the code as release not debug to increase the quality of measurement.
                // 1-Wire Protocol Circuit:
                //  Simply connect your DHTxx data pin to GPIO12 and GPIO14, the ground to the ground and the VCC to +3.3V.

                using (Dht11 dht = new Dht11(pinEcho, pinTrigger))
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
                }

                //    //ssd1306.Write(10,10,"Allumé");
                //    led.Toggle();
                //    Thread.Sleep(125);
                //    led.Toggle();
                //    Thread.Sleep(125);
                //    led.Toggle();
                //    Thread.Sleep(125);
                //    led.Toggle();
                Thread.Sleep(2525);
            }
            //Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
        }
    }
}
