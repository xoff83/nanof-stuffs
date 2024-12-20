﻿namespace nf_Utils
{
    using System.Device.Gpio;
    using nanoFramework.Hardware.Esp32;
    using System.Threading;

    public class Led
    {
        // Browse our samples repository: https://github.com/nanoframework/samples
        // Check our documentation online: https://docs.nanoframework.net/
        // Join our lively Discord community: https://discord.gg/gCyBu8T

        // tuto :   https://www.robin-gueldenpfennig.de/2021/09/how-to-run-net-nanoframework-on-esp32-nodeesp/
        //          https://docs.nanoframework.net/content/getting-started-guides/getting-started-managed.html

        // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
        private const int pinLed = Gpio.IO02;
        private static GpioPin led;

        public static void setLed(int gpioLed = pinLed)
        {
            led = new GpioController().OpenPin(gpioLed, PinMode.Output);
        }

        public static void On()
        {
            if (led == null)
            {
                setLed();
            }
            led.Write(PinValue.High);
        }

        public static void Off()
        {
            if (led == null)
            {
                setLed();
            }
            led.Write(PinValue.Low);
        }
        public static void blink(int msOn = 200, int msOff = 0, int nb = 1)
        {
            if (led == null)
            {
                setLed();
            }
            for (int i = 0; i < nb; i++)
            {
                led.Write(PinValue.High);
                if (msOn > 0 || msOff > 0)
                {
                    Thread.Sleep(msOn);
                    led.Toggle();
                    Thread.Sleep(msOff);
                }

            }
            if (msOn > 0 || msOff > 0)
            {
                led.Write(PinValue.Low);
            }
        }
    }
}
