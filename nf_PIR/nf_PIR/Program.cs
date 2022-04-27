using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace nf_PIR
{
    public class Program
    {
        private static GpioController s_GpioController;
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");
            Int32 pinPIR = 21;
            Int32 pinLED = 2;

            s_GpioController = new GpioController();
            // ESP32 DevKit: Xiuxin ESP32 may require GPIO Pin 2.
            GpioPin led = s_GpioController.OpenPin(pinLED, PinMode.Output);
            led.Write(PinValue.Low);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T


            // Starting
            led.Toggle();
            Thread.Sleep(125);
            led.Toggle();
            Thread.Sleep(125);
            led.Toggle();
            Thread.Sleep(125);
            led.Toggle();
            Thread.Sleep(525);

            GpioPin pir = s_GpioController.OpenPin(pinPIR, PinMode.InputPullUp);

            while (true)
            {
                Debug.WriteLine($"Pir  on {pir.PinNumber} value :  {pir.Read()}");
                led.Write(pir.Read());
                Thread.Sleep(1000);
                led.Write(PinValue.Low);
                Thread.Sleep(500);
            }

            using (Iot.Device.Hcsr501.Hcsr501 sensor = new Iot.Device.Hcsr501.Hcsr501(pinPIR))
            {
                while (true)
                {


                    // adjusting the detection distance and time by rotating the potentiometer on the sensor
                    if (sensor.IsMotionDetected)
                    {
                        // turn the led on when the sensor detected infrared heat
                        //ledController.Write(pinLED, PinValue.High);
                        led.Write(PinValue.High);
                        Debug.WriteLine("Detected! Turn the LED on.");
                        Thread.Sleep(500);
                    }
                    else
                    {
                        // turn the led off when the sensor undetected infrared heat
                        //ledController.Write(pinLED, PinValue.Low);
                        led.Write(PinValue.Low);
                        Debug.WriteLine("Undetected! Turn the LED off.");
                    }
                    led.Write(PinValue.Low);
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
