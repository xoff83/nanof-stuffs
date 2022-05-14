using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Adc;
using System.Diagnostics;
using System.Threading;

namespace nf_Inmp441
{
    public class Program
    {
        // ESP32 DevKit: 4 is a valid GPIO pin in, some boards like Xiuxin ESP32 may require GPIO Pin 2 instead.
        private const int pinLed = Gpio.IO02;
      

        /**
    * T�l�chargement de donn�es: https://pan.baidu.com/s/1bQ5_QeCSIL2UOH7wSF_IFg
    * 
    * Lancement de produit:
    * INMP441 est un microphone MEMS omnidirectionnel � haute performance, faible consommation d'�nergie, sortie num�rique avec port inf�rieur.
    * La solution compl�te INMP441 se compose d'un capteur MEMS, d'un conditionnement de la composition du signal, d'un convertisseur analogique-num�rique, 
    * d'un filtre anti-cr�nelage, d'une gestion de l'alimentation et d'une interface i� 24 bits standard de l'industrie. L'interface i�s permet � INMP441 
    * de se connecter directement aux processeurs num�riques, tels que DSPs et microcontr�leurs, sans avoir besoin du codec audio utilis� dans le syst�me. 
    * INMP441 a un rapport signal/bruit �lev� et est un excellent choix pour les applications en champ proche. 
    * INMP441 a une r�ponse en fr�quence � large bande plate, ce qui entra�ne une haute d�finition du son naturel.
    * 
    * Caract�ristiques du produit:
    * 1. interface i�s num�rique avec donn�es 24 bits de haute pr�cision
    * 2. Rapport signal/bruit �lev� de 61 dBA
    * 3. Haute sensibilit� -26 dBFS
    * 4. R�ponse en fr�quence Stable de 60Hz � 15 kHz
    * 5. Faible consommation d'�nergie: faible consommation de courant de 1.4 mA
    * 6. PSR �lev�: -75 dBFS
    * 
    * D�finition de l'interface:
    *   SCK:        horloge de donn�es s�rie de l'interface i�s
    *   WS:         s�lection du mot de donn�es s�rie pour l'interface i�s
    *   L/R:        s�lection de canal gauche/droite.
    *                   Lorsqu'il est r�gl� au niveau bas, le microphone �met des signaux sur le canal gauche du cadre i�s.
    *                   Lorsqu'il est r�gl� � un niveau �lev�, le microphone �met le signal sur le bon canal 
    *   SD:         sortie de donn�es s�rie de l'interface i�s.
    *   VCC:        puissance d'entr�e, 1.8V � 3.3V.
    *   LA TERRE:   puissance sol
    *   
    * Ce produit fournit des tutoriels pour l'utilisation du module ESP32 avec fonction I2S
    * 
    * Connectez-vous � notre carte de d�veloppement ESP32:
    * INMP441   ESP32 Carte de D�veloppement
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

    * ADC
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
        public static void Main()
        {
            Debug.WriteLine("Decibelmeter nanoFramework!");

            //Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T

            /* There are no dedicated I2S pins on the ESP32. Instead, you will have to configure/enable the pins in your code.
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

            //https://github.com/ikostoski/esp32-i2s-slm
            //https://fr.wikipedia.org/wiki/I2S
            // tuto ADC PWM : https://www.upesy.fr/blogs/tutorials/measure-voltage-on-esp32-with-adc-with-arduino-code


      

            while (true)
            {

                try
                {

                    // Init the ADC
                    AdcController adc = new AdcController();

                    // for testing
                    //Logical channel # 	Internal ADC# 	GPIO # 	Note
                    //8 	                ADC1 	        36 	    Internal Temperture sensor (VP), See restrictions
                    //9                     ADC1            39      Internal Hall Sensor(VN), See restrictions
                    AdcChannel acTemperature = adc.OpenChannel(8);
                    AdcChannel acHallSensor = adc.OpenChannel(9);
                    Console.WriteLine($"Temperature: {acTemperature.ReadValue()} , HallSensor: {acHallSensor.ReadValue()}");


                    //AdcChannel acSckViolet = adc.OpenChannel(16);//16
                    //AdcChannel acWsBleu = adc.OpenChannel(13);//13
                    AdcChannel acSdGris = adc.OpenChannel(4);

                    int valSck, valWs, valSd = 0;

                    //// Get the value
                    //valSck = acSckViolet.ReadValue();
                    //valWs = acWsBleu.ReadValue();
                    valSd = acSdGris.ReadValue();

                    //Console.WriteLine($"SCK: {valSck} , WS: {valWs} , SD: {valSd}");
                    //Console.WriteLine($"SCK: {valSck}");
                    //Console.WriteLine($"WS: {valWs}");
                    Console.WriteLine($"SD: {valSd}");


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
}
