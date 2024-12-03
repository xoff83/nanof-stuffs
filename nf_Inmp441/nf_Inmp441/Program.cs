using System;
using System.Device.Adc;
using System.Device.I2s;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;
//using nf_Utils;


namespace nf_Inmp441
{
    public class Program
    {
        /**
        * Téléchargement de données: https://pan.baidu.com/s/1bQ5_QeCSIL2UOH7wSF_IFg
        * 
        * Lancement de produit:
        * INMP441 est un microphone MEMS omnidirectionnel à haute performance, faible consommation d'énergie,
        * sortie numérique avec port inférieur.
        * La solution complète INMP441 se compose d'un capteur MEMS, d'un conditionnement de la composition du signal,
        * d'un convertisseur analogique-numérique, d'un filtre anti-crénelage,
        * d'une gestion de l'alimentation et d'une interface i² 24 bits standard de l'industrie.
        * L'interface i²s permet à INMP441 de se connecter directement aux processeurs numériques, tels que DSPs et microcontrôleurs,
        * sans avoir besoin du codec audio utilisé dans le système. 
        * INMP441 a un rapport signal/bruit élevé et est un excellent choix pour les applications en champ proche. 
        * INMP441 a une réponse en fréquence à large bande plate, ce qui entraéne une haute définition du son naturel.
        * 
        * Caractéristiques du produit:
        * 1. interface i²s numérique avec données 24 bits de haute précision
        * 2. Rapport signal/bruit élevé de 61 dBA
        * 3. Haute sensibilité -26 dBFS
        * 4. Réponse en fréquence Stable de 60Hz à 15 kHz
        * 5. Faible consommation d'énergie: faible consommation de courant de 1.4 mA
        * 6. PSR élevé: -75 dBFS
        * 
        * Définition de l'interface:
        *   SCK:        horloge de données série de l'interface i²s
        *   WS:         sélection du mot de données série pour l'interface i²s
        *   L/R:        sélection de canal gauche/droite.
        *                   Lorsqu'il est réglé au niveau bas, le microphone émet des signaux sur le canal gauche du cadre i²s.
        *                   Lorsqu'il est réglé à un niveau élevé, le microphone émet le signal sur le canal droit 
        *   SD:         sortie de données série de l'interface i²s.
        *   VCC:        puissance d'entrée, 1.8V à 3.3V.
        *   LA TERRE:   puissance sol
        *   
        * Ce produit fournit des tutoriels pour l'utilisation du module ESP32 avec fonction I2S
        * 
        * Connectez-vous à notre carte de développement ESP32:
        * INMP441   ESP32 Carte de Développement
        * LA SCK    >> GPIO14 : ADC2 CH6
        * SD        >> GPIO32 : ADC1 CH4
        * WS        >> GPIO15 : ADC1 CH3
        * L/R       >> GND
        * GND       >> GND
        * LA DMV    >> VDD3.3
        * 
        * https://spacehuhn.com/pages/projects
        * https://github.co m/SpacehuhnTech/esp8266_deauther
        * https://github.com/SpacehuhnTech/esp8266_deauther/tree/v3
        * https://github.com/spacehuhntech/esp8266_deauther/wiki/Installation
        * 
        * extract from : https://docs.nanoframework.net/content/esp32/esp32_pin_out.html
        * I2C
        *   There are 2 I2C bus available:
        * 
        *   I2C#	    Data	    Clock
        *   I2C1	    GPIO 18	    GPIO 19
        *   I2C2	    GPIO 25	    GPIO 26

        * ADC (https://deepbluembedded.com/esp32-adc-tutorial-read-analog-voltage-arduino/ OU https://microcontrollerslab.com/adc-esp32-measuring-voltage-example/)
        *   We use "ADC1" with 20 logical channels mapped to the ESP32 internal controllers ADC1 and ADC2 
        *   There are the 18 available ESP32 channels plus the internal Temperature and Hall sensors making the 20 logical channels.
        * 
        * Restrictions:-
        * 
        *   Channels 10 to 19 can not be used while the WiFi is enabled. (exception CLR_E_PIN_UNAVAILABLE)
        *   Hall sensor and Temperature sensor can not be used at same time as Channels 0 and 3.
        *   Gpio 0, 2, 15 are strapping pins and can not be freely used ( Channels 11, 12, 13 ), check board schematics.
        * 
        *       Logical channel       #	Internal ADC        #	GPIO        #	Note
        *       0	                    ADC1	                36	            See restrictions
        *       1	                    ADC1	                37	            
        *       2	                    ADC1	                38	            
        *       3	                    ADC1	                39	            See restrictions
        *       4	                    ADC1	                32	            
        *       5	                    ADC1	                33	            
        *       6	                    ADC1	                34	            
        *       7	                    ADC1	                35	            
        *       8	                    ADC1	                36	            Internal Temperture sensor (VP), See restrictions
        *       9	                    ADC1	                39	            Internal Hall Sensor (VN), See restrictions
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

        private const int SampleRate = 16_000; //8_000; //44_100 ;
        private const int BufferSize = 1024;

        public static void Main()
        {
            Debug.WriteLine("Kick's on nanoFramework!");
            //Led.blink(125, 125, 5);
            //Led.blink(525, 1000);

            Debug.WriteLine("Decibelmeter nanoFramework!");

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T

            /* There are no dedicated I2S pins on the ESP32. 
             * Instead, you will have to configure/enable the pins in your code.
             * Eventually, 
             * I connected I2S-MEMS pins to the following ESP32 pins:
             * 
             *     SEL is unconnected, i.e. only one channel, apparently left
             *     LRCL (WS or FS)                                          to #15 : Left-Right Clock / Word Select / Frame Sync
             *     DOUT (SD or SDATA or SDIN or SDOUT or DAXDAT or ADCDAT)  to #32 : Serial Out / 
             *     BCKL (SCK)                                               to #14 : Bit Clock / Continuous serial clock
             *     GND                                                      to GND
             *     3V                                                       to 3V
             *     MCLK : master clock = 256 times the WS
             *     
             *     Don't try to connect them to the similar sounding SCL/SCA/SCK, they're for the incompatible I2C interface.
             *     I2S is similar to SPI
             */

