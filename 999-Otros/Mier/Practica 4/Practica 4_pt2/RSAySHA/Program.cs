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
            Console.WriteLine("----- Esta solucion contiene el CIFRADO de informacion -----\n");



            // Objeto ayuda para usar todos los metodos de ayuda desarrollados
            Ayuda ayuda = new Ayuda();

            // Crear una clave RSA de 1024 bits
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // Mostrar si la longitud de la clave es la deseada
            Console.WriteLine("Longitud de la clave: " + rsa.KeySize);

            // Exportar las claves a BLOB
            ayuda.GuardaBufer("zz_BlobRSA_Priva.bin", rsa.ExportCspBlob(true));  // Con la clave privada
            ayuda.GuardaBufer("zz_BlobRSA_Publi.bin", rsa.ExportCspBlob(false)); // Sin la clave privada

            // Obtener tamaño en bytes del fichero publico y declarar un array de bytes con ese tamaño para almacenar el contenido
            long size = ayuda.BytesFichero("zz_BlobRSA_Publi.bin");
            byte[] buffer = new byte[size];

            // Cargar array de bytes con el contenido del fichero
            ayuda.CargaBufer("zz_BlobRSA_Publi.bin", buffer);

            //Importar el Blob en el proveedor RSA
            rsa.ImportCspBlob(buffer);

            // Crear un array de bytes para almacenar texto plano a cifrar
            byte[] textoPlano = new byte[64];
            for (int i = 0; i < textoPlano.Length; i++)
            {
                textoPlano[i] = Convert.ToByte(i);
            }

            // Mostramos por consola para ver el contenido del texto plano
            Console.WriteLine("\nContenido del texto plano: ");
            ayuda.WriteHex(textoPlano, textoPlano.Length);
            Console.WriteLine();

            // Declarar array de bytes para el texto cifrado y asignarle
            // resultado de llamar al metodo Encrypt()
            byte[] textoCifrado = new byte[textoPlano.Length];
            textoCifrado = rsa.Encrypt(textoPlano, true);

            Console.WriteLine("Contenido del texto cifrado: ");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);
            ayuda.GuardaBufer("zz_TextoCifrado.bin", textoCifrado);
            Console.WriteLine();

            rsa.Dispose();
        }
    }
}
