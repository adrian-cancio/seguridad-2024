using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

/// <summary>
/// Programa que demuestra el cifrado de texto usando RSA con clave pública.
/// 
/// PROCESO DE CIFRADO RSA:
/// 1. Cargar la clave pública desde un archivo blob
/// 2. Solicitar texto plano al usuario
/// 3. Cifrar el texto con la clave pública
/// 4. Guardar el texto cifrado en un archivo
/// 
/// CONCEPTOS IMPORTANTES:
/// - Solo se necesita la clave pública para cifrar
/// - El texto cifrado solo puede descifrarse con la clave privada correspondiente
/// - RSA tiene limitaciones de tamaño: no puede cifrar datos mayores que el tamaño de clave
///   (por eso se usa típicamente para cifrar claves simétricas, no datos grandes)
/// 
/// PARÁMETRO OAEP:
/// - true: usa OAEP (Optimal Asymmetric Encryption Padding) - más seguro, recomendado
/// - false: usa PKCS#1 v1.5 padding - menos seguro, compatible con sistemas antiguos
/// 
/// Este programa:
/// 1. Busca la clave pública más reciente en la carpeta Compartido
/// 2. Importa la clave pública desde el blob
/// 3. Muestra los componentes públicos (e y n)
/// 4. Cifra un texto introducido por el usuario
/// 5. Guarda el texto cifrado en un archivo con timestamp
/// </summary>
namespace CifradoRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear instancia de la clase de ayuda
            Ayuda ayuda = new Ayuda();
            
            // Crear un proveedor RSA de 1024 bits
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            Console.WriteLine("Longitud de la clave: {0} bits", rsa.KeySize);

            // ==================== BUSCAR Y CARGAR CLAVE PÚBLICA ====================
            
            // Ruta a la carpeta compartida donde están las claves
            String rutaBase = "..\\..\\..\\..\\Compartido\\";
            String rutaBaseAbsoluta = Path.GetFullPath(rutaBase);
            
            // Verificar que existe el directorio
            if (!Directory.Exists(rutaBase))
            {
                Console.WriteLine("No se encuentra la carpeta Compartido en la ruta: {0}", rutaBaseAbsoluta);
                return;
            }

            // Buscar archivos de clave pública con el patrón zz_BlobRSA*_Publi.bin
            String[] listaFicheros = Directory.GetFiles(rutaBase, "zz_BlobRSA*_Publi.bin");

            // Elegir la última clave pública generada (la más reciente)
            String nombreFichBlobPubli = listaFicheros[listaFicheros.Length - 1];

            // Cargar el blob de la clave pública desde el archivo
            byte[] blobFich = new byte[ayuda.BytesFichero(nombreFichBlobPubli)];
            ayuda.CargaBufer(nombreFichBlobPubli, blobFich);

            // Importar la clave pública al proveedor RSA
            rsa.ImportCspBlob(blobFich);

            // ==================== MOSTRAR COMPONENTES DE LA CLAVE PÚBLICA ====================
            
            // Exportar solo los parámetros públicos (false = no incluir privados)
            RSAParameters rsaParameters = rsa.ExportParameters(false);

            Console.WriteLine("Exponente público:");
            ayuda.WriteHex(rsaParameters.Exponent, rsaParameters.Exponent.Length);
            Console.WriteLine("Módulo:");
            ayuda.WriteHex(rsaParameters.Modulus, rsaParameters.Modulus.Length);
            Console.WriteLine();

            // ==================== SOLICITAR Y CIFRAR TEXTO ====================
            
            // Solicitar texto plano al usuario
            Console.WriteLine("Introduce el texto plano: ");
            String textoPlanoStr = Console.ReadLine();
            
            // Convertir el texto a bytes usando UTF-8
            byte[] textoPlano = Encoding.UTF8.GetBytes(textoPlanoStr);

            Console.WriteLine();
            Console.WriteLine("Texto plano en bytes:");
            ayuda.WriteHex(textoPlano, textoPlano.Length);
            Console.WriteLine();

            // Cifrar el texto usando RSA
            // Parámetros:
            //   - textoPlano: datos a cifrar
            //   - true: usar OAEP padding (más seguro)
            //   - false: usar PKCS#1 v1.5 padding (menos seguro, compatible)
            byte[] textoCifrado = rsa.Encrypt(textoPlano, true);

            Console.WriteLine("Texto cifrado:");
            ayuda.WriteHex(textoCifrado, textoCifrado.Length);
            Console.WriteLine();

            // ==================== GUARDAR TEXTO CIFRADO ====================
            
            // Crear nombre de archivo con timestamp
            DateTime dateTime = DateTime.Now;
            String nombreFichCifrado = "zz_TextoCifrado_"+ dateTime.ToString("yyyyMMddHHmmss") + ".bin";
            
            Console.WriteLine("Guardando el texto cifrado en el fichero:\n{0}", rutaBaseAbsoluta+nombreFichCifrado);

            // Guardar el texto cifrado en la carpeta compartida
            ayuda.GuardaBufer(rutaBase+nombreFichCifrado, textoCifrado);
        }
    }
}
