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
    class Program
    {
        static void Main(string[] args)
        {
            //Primera fase . 1
            //Creacion de una variable para indicar la longitud del array de bytes
            int num_bytes = 16; //Valor temporal
            //Declaracion del array de bytes
            byte[] buffer_bytes = new byte[num_bytes];
            //Creacion de un objeto Ayuda
            Ayuda ayuda_var = new Ayuda();

            //Primera fase .2
            //Creacion del objetjo RNGCryptoServiceProvider
            RNGCryptoServiceProvider rngcsp = new RNGCryptoServiceProvider();
            //Imprimimos por consola el entero aleatorio resultante
            rngcsp.GetBytes(buffer_bytes);
            Console.WriteLine("Entero aleatorio: ");
            ayuda_var.WriteHex(buffer_bytes, num_bytes);

            Console.WriteLine();
            //Cifrando los datos. Protegemos buffer_bytes, optionalEntropy opcional y decretamos Scope como CurrentUser
            byte[] bytes_cifrados = ProtectedData.Protect(buffer_bytes, null, DataProtectionScope.CurrentUser);

            
            //Imprimimos por pantalla los bytes cifrados
            Console.WriteLine("Bytes cifrados: ");
            ayuda_var.WriteHex(bytes_cifrados, bytes_cifrados.Length);
            Console.WriteLine();

            //Guardamos el entero aleatorio a fichero
            string FicheroLectura = "NumeroAleatorio.bin";
            ayuda_var.GuardaBufer(FicheroLectura, bytes_cifrados);

            //Creamos un nuevo buffer de lectura del fichero
            int tam_buffer = (int)ayuda_var.BytesFichero(FicheroLectura);
            byte[] buffer_lectura = new byte[tam_buffer];

            //Cargamos el contenido del fichero
            ayuda_var.CargaBufer(FicheroLectura, buffer_lectura);

            //Desciframos el fichero con los mismos parametros que con los que ciframos, creamos un nuevo buffer
            //Donde se almacenaran los bytes descifrados
            byte[] bytes_descifrados = ProtectedData.Unprotect(buffer_lectura, null, DataProtectionScope.CurrentUser);

            Console.WriteLine("Bytes descifrados: ");
            ayuda_var.WriteHex(bytes_descifrados, bytes_descifrados.Length);
            Console.WriteLine();
            
            
            //Mostramos por consola el resultado
            int tam_fichero = (int)ayuda_var.BytesFichero(FicheroLectura);
            Console.WriteLine("Entero recuperado del fichero y desencriptado: ");
            ayuda_var.WriteHex(buffer_lectura, tam_fichero);

            //Ciframos el ultimo byte del fichero
            byte[] ultimo_digito = new byte[1];
            Array.Copy(bytes_descifrados, bytes_descifrados.Length - (bytes_descifrados.Length-1), ultimo_digito, 0, 1);

            ProtectedMemory.Protect(ultimo_digito , MemoryProtectionScope.CrossProcess);
            Console.WriteLine("Ultimo byte cifrado: "); ayuda_var.WriteHex(bytes_descifrados, bytes_descifrados.Length); Console.WriteLine();

            //Desciframos el ultimo byte del fichero
            ProtectedMemory.Unprotect(ultimo_digito, MemoryProtectionScope.CrossProcess);
            Console.WriteLine("Ultimo byte descifrado: "); ayuda_var.WriteHex(bytes_descifrados, bytes_descifrados.Length); Console.WriteLine();


            //Liberamos los recursos
            rngcsp.Dispose();
        }
    }
}
