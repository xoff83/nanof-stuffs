using System;
using System.Device.Adc;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;
using nf_Utils;

namespace nf_Moisture
{
    public class Program
    {
        /** ADC(https://deepbluembedded.com/esp32-adc-tutorial-read-analog-voltage-arduino/ OU https://microcontrollerslab.com/adc-esp32-measuring-voltage-example/)
        *   We use "ADC1" with 20 logical channels mapped to the ESP32 internal controllers ADC1 and ADC2 
        *   There are the 18 available ESP32 channels plus the internal Temperature and Hall sensors making the 20 logical channels.
        * 
        * Restrictions:-
        * 
        *   Channels 10 to 19 can not be used while the WiFi is enabled. (exception CLR_E_PIN_UNAVAILABLE)
        * Hall sensor and Temperature sensor can not be used at same time as Channels 0 and 3.
        *   Gpio 0, 2, 15 are strapping pins and can not be freely used (Channels 11, 12, 13 ), check board schematics.

       *
       * Logical channel       #	Internal ADC        #	GPIO        #	Note
        *       0	                    ADC1	                36	            See restrictions
        *       1	                    ADC1	                37	            
        *       2	                    ADC1	                38	            
        *       3	                    ADC1	                39	            See restrictions
        *       4	                    ADC1	                32	            
        *       5	                    ADC1	                33	            
        *       6	                    ADC1	                34	            
        *       7	                    ADC1	                35	            
        *       8	                    ADC1	                36	            Internal Temperture sensor(VP), See restrictions
        *       9	                    ADC1	                39	            Internal Hall Sensor(VN), See restrictions
        *       10	                    ADC2	                04	            
        *       11	                    ADC2	                00	            Strapping pin
        *       12	                    ADC2	                02	            Strapping pin
        *       13	                    ADC2	                15	            Strapping pin
        *       14	                    ADC2	                13	            
        *       15	                    ADC2	                12	            
        *       16	                    ADC2	                14	            
        *       17	                    ADC2	                27	            
        *       18	                    ADC2	                25	            
        *       19	                    ADC2	                26	 
        **/
        //GPIO pin 35 is adc channel 7
        private static readonly int pinAdc = Gpio.IO35;

        public static void Main()
        {
            Debug.WriteLine("Kick's on nanoFramework!");
            Led.blink(125, 125, 5);
            Led.blink(525, 1000);

            // Init the ADC
            AdcController adc = new AdcController();
            AdcChannel ac7 = adc.OpenChannel(7);
            // get maximum raw value from the ADC controller
            int max1 = adc.MaxValue;

            // get minimum raw value from the ADC controller
            int min1 = adc.MinValue;

            // find how many channels are available 
            int channelCount = adc.ChannelCount;

            // resolution provided by the ADC controller
            int adcResolution = adc.ResolutionInBits;

            // Oh no, not again, man what a day I'm having
            for (; ; )
            {
                // Get the value
                val4 = ac4.ReadValue();



                // Very slow sampling rate 
                Thread.Sleep(250);
            }

        }
    }
}
