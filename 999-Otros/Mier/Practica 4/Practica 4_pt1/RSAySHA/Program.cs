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



            // ----------------------------- 2. GENERACION DE CLAVES RSA ----------------------------- //
            Console.WriteLine("----- 2. GENERACION DE CLAVES RSA -----\n");



            // Objeto ayuda para usar todos los metodos de ayuda desarrollados
            Ayuda ayuda = new Ayuda();

            // Objeto proveedor de servicios de criptografía simétrica (sin parametros para que sea por defecto)
            // RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            // Objeto proveedor de servicios de criptografía simétrica (con parametros para que sea de 1024 bits)
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // UNIDADES BASICAS DEL PROVEEDOR
            Console.WriteLine(".--------------------------------.");
            Console.WriteLine("| UNIDADES BASICAS DEL PROVEEDOR |");
            Console.WriteLine("*--------------------------------*");

            Console.Write("Tamaños legales de las claves: ");
            foreach (var keySize in rsa.LegalKeySizes)
            {
                Console.Write($"Mínimo: {keySize.MinSize}, Máximo: {keySize.MaxSize}, Incremento: {keySize.SkipSize}\n");
            }
            Console.WriteLine("Tamaño clave actual: " + rsa.KeySize);
            Console.WriteLine("Ver si la clave se conserva en el proveedor actual: " + rsa.PersistKeyInCsp);
            Console.WriteLine();

            // Exportar las claves formato XML
            Console.WriteLine("Exportación claves formato xml: ");
            Console.WriteLine(rsa.ToXmlString(true));

            // Este código esta exportando las claves (pública y privada) del proveedor RSA y guardandolas en el objeto parameters
            RSAParameters parameters = rsa.ExportParameters(true);

            // El tamaño del modulo de la clave equivale al tamaño del texto cifrado
            Console.WriteLine("\nModulo RSA: ");
            ayuda.WriteHex(parameters.Modulus, parameters.Modulus.Length);

            // Exponente publico (exponent) y privado (D)
            Console.WriteLine("\nExponente publico: ");
            ayuda.WriteHex(parameters.Exponent, parameters.Exponent.Length);
            Console.WriteLine("\nExponente privado: ");
            ayuda.WriteHex(parameters.D, parameters.D.Length);

            // Primos P y Q
            Console.WriteLine("\nP: ");
            ayuda.WriteHex(parameters.P, parameters.P.Length);
            Console.WriteLine("\nQ: ");
            ayuda.WriteHex(parameters.Q, parameters.Q.Length);

            // Guardar los cinco elementos de la clave en un fichero txt

            // Guardar el modulo
            ayuda.GuardaTxt("n.txt", parameters.Modulus);
            // Guardar el exponente publico
            ayuda.GuardaTxt("e.txt", parameters.Exponent);
            // Guardar el exponente privado
            ayuda.GuardaTxt("d.txt", parameters.D);
            // Guardar el primo P
            ayuda.GuardaTxt("p.txt", parameters.P);
            // Guardar el primo Q
            ayuda.GuardaTxt("q.txt", parameters.Q);

            // Exportar las claves a un objero binario
            ayuda.GuardaBufer("zz_BlobRSA_Priva.bin", rsa.ExportCspBlob(true)); // Con clave privada
            ayuda.GuardaBufer("zz_BlobRSA_Publi.bin", rsa.ExportCspBlob(false)); // Sin clave privada

            // Comprobar que las claves se generan y exportan de forma correcta
            RSACryptoServiceProvider rsa2 = new RSACryptoServiceProvider();
            long size = ayuda.BytesFichero("zz_BlobRSA_Priva.bin");
            byte[] buffer = new byte[size];

            ayuda.CargaBufer("zz_BlobRSA_Priva.bin", buffer);
            Console.WriteLine("\nClave importada: ");
            ayuda.WriteHex(buffer, buffer.Length);
            Console.WriteLine();

            // Importar clave al nuevo proveedor RSA
            rsa2.ImportCspBlob(buffer);

            // Se utilizan para liberar los recursos no administrados que están siendo utilizados por los objetos rsa y rsa2,
            // que son instancias de clases que implementan la interfaz IDisposable.
            rsa.Dispose();
            rsa2.Dispose();

        }
    }
}