            //     I2S1/2 function Master Clock. Used only in master mode.
            //Configuration.SetPinFunction(Gpio.IO32, DeviceFunction.I2S1_MCK);

            //     I2S1/2 function Bit Clock. Used for general purpose read and write on the I2S bus.
            Configuration.SetPinFunction(Gpio.IO33, DeviceFunction.I2S1_BCK); // SCK (BCLK)

            //     I2S1/2 function WS. Used if your have stereo.
            Configuration.SetPinFunction(Gpio.IO34, DeviceFunction.I2S1_WS); // WS (LRCLK)

            //     I2S1/2 function DATA_OUT. Used for output data typically on a speaker.
            //Configuration.SetPinFunction(Gpio.IO32, DeviceFunction.I2S1_DATA_OUT);

            //     I2S1/2 function MDATA_IN. Used for input data typically from a microphone.
            Configuration.SetPinFunction(Gpio.IO32, DeviceFunction.I2S1_MDATA_IN); // SD (DATA)

            // Configuration I2S pour le capteur INMP441
            I2sConnectionSettings i2sConfig = new I2sConnectionSettings(1) {
                BusId = 1,
                BitsPerSample = I2sBitsPerSample.Bit24,
                ChannelFormat = I2sChannelFormat.OnlyLeft,
                Mode = I2sMode.Master | I2sMode.Rx,//| I2sMode.Pdm,
                CommunicationFormat = I2sCommunicationFormat.I2S,
                SampleRate = SampleRate,
            };

