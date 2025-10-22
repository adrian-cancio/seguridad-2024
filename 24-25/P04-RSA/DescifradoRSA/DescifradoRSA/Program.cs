using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

/// <summary>
/// Programa que demuestra el descifrado de texto usando RSA con clave privada.
/// 
/// PROCESO DE DESCIFRADO RSA:
/// 1. Cargar la clave privada desde un archivo blob
/// 2. Buscar archivos de texto cifrado
/// 3. Descifrar cada archivo usando la clave privada
/// 4. Guardar los textos descifrados
/// 
/// CONCEPTOS IMPORTANTES:
/// - Se necesita la clave PRIVADA para descifrar
/// - Solo el poseedor de la clave privada puede descifrar
/// - El texto cifrado con la clave pública solo se puede descifrar con la privada correspondiente
/// - Esto garantiza confidencialidad: solo el destinatario puede leer el mensaje
/// 
/// FLUJO TÍPICO DE CIFRADO/DESCIFRADO RSA:
/// 1. Generador: crea par de claves (pública/privada)
/// 2. Generador: comparte la clave pública
/// 3. Cifrador: usa clave pública para cifrar mensaje
/// 4. Descifrador: usa clave privada para descifrar mensaje
/// 
/// Este programa:
/// 1. Carga la clave privada más reciente
/// 2. Muestra los componentes de la clave privada
/// 3. Busca todos los archivos cifrados en la carpeta compartida
/// 4. Descifra solo los que aún no han sido descifrados
/// 5. Guarda cada texto descifrado en un archivo .txt
/// </summary>
namespace DescifradoRSA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Crear instancia de la clase de ayuda
            Ayuda ayuda = new Ayuda();
            
            // Crear proveedor RSA de 1024 bits
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);

            // ==================== CARGAR CLAVE PRIVADA ====================
            
            // Ruta a la carpeta compartida
            String rutaBase = "..\\..\\..\\..\\Compartido\\";
            
            // Buscar archivos de clave privada con el patrón zz_BlobRSA*_Priv.bin
            String[] listaFicheros = Directory.GetFiles(rutaBase, "zz_BlobRSA*_Priv.bin");

            // Elegir la última clave privada generada (la más reciente)
            String nombreFichBlobPriv = listaFicheros[listaFicheros.Length - 1];

            // Cargar el blob de la clave privada desde el archivo
            byte[] fichBlobPriv = new byte[ayuda.BytesFichero(nombreFichBlobPriv)];
            ayuda.CargaBufer(nombreFichBlobPriv, fichBlobPriv);

            // Importar la clave privada al proveedor RSA
            rsa.ImportCspBlob(fichBlobPriv);

            // ==================== MOSTRAR COMPONENTES DE LA CLAVE ====================
            
            // Exportar todos los parámetros (true = incluir privados)
            RSAParameters rsaParameters = rsa.ExportParameters(true);

            // Mostrar componentes públicos
            Console.WriteLine("Exponente público:");
            ayuda.WriteHex(rsaParameters.Exponent, rsaParameters.Exponent.Length);
            
            // Mostrar componentes privados
            Console.WriteLine("Exponente privado:");
            ayuda.WriteHex(rsaParameters.D, rsaParameters.D.Length);  
            Console.WriteLine("Módulo:");
            ayuda.WriteHex(rsaParameters.Modulus, rsaParameters.Modulus.Length);
            Console.WriteLine("Primo P:");
            ayuda.WriteHex(rsaParameters.P, rsaParameters.P.Length);
            Console.WriteLine("Primo Q:");
            ayuda.WriteHex(rsaParameters.Q, rsaParameters.Q.Length);
            Console.WriteLine();

            // ==================== DESCIFRAR ARCHIVOS ====================
            
            // Buscar todos los archivos de texto cifrado
            String[] fichCifrados = Directory.GetFiles(rutaBase, "zz_TextoCifrado_*.bin");
            
            byte[] textoCifrado;
            byte[] textoPlano;
            
            // Recorrer todos los archivos cifrados
            for (int i = 0; i < fichCifrados.Length; i++)
            {
                // Construir el nombre del archivo descifrado
                // Reemplazar "TextoCifrado" por "TextoDescifrado" y .bin por .txt
                String nombreFichDescif = fichCifrados[i].Replace("TextoCifrado", "TextoDescifrado");
                nombreFichDescif = nombreFichDescif.Replace(".bin", ".txt");

                // Comprobar si ya existe el archivo descifrado (evitar procesar dos veces)
                if (!File.Exists(nombreFichDescif))
                {
                    // Cargar el texto cifrado
                    textoCifrado = new byte[ayuda.BytesFichero(fichCifrados[i])];
                    ayuda.CargaBufer(fichCifrados[i], textoCifrado);
                    
                    Console.WriteLine("Texto cifrado:");
                    ayuda.WriteHex(textoCifrado, textoCifrado.Length);
                    Console.WriteLine();

                    // Descifrar el texto usando RSA
                    // Parámetros:
                    //   - textoCifrado: datos cifrados a descifrar
                    //   - true: usar OAEP padding (debe coincidir con el usado al cifrar)
                    textoPlano = rsa.Decrypt(textoCifrado, true);
                    
                    Console.WriteLine("Texto descifrado:");
                    ayuda.WriteHex(textoPlano, textoPlano.Length);

                    // Guardar el texto descifrado
                    ayuda.GuardaBufer(nombreFichDescif, textoPlano);
                }
            }
        }
    }
}
