using System;
using System.Diagnostics;
using Iot.Device.Hcsr04.Esp32;
using nanoFramework.Hardware.Esp32;
using UnitsNet;
using nf_Utils;
using System.Threading;
using System.Device.I2c;
using Iot.Device.Ssd13xx;

namespace Test
{
    public class Program
    {
        //Sonar Hcsr04
        private static readonly int pinTrigger = Gpio.IO13;
        private static readonly int pinEcho = Gpio.IO12;
        //Screen SSD1306
        private static readonly int pinData = Gpio.IO21;
        private static readonly int pinClock = Gpio.IO22;

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
            Debug.WriteLine("Kick's on nanoFramework!");
            Led.blink(125, 125, 5);
            Led.blink(525, 1000);
            //Screen SSD1306
            // I2C1_CLOCK :  Device function CLOCK for I2C1
            // I2C1_DATA  :  Device function DATA for I2C1
            Configuration.SetPinFunction(pinData, DeviceFunction.I2C1_DATA);
            Configuration.SetPinFunction(pinClock, DeviceFunction.I2C1_CLOCK);

            Console.WriteLine("---------------------");
            Console.WriteLine("Start SSD1306 Display");
            Console.WriteLine("---------------------");
            Thread.Sleep(500);
            I2cDevice i2c = I2cDevice.Create(new I2cConnectionSettings(1, 0x3C /*Ssd1306.DefaultI2cAddress*//*, I2cBusSpeed.StandardMode*/));

            // instantiation example
            //rectangle 0.91
            Ssd1306 ssd1306 = new Ssd1306(i2c, Ssd13xx.DisplayResolution.OLED128x32);
            //bigger square 0.96
            //Ssd1306 ssd1306 = new Ssd1306(i2c, Ssd13xx.DisplayResolution.OLED128x64);


            ssd1306.ClearScreen();
            ssd1306.Font = new BasicFont();
            ssd1306.DrawString(2, 2, "Kick",2);//large size 2 font
            ssd1306.DrawString(70, 8, "'s on", 1);
            ssd1306.DrawString(2, 16, "nanoFramework", 1, true);//centered text
            ssd1306.Display();

            //Configuration.SetPinFunction(pinTrigger, DeviceFunction.???);
            //Configuration.SetPinFunction(pinEcho, DeviceFunction.I2C1_CLOCK);

            ssd1306.ClearScreen();
            using (var sonar = new Hcsr04(pinTrigger, pinEcho))
            {
                while (true)
                {
                    if (sonar.TryGetDistance(out Length distance))
                    {

                       
                        ssd1306.DrawString(2, 2, $"Distance: ", 1);
                        ssd1306.DrawString(0, 12, $"{(int)Math.Round(distance.Centimeters - 3.9)} cm   ", 2,true);
                        ssd1306.Display();
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