            try
            {
                using (I2sDevice i2s = I2sDevice.Create(i2sConfig))
                {
                    var buffer = new byte[BufferSize];

                    while (true)
                    {
                        i2s.Read(buffer);
                        double dbLevel = CalculateDecibels(buffer);
                        Debug.WriteLine($"Niveau sonore : {dbLevel:F2} dB");
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur I2S : {ex.Message}");
            }





        }

        private static double CalculateDecibels(byte[] buffer)
        {
            double sum = 0;
            int samples = buffer.Length / 4; // 4 bytes par échantillon (24 bits + 8 bits de padding)

            for (int i = 0; i < buffer.Length; i += 4)
            {
                int sample = (buffer[i + 2] << 16) | (buffer[i + 1] << 8) | buffer[i];
                if ((sample & 0x800000) != 0)
                {
                    sample |= ~0xFFFFFF; // Extension du signe pour les nombres négatifs
                }
                double normalizedSample = sample / 8388608.0; // Normalisation à [-1, 1]
                sum += normalizedSample * normalizedSample;
            }

            double rms = Math.Sqrt(sum / samples);
            double db = (20 * Math.Log10(rms)) + 94; // +94 dB pour la référence du microphone

            return db;
        }
        public static void adcMain()
        {

            //https://github.com/ikostoski/esp32-i2s-slm
            //https://fr.wikipedia.org/wiki/I2S
            // tuto ADC PWM : https://www.upesy.fr/blogs/tutorials/measure-voltage-on-esp32-with-adc-with-arduino-code

            /*
             *       Logical channel       #	Internal ADC        #	GPIO        #	Note
             *       1	                    ADC1	                37	            
             *       2	                    ADC1	                38	            
             *       4	                    ADC1	                32	            
             *       5	                    ADC1	                33	            
             *       6	                    ADC1	                34	            
             *       7	                    ADC1	                35	   
             */

            /*              SCK         |           WS          |           L/R
             *          Synchro ClocK   |   Word Selector I2s   |       Left / Right channel
             *              Violet      |          Bleu         |            Vert
             *              ADC1:5      |         ADC1:6        |        0v (ground) for Left Channel
             *             GPIO: 33     |        GPIO: 34       |          GPIO:  GND
             *   -------------------------------------------------------------------------------
             *                          |                       |
             *              SD          |           VDD         |           GND
             *        Serial Data Out   |                       |           
             *             Gris         |         blanc         |           Noir
             *              ADC1:4      |      1.8V to 3.3V     |          0v (masse)  
             *             GPIO: 32     |       GPIO: 3V3       |          GPIO:  GND
             *              
             *              
             *   SCK:        horloge de données série de l'interface i²s
             *   WS:         sélection du mot de données série pour l'interface i²s
             *   L/R:        sélection de canal gauche/droite.
             *                   Lorsqu'il est réglé au niveau bas, le microphone émet des signaux sur le canal gauche du cadre i²s.
             *                   Lorsqu'il est réglé à un niveau élevé, le microphone émet le signal sur le bon canal 
             *   SD:         sortie de données série de l'interface i²s.
             *   VCC:        puissance d'entrée, 1.8V à 3.3V.
             *   GND:        puissance sol              
             */


            //     I2S1/2 function Master Clock. Used only in master mode.
            //Configuration.SetPinFunction(Gpio.IO32, DeviceFunction.I2S1_MCK);

            //     I2S1/2 function Bit Clock. Used for general purpose read and write on the I2S bus.
            Configuration.SetPinFunction(Gpio.IO33, DeviceFunction.I2S1_BCK);

            //     I2S1/2 function WS. Used if your have stereo.
            Configuration.SetPinFunction(Gpio.IO34, DeviceFunction.I2S1_WS);

            //     I2S1/2 function DATA_OUT. Used for output data typically on a speaker.
            //Configuration.SetPinFunction(Gpio.IO32, DeviceFunction.I2S1_DATA_OUT);

            //     I2S1/2 function MDATA_IN. Used for input data typically from a microphone.
            Configuration.SetPinFunction(Gpio.IO33, DeviceFunction.I2S1_MDATA_IN);


            I2sDevice i2s = new(new I2sConnectionSettings(1) {
                BitsPerSample = I2sBitsPerSample.Bit8,
                ChannelFormat = I2sChannelFormat.OnlyLeft,
                Mode = I2sMode.Master | I2sMode.Rx | I2sMode.Pdm,
                CommunicationFormat = I2sCommunicationFormat.I2S,
                SampleRate = 8_000
            });



            // Init the ADC
            AdcController adc = new AdcController();

            // for testing
            //Logical channel # 	Internal ADC# 	GPIO # 	Note
            //8 	                ADC1 	        36 	    Internal Temperature sensor (VP), See restrictions
            //9                     ADC1            39      Internal Hall Sensor(VN), See restrictions
            //AdcChannel acTemperature = adc.OpenChannel(8);
            //AdcChannel acHallSensor = adc.OpenChannel(9);
            //Console.WriteLine($"Temperature: {acTemperature.ReadValue()} , HallSensor: {acHallSensor.ReadValue()}");



            AdcChannel acSckViolet = adc.OpenChannel(5);//16
            AdcChannel acWsBleu = adc.OpenChannel(6);//13
            AdcChannel acSdGris = adc.OpenChannel(4);


            int max1 = adc.MaxValue;
            int min1 = adc.MinValue;

            Debug.WriteLine("min1=" + min1.ToString() + " max1=" + max1.ToString());


            int valSck, valWs, valSd = 0;

            while (true)
            {

                try
                {
                    //// Get the value
                    valSck = acSckViolet.ReadValue();
                    valWs = acWsBleu.ReadValue();
                    valSd = acSdGris.ReadValue();

                    //Console.WriteLine($"SCK: {valSck} , WS: {valWs} , SD: {valSd}");
                    Console.WriteLine($"SCK: {acSckViolet.ReadRatio()}% , WS: {acSckViolet.ReadRatio()}% , SD: {acSdGris.ReadRatio()}");



                }
                catch (Exception ex)
                {
                    // Do whatever pleases you with the exception caught
                    Console.WriteLine(ex.ToString());
                }
                // Very slow sampling rate 
                Thread.Sleep(250);
            }
        }

    }

