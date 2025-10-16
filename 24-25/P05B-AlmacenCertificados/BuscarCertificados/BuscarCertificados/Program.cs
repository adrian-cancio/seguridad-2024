using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Programa que demuestra cómo buscar y extraer certificados X.509 
/// del almacén de certificados de Windows.
/// 
/// CERTIFICADOS X.509:
/// Son documentos digitales que vinculan una identidad con una clave pública.
/// Contienen información como:
/// - Sujeto (Subject): a quién pertenece el certificado
/// - Emisor (Issuer): quién firmó el certificado (CA - Autoridad Certificadora)
/// - Clave pública: para cifrar o verificar firmas
/// - Fechas de validez: desde cuándo hasta cuándo es válido
/// - Huella digital (Thumbprint): identificador único del certificado
/// 
/// ALMACENES DE CERTIFICADOS EN WINDOWS:
/// - StoreName.My: certificados personales del usuario
/// - StoreName.Root: autoridades raíz confiables
/// - StoreName.CertificateAuthority: autoridades intermedias
/// - StoreLocation.CurrentUser: almacén del usuario actual
/// - StoreLocation.LocalMachine: almacén del sistema
/// 
/// TIPOS DE BÚSQUEDA (X509FindType):
/// - FindBySubjectName: busca por nombre en el campo Subject
/// - FindBySubjectDistinguishedName: busca por nombre distintivo completo
/// - FindByThumbprint: busca por huella digital (más preciso)
/// - FindByTimeValid: busca certificados válidos en una fecha
/// - FindBySerialNumber: busca por número de serie
/// 
/// Este programa:
/// 1. Abre el almacén de certificados personales del usuario
/// 2. Busca un certificado por su nombre distintivo (DN)
/// 3. Muestra la información del certificado si se encuentra
/// </summary>
namespace BuscarCertificados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Nombre distintivo (DN) del certificado a buscar
            // CN = Common Name (nombre común)
            string nombreSujetoCer = "CN=zpUSU.as";
            
            // Buscar y extraer el certificado
            X509Certificate2 cert = ExtraeCertificado(nombreSujetoCer);
            
            // Si se encontró el certificado, mostrar su información
            if (cert != null)
            {
                Console.WriteLine("Certificado encontrado");
                Console.WriteLine();
                
                // Sujeto: a quién pertenece el certificado
                Console.WriteLine("Sujeto: {0}", cert.SubjectName.Name);
                
                // Emisor: quién firmó/emitió el certificado (CA)
                Console.WriteLine("Emisor: {0}", cert.Issuer);
                
                // Número de serie: identificador único del certificado
                Console.WriteLine("Número de serie: {0}", cert.SerialNumber);
                
                // Fecha desde la cual el certificado es válido
                Console.WriteLine("Válido desde: {0}", cert.NotBefore);
                
                // Fecha hasta la cual el certificado es válido
                Console.WriteLine("Válido hasta: {0}", cert.NotAfter);
                
                // Huella digital: hash único del certificado
                Console.WriteLine("Huella digital: {0}", cert.Thumbprint);
            }
        }

        /// <summary>
        /// Extrae un certificado del almacén de certificados personales
        /// buscando por nombre distintivo
        /// </summary>
        /// <param name="nombreCert">Nombre distintivo del certificado (ej: "CN=zpUSU.as")</param>
        /// <returns>El certificado si se encuentra, null en caso contrario</returns>
        static X509Certificate2 ExtraeCertificado(String nombreCert)
        {
            X509Certificate2 certBuscado = null;
            
            // Abrir el almacén de certificados personales del usuario actual
            // StoreName.My: almacén de certificados personales
            // StoreLocation.CurrentUser: del usuario actual (no del sistema)
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrir el almacén en modo solo lectura
            // ReadOnly: solo lectura
            // OpenExistingOnly: solo si el almacén ya existe (no crear uno nuevo)
            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            
            // Obtener la colección de todos los certificados en el almacén
            X509Certificate2Collection coleCert = almacen.Certificates;
            
            // Cerrar el almacén (liberar recursos)
            almacen.Close();

            // Mostrar cuántos certificados hay en el almacén
            Console.WriteLine("Número de certificados: {0}", coleCert.Count);

            // Buscar certificados que coincidan con el nombre distintivo
            // X509FindType.FindBySubjectDistinguishedName: busca por DN completo
            // nombreCert: el DN a buscar (ej: "CN=zpUSU.as")
            // false: no validar que el certificado sea válido
            X509Certificate2Collection certsEncontrados = coleCert.Find(
                X509FindType.FindBySubjectDistinguishedName, 
                nombreCert, 
                false);
            
            // Procesar los resultados de la búsqueda
            switch (certsEncontrados.Count)
            {
                case 1:
                    // Éxito: se encontró exactamente un certificado
                    certBuscado = certsEncontrados[0];
                    break;
                    
                case 0:
                    // No se encontró ningún certificado
                    Console.WriteLine("No se han encontrado certificados");
                    break;
                    
                default:
                    // Se encontraron múltiples certificados (ambiguo)
                    Console.WriteLine("Se ha encontrado más de un certificado");
                    break;
            }

            return certBuscado;
        }
    }
}
