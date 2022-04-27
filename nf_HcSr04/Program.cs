using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Hcsr04.Esp32;
using nanoFramework.Hardware.Esp32;
using UnitsNet;

namespace Test
{
    public class Program
    {
        private static readonly int pinTrigger = Gpio.IO13;
        private static readonly int pinEcho = Gpio.IO12;
        private static readonly int pinLed = Gpio.IO02;

        // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
        private static GpioPin led;
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


            flashLed(125, 125, 5);
            flashLed(525, 1000);

            using (var sonar = new Hcsr04(pinTrigger, pinEcho))
            {
                while (true)
                {
                    if (sonar.TryGetDistance(out Length distance))
                    {
                        Debug.WriteLine($"Distance: {distance.Centimeters} cm");
                        int tps = (int)Math.Round(distance.Centimeters * 5);
                        flashLed(tps, tps);
                    }
                    else
                    {
                        Debug.WriteLine("Error reading sensor");
                    }

                    // Thread.Sleep(100);
                }

            }

        }

        private static void flashLed(int msOn = 200, int msOff = 0, int nb = 1)
        {
            if (led == null)
            {
                led = new GpioController().OpenPin(pinLed, PinMode.Output);
            }
            for (int i = 0; i < nb; i++)
            {
                led.Write(PinValue.High);
                Thread.Sleep(msOn);
                led.Toggle();
                Thread.Sleep(msOff);
            }
            led.Write(PinValue.Low);
        }
    }
}
