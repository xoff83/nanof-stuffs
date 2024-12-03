using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;

namespace nf_Utils
{
    /* 
     *     Nom de marque: Shanwen
     *     Origine: CN (Origine)
     *     Fournitures DIY: électrique
     *     Numéro de Modèle: Level Flow Meter
     *     Certification: NONE
     *     
     * Description:
     *   Le capteur de débit d'eau comprend un boîtier en plastique, un rotor d'eau et un capteur à effet hall, 
     *   une carte électronique PCB
     *   Performance de travail: lorsque l'eau traverse le rotor, le rotor est roulé
     *   Sa vitesse change avec un débit différent. Ensuite, le rotor magnétique réagit au capteur Hall,
     *   Le capteur à effet hall émet le signal d'impulsion correspondant
     *   Le capteur de débit est adapté pour détecter le débit dans le distributeur d'eau ou la machine à café... 
     *
     * component details:
     * 
     * 
     * Spécification:
     *   Matériau: Plastique
     *   Couleur: Blanc
     *   Fonction: capteur, contrôle du débit
     *   Débit: 0.3 ~ 3 L/min
     *   Débit d'impulsion: F(Hz)=(109xQ) L/min +/- 5% (En prenant le débit de 1L/min comme colonne, 109*1*60= 6540 est égal au nombre d'impulsions produites par 1 litre d'eau)
     *   Max. Courant de fonctionnement: 15mA (DC5 V)
     *   Min. Tension de fonctionnement: cc 4.5V
     *   Tension de fonctionnement: cc 5V ~ 24V
     *   Capacité de charge: = 10 mA (cc 5 V)
     *   Pression: 0.8mPa
     *   Température de fonctionnement: -25 ~ + 60 ℃
     *   1 litre d'eau = 7055 impulsion +/-10%
     *   Précision: +- 5%
     *   Environnement de travail: eau, liquide, huile légère, bière, eau potable
     *   Connecter le diamètre du tuyau: 6mm ou 1/4"
     *   Diamètre intérieur: 58.4mm/0.23"
     *   Diamètre extérieur: 12.2mm/0.48"
     *   Quantité: 1 Pc */
    public class NfHall
    {

        private const int defaultPinHall = Gpio.IO15;
        private static GpioPin hallInpulsePin;
        public static GpioPulseCounter counter;

        public static void setLed(int gpioHall = defaultPinHall)
        {
            hallInpulsePin = new GpioController().OpenPin(gpioHall, PinMode.Output);

            GpioPulseCounter counter = new GpioPulseCounter(gpioHall);
        }
        public static void Launch()
        {
            //https://github.com/nanoframework/Samples/tree/main/samples/Gpio/Esp32PulseCounter
            Debug.WriteLine("Hello pulse counter!");
            /**
             *  PWM	Modulation de largeur d'impulsion
             *  ADC	Convertisseur analogique-numérique
             *  SPI	Interface de communication série point à point
             *  I2S	Interface de communication audio numérique
             *  I2C	Interface de communication bus maître-esclave
            **/


            Debug.Write("Starting PWM and Counter. With 10 Hz and counting on rising");
            counter.Polarity = GpioPulsePolarity.Rising;
            counter.FilterPulses = 2; // 80Mhz
            counter.Reset();


            counter.Start();
            int inc = 0;
            long previousValue = 0;
            long currentValue = 0;

            GpioPulseCount counterCount;

            TimeSpan intalTS = counter.Read().RelativeTime;
            while (inc++ < 200)
            {

                counterCount = counter.Read();
                currentValue = counterCount.Count - previousValue;
                Console.WriteLine($"{counterCount.RelativeTime - intalTS}: {counterCount.Count} => {currentValue} in {counterCount.RelativeTime.TotalSeconds} s ({counterCount.Count / counterCount.RelativeTime.TotalSeconds}");
                Console.WriteLine($"                    soit {(((float)counterCount.Count / 7055))} litre(s) , {60 * (currentValue) / 7055} L/min");
                Thread.Sleep(1000);
                //counterCount = counter.Reset();
                previousValue = counterCount.Count;
            }

            counter.Stop();
            counter.Dispose();

            //Thread.Sleep(Timeout.Infinite);
        }
    }
}
