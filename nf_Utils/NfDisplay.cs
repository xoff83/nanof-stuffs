using System;
using System.Diagnostics;
using Iot.Device.Ssd13xx;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Logging.Debug;

namespace nf_Utils
{

    internal class NfDisplay
    {
        public static int pinData = Gpio.IO33;
        public static int pinClock = Gpio.IO33;
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
            ssd1306.DrawString(2, 2, "Kick", 2);//large size 2 font
            ssd1306.DrawString(70, 8, "'s on", 1);
            ssd1306.DrawString(2, 16, "nanoFramework", 1, true);//centered text
            ssd1306.Display();

            //Configuration.SetPinFunction(pinTrigger, DeviceFunction.???);
            //Configuration.SetPinFunction(pinEcho, DeviceFunction.I2C1_CLOCK);

            ssd1306.ClearScreen();
        }
    }
}
