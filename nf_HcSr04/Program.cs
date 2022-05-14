using System;
using System.Diagnostics;
using Iot.Device.Hcsr04.Esp32;
using nanoFramework.Hardware.Esp32;
using UnitsNet;
using nf_Utils;

namespace Test
{
    public class Program
    {
        private static readonly int pinTrigger = Gpio.IO13;
        private static readonly int pinEcho = Gpio.IO12;

        /// <summary>
        /// This example shows how to use change default pins for devices and to use the Sleep methods in the 
        /// nanoFramework.Hardware.Esp32 nuget package.
        /// 
        /// Putting the Esp32 into deep sleep mode and starting after timer expires.
        /// 
        /// Other examples of Deep sleep are commented out
        /// </summary>
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");



            //Configuration.SetPinFunction(pinTrigger, DeviceFunction.???);
            //Configuration.SetPinFunction(pinEcho, DeviceFunction.I2C1_CLOCK);


            Led.blink(125, 125, 5);
            Led.blink(525, 1000);

            using (var sonar = new Hcsr04(pinTrigger, pinEcho))
            {
                while (true)
                {
                    if (sonar.TryGetDistance(out Length distance))
                    {
                        Debug.WriteLine($"Distance: {distance.Centimeters} cm");
                        int tps = (int)Math.Round(distance.Centimeters * 5);
                        Led.blink(tps, tps);
                    }
                    else
                    {
                        Debug.WriteLine("Error reading sensor");
                    }

                    // Thread.Sleep(100);
                }

            }

        }
    }
}
