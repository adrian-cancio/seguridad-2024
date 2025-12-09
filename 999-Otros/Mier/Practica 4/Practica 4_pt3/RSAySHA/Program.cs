using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace RSAySHA
{
    internal class Program
    {
        static void Main(string[] args)
        {



            // ----------------------------- 3. CIFRAR Y DESCIFRAR INFORMACION CON RSA ----------------------------- //
            Console.WriteLine("----- 3. CIFRAR Y DESCIFRAR INFORMACION CON RSA -----\n");
            Console.WriteLine("----- Esta solucion contiene el DESCIFRADO de informacion -----\n");



            // Objeto ayuda para usar todos los metodos de ayuda desarrollados
            Ayuda ayuda = new Ayuda();

            // Crear una clave RSA de 1024 bits
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // Mostrar si la longitud de la clave es la deseada
            Console.WriteLine("Longitud de la clave: " + rsa.KeySize);

            // Obtener tamaño en bytes del fichero publico y declarar un array de bytes con ese tamaño para almacenar el contenido
            long size = ayuda.BytesFichero("zz_BlobRSA_Priva.bin");
            byte[] buffer = new byte[size];

            // Cargar array de bytes con el contenido del fichero
            ayuda.CargaBufer("zz_BlobRSA_Priva.bin", buffer);

            //Importar el Blob en el proveedor RSA
            rsa.ImportCspBlob(buffer);

            //Obtener tam en bytes del textoCifrado y alamacenarlo en un array de bytes
            long sizeTextCifra = ayuda.BytesFichero("zz_TextoCifrado.bin");
            byte[] textoCifrado = new byte[sizeTextCifra];
            ayuda.CargaBufer("zz_TextoCifrado.bin", textoCifrado);

            // Mostramos por consola para ver el contenido del texto plano
            Console.WriteLine("\nContenido del texto cifrado: ");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);
            Console.WriteLine();

            // Declarar un array de bytes para el texto descifrado y asignarle resultado
            // de llamar al método Decrypt()

            byte[] textoDescifrado = new byte[sizeTextCifra]; // 64 que era el tamaño del texto plano
            textoDescifrado = rsa.Decrypt(textoCifrado, true);

            Console.WriteLine("Contenido del texto descifrado:");
            ayuda.WriteHex(textoDescifrado, textoDescifrado.Length);
            Console.WriteLine();

            rsa.Dispose();
        }
    }
}
