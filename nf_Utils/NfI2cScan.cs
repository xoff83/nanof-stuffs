using nanoFramework.Hardware.Esp32;
using System;
using System.Collections;
using System.Device.I2c;

namespace nf_Utils
{
    public class NfI2cScan
    {
        /// <summary>
        /// Scan I2C device addresses
        /// </summary>
        /// <param name="busId">I2C bus id, 1 or 2. default 1</param>
        /// <param name="clockPin">Clock pin</param>
        /// <param name="dataPin">Data pin</param>
        /// <returns></returns>
        public static byte[] Scan(int busId = 1, int clockPin = 22, int dataPin = 21)
        {
            switch (busId)
            {
                case 1:
                    Configuration.SetPinFunction(clockPin, DeviceFunction.I2C1_CLOCK);
                    Configuration.SetPinFunction(dataPin, DeviceFunction.I2C1_DATA);
                    break;
                case 2:
                    Configuration.SetPinFunction(clockPin, DeviceFunction.I2C2_CLOCK);
                    Configuration.SetPinFunction(dataPin, DeviceFunction.I2C2_DATA);
                    break;
                default:
                    throw new ArgumentException($"Unsupported busId - {busId}");
            }

            ArrayList list = new();
            SpanByte b = new SpanByte(new byte[1]);
            // I2C address range from https://learn.adafruit.com/i2c-addresses/the-list
            for (byte addr = 0x0E; addr <= 0x77; addr++)
            {
                using (var dev = I2cDevice.Create(new I2cConnectionSettings(busId, addr)))
                {
                    var r = dev.Read(b);
                    if (r.Status == I2cTransferStatus.SlaveAddressNotAcknowledged)
                        continue;
                    if (r.Status == I2cTransferStatus.FullTransfer ||
                        r.Status == I2cTransferStatus.PartialTransfer)
                    {
                        list.Add(addr);
                    }
                }
            }
            return list.ToArray(typeof(byte)) as byte[];
        }
    }
}