    public class calculPuissance
    {
        //CF : https://forum.arduino.cc/t/arduino-decibel-meter/508876/27
        int num_Measure = 1024; // Set the number of measurements   
        int pinSignal = 0; // pin connected to pin O module sound sensor   
        long Sound_signal;    // Store the value read Sound Sensor   
        long sum = 0; // Store the total value of n measurements   
        long level = 0; // Store the average value   
        int sortie_source = 234; // valeur de sortie avec source de 30 dB
        public void calculate()
        {
            I2sDevice i2s = new(new I2sConnectionSettings(1) {
                BitsPerSample = I2sBitsPerSample.Bit8,
                ChannelFormat = I2sChannelFormat.OnlyLeft,
                Mode = I2sMode.Master | I2sMode.Rx | I2sMode.Pdm,
                CommunicationFormat = I2sCommunicationFormat.I2S,
                SampleRate = 8_000
            });


            // should be one second of sound data:
            SpanByte buff = new byte[8000];
            i2s.Read(buff);
            i2s.Dispose();

            byte[] buffer = buff.ToArray();

            //  128 signal readings   
            for (int i = 0; i < num_Measure; i++)
            {
                //Sound_signal = analogRead(pinSignal);
                sum = sum + Sound_signal ^ 2;
                sum = sum + Sound_signal * Sound_signal;
            }

            level = (sum / num_Measure) / (sortie_source) + 30; // Calculate the average value   
            Debug.Write("Sound Level: ");
            Debug.WriteLine("" + level);
            sum = 0; // Reset the sum of the measurement values  

        }
    }

}