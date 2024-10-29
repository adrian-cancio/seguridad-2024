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
            int num_bytes = 8; //Valor temporal
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


            //Guardamos el entero aleatorio a fichero
            string FicheroLectura = "NumeroAleatorio.bin";
            ayuda_var.GuardaBufer(FicheroLectura, buffer_bytes);

            //Creamos un nuevo buffer de lectura del fichero
            int tam_buffer = (int)ayuda_var.BytesFichero(FicheroLectura);
            byte[] buffer_lectura = new byte[tam_buffer];

            //Cargamos el contenido del fichero
            ayuda_var.CargaBufer(FicheroLectura, buffer_lectura);

            //Mostramos por consola el resultado
            int tam_fichero = (int)ayuda_var.BytesFichero(FicheroLectura);
            Console.WriteLine("Entero recuperado del fichero: ");
            ayuda_var.WriteHex(buffer_lectura, tam_fichero);

            //Liberamos los recursos
            rngcsp.Dispose();
        }
    }
}
