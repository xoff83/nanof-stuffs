
namespace nf_Utils
{
    using System.Device.Gpio;
    using nanoFramework.Hardware.Esp32;
    using System.Threading;

    public class Led
    {
        // Browse our samples repository: https://github.com/nanoframework/samples
        // Check our documentation online: https://docs.nanoframework.net/
        // Join our lively Discord community: https://discord.gg/gCyBu8T


        // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
        private const int pinLed = Gpio.IO02;
        private static GpioPin led;

        public static void setLed(int gpioLed = pinLed)
        {
            led = new GpioController().OpenPin(gpioLed, PinMode.Output);
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
                Thread.Sleep(msOn);
                led.Toggle();
                Thread.Sleep(msOff);
            }
            led.Write(PinValue.Low);
        }
    }
}
