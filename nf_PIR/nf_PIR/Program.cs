using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;
using nf_Utils;

namespace nf_PIR
{
    public class Program
    {
        private static readonly int pinPIR = Gpio.IO21;
        private static readonly int pinLED = Gpio.IO02;
        private static GpioController s_GpioController;

        public static void Main()
        {
            Debug.WriteLine("Kick's on nanoFramework!");
            Led.blink(125, 125, 5);
            Led.blink(525, 1000);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T

            using (Iot.Device.Hcsr501.Hcsr501 sensor = new Iot.Device.Hcsr501.Hcsr501(pinPIR))
            {
                while (true)
                {
                    // adjusting the detection distance and time by rotating the potentiometer on the sensor
                    if (sensor.IsMotionDetected)
                    {
                        // turn the led on when the sensor detected infrared heat
                        Led.blink(200, 50, 4);
                        Debug.WriteLine("Detected! Turn the LED on.");
                    }
                    else
                    {
                        // turn the led off when the sensor undetected infrared heat
                        Led.Off();
                        Debug.WriteLine("Undetected! Turn the LED off.");
                    }
                    Led.Off();
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
