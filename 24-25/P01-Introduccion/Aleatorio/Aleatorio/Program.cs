using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using Apoyo;


namespace Aleatorio
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Ayuda ayuda = new Ayuda();
            const int BYTES = 64;
            byte[] bufer = new byte[BYTES];
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            provider.GetBytes(bufer);
            provider.Dispose();
            
            ayuda.WriteHex(bufer, BYTES);

            const String NOMBRE_FICHERO = "NumeroAleatorio.bin";

            ayuda.GuardaBufer(NOMBRE_FICHERO, bufer);
            Console.WriteLine("Fichero de "+ ayuda.BytesFichero(NOMBRE_FICHERO) + " bytes guardado");

            byte[] newBufer = new byte[BYTES];
            ayuda.CargaBufer(NOMBRE_FICHERO, newBufer);

            ayuda.WriteHex(newBufer, BYTES);

        }
    }
}
