using System;
using System.Linq;
using System.Security.Cryptography;
using Apoyo;

namespace AleatorioProtegido
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int BYTES = 64;
            const String NOMBRE_FICHERO = "NumeroAleatorio.bin";
            
            Ayuda ayuda = new Ayuda();
            
            byte[] bufer = new byte[BYTES];

            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(bufer);
            }


            // Protegemos el búfer
            byte[] buferProtegido = ProtectedData.Protect(bufer, null, DataProtectionScope.CurrentUser);

            Console.WriteLine("Búfer cifrado");
            ayuda.WriteHex(buferProtegido, buferProtegido.Length);

            // Guardamos el búfer protegido en el archivo
            ayuda.GuardaBufer(NOMBRE_FICHERO, buferProtegido);
            Console.WriteLine("Fichero de " + ayuda.BytesFichero(NOMBRE_FICHERO) + " bytes guardado");

            // Cargamos el búfer protegido desde el archivo
            byte[] newBuferProtegido = new byte[buferProtegido.Length];
            ayuda.CargaBufer(NOMBRE_FICHERO, newBuferProtegido);

            // Desprotegemos el búfer cargado
            byte[] newBufer = ProtectedData.Unprotect(newBuferProtegido, null, DataProtectionScope.CurrentUser);

            Console.WriteLine("Búfer descifrado");
            ayuda.WriteHex(newBufer, newBufer.Length);

            // Verificamos que el búfer original y el desprotegido son iguales
            bool iguales = bufer.SequenceEqual(newBufer);
            Console.WriteLine("Los búferes {0}son iguales ", (iguales?"":"no "));
        }
    }
}
