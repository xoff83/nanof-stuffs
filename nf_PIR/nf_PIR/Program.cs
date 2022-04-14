using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace nf_PIR
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            Int32 pinPIR = 17;
            Int32 pinLED = 27;

            Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T

            GpioController ledController = new GpioController();
            ledController.OpenPin(pinLED, PinMode.Output);

            using (Iot.Device.Hcsr501.Hcsr501 sensor =
                new Iot.Device.Hcsr501.Hcsr501(pinPIR))
            {
                while (true)
                {
                    // adjusting the detection distance and time by rotating the potentiometer on the sensor
                    if (sensor.IsMotionDetected)
                    {
                        // turn the led on when the sensor detected infrared heat
                        ledController.Write(pinLED, PinValue.High);
                        Debug.WriteLine("Detected! Turn the LED on.");
                    }
                    else
                    {
                        // turn the led off when the sensor undetected infrared heat
                        ledController.Write(pinLED, PinValue.Low);
                        Debug.WriteLine("Undetected! Turn the LED off.");
                    }

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
