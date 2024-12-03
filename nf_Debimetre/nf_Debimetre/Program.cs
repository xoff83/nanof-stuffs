using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;

namespace nfDebimetre
{
    public class Program
    {

        private static uint _pulseCount = 0;
        private static GpioController _gpio;
        private static GpioPin _flowSensorPin;

        /// <summary>
        /// 
        /// Protection
        /// Une résistance de pull-up(généralement 10 kΩ) entre la sortie du signal et l'alimentation positive.
        /// Un condensateur de découplage(0,1 μF) entre l'alimentation et la masse pour filtrer le bruit.
        /// </summary>
        public static void Main()
        {
            Debug.WriteLine("Hello from nanoFramework!");

            //Thread.Sleep(Timeout.Infinite);

            // Browse our samples repository: https://github.com/nanoframework/samples
            // Check our documentation online: https://docs.nanoframework.net/
            // Join our lively Discord community: https://discord.gg/gCyBu8T
            _gpio = new GpioController();
            _flowSensorPin = _gpio.OpenPin(23, PinMode.InputPullUp);
            _flowSensorPin.ValueChanged += FlowSensorPin_ValueChanged;
            PinValue pin = _flowSensorPin.Read();
            uint compteur = 0;
            while (true)
            {
                try
                {
                    Thread.Sleep(1000);
                    compteur++;
                    Console.WriteLine($"--------------------- {compteur} -------------------------------");
                    Console.WriteLine($"Impulsions: {_pulseCount} /s  - valeur de lecture {pin.ToString()}");
                    Console.WriteLine($"Débit: {debit(_pulseCount, 109)} L/min");
                    Console.WriteLine($"-------------------------------------------------------------");
                }
                catch (Exception ex) { Console.WriteLine($"Erreur avec un pin {pin.ToString()} (pour {_pulseCount} impulsion(s)): {ex}"); }


                _pulseCount = 0;
            }
        }
        private static float debit(uint pulse, float kFactor = 109)
        {
            /*Calcul du débit
            Pour convertir les impulsions en débit, utilisez la formule fournie par le fabricant du débitmètre.
            Généralement, elle ressemble à :
            Débit(L/min)=(Nombre d′impulsions / FacteurK)∗60
            où le Facteur K est spécifique à votre modèle de débitmètre.
            Spécification:
Matériel: Plastique
Couleur: Blanc
Fonction: Capteur, contrôle de débit
Débit: 0.3 ~ 3 L/min
Impulsion de débit: F(Hz)=(109xQ) L/min +/- 5%
Max. Courant de travail: 15mA (DC5 V)
Min. Tension de fonctionnement: DC 4.5V
Tension de fonctionnement: DC 5V ~ 24V
Capacité de charge: = 10 mA (DC 5 V)
Pression: 0.8mPa
Température de fonctionnement: -25 ~ + 60 ℃
1 litière d'eau = 7055 impulsion +/-10%
Précision: +- 5%
Environnement de travail: Eau, liquide, huile légère, bière, eau potable
Diamètre du tuyau de connexion: 6mm ou 1/4"
Diamètre intérieur: 58.4mm/0.23"
Diamètre extérieur: 12.2mm/0.48"*/
            return pulse / kFactor;
        }
        private static void FlowSensorPin_ValueChanged(object sender, PinValueChangedEventArgs e)
        {
            try
            {
                if (e.ChangeType == PinEventTypes.Rising)
                {
                    _pulseCount++;
                }
                if (PinEventTypes.None == e.ChangeType)
                {
                    _pulseCount = 0;
                }
            }
            catch (Exception ex) { Console.WriteLine($"Erreur avec {sender} (pour {_pulseCount} impulsion(s)): {ex}"); }
        }
    }
}

