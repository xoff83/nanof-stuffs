using System;
using System.Device.I2c;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;
using nf_Utils;

namespace nf_HallEffect
{
    public class Program
    {
        public static void Main()
        {
            Debug.WriteLine("Kick's on nanoFramework!");
            Led.blink(125, 125, 5);
            Led.blink(525, 1000);
            
            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T

            NfHall.Launch();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
