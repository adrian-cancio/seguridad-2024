using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Apoyo;

namespace UsoCerficados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Clase de ayuda
            Ayuda a = new Ayuda();

            // Crear un array de 64 bytes
            byte[] Msg = new byte[64];
            for (int i = 0; i < Msg.Length; i++)
            {
                Msg[i] = Convert.ToByte(i);
            }



            // ---------- FASE 1: ACCESO A UN ALMACÉN DE CERTIFICADOS ---------- //
            Console.WriteLine("\nFASE 1: ACCESO A UN ALMACÉN DE CERTIFICADOS\n");



            // Nombre del certificado. Debe estar importado en el almacen de certificados del usuario y la clave debe estar marcada como exportable o no funcionara el codigo
            string NombreCert = "zpusu.as";

            // Declarar objeto Cert de la clase X509Certificate2
            X509Certificate2 cert = ExtraeCert(NombreCert);

            Console.WriteLine("Se ha creado correctamente el objeto cert de la clase X509Certificate2 y se ha extraido el cerficado");



            // ---------- FASE 2: Uso del certificado ---------- //
            Console.WriteLine("\nFASE 2: USO DEL CERTIFICADO\n");



            // Mostrar propiedades del certificado
            Console.WriteLine("Propiedades del Certificado");

            // Mostrar el sujeto (Subject) del certificado, que identifica al titular del certificado
            Console.Write("Sujeto del Certificado (Subject): ");
            Console.WriteLine(cert.Subject);

            Console.WriteLine();

            // Indica si el certificado contiene una clave privada asociada
            Console.WriteLine("¿Contiene clave privada?");
            Console.WriteLine(cert.HasPrivateKey ? "Sí, el certificado contiene una clave privada." : "No, el certificado no contiene una clave privada.");

            if (cert.HasPrivateKey)
            {
                // Mostrar el algoritmo de clave del certificado, que indica qué algoritmo de cifrado se utiliza (por ejemplo, RSA)
                Console.Write("Algoritmo de clave del certificado: ");
                Console.WriteLine(cert.GetKeyAlgorithm());

                // Mostrar la longitud en bytes del nombre del algoritmo de clave del certificado
                Console.Write("Longitud del nombre del algoritmo de clave: ");
                Console.WriteLine(cert.GetKeyAlgorithm().Length);
            }

            // 1) Declarar un objeto ProvRSA1 y asígnale en la misma declaración la clave pública contenida en Cert
            RSACryptoServiceProvider ProvRSA1 = (RSACryptoServiceProvider)cert.PublicKey.Key;

            // Llama al método VerParam para mostrar solo los parámetros públicos de la clave RSA
            Console.Write("\nA continuacion, se muestran los parámetros públicos de la clave RSA (módulo y exponente público), sin incluir el exponente privado. (Especificado con false en la llamada del metodo) \n");
            VerParam(ProvRSA1, false);

            // 2) Declarar un objeto ProvRSA2 y asígnale en la misma declaración la clave privada asociada a Cert
            RSACryptoServiceProvider ProvRSA2 = (RSACryptoServiceProvider)cert.PrivateKey;

            //----La línea de arriba puede dar problemas al usar las claves. Se soluciona así:----//
            string ClavePriXML = cert.PrivateKey.ToXmlString(true);
            Console.WriteLine("\nClave Privada en XML: ");
            Console.WriteLine();
            Console.WriteLine(ClavePriXML);
            ProvRSA2 = new RSACryptoServiceProvider(2048);
            ProvRSA2.FromXmlString(ClavePriXML);
            //----Hasta aqui la solucion del error----//

            // 3) Comprueba si ProvRSA2 == null. 
            if (ProvRSA2 == null)
            {
                // Si no se ha creado
                Console.WriteLine("Error");
                return;
            }
            // Si se ha creado correctamente
            Console.WriteLine("\n[Se ha creado un proveedor con una clave privada]");

            Console.WriteLine("\nA continuación, se muestran todos los parámetros de la clave RSA, incluyendo el módulo, el exponente público y el exponente privado, ya que se ha especificado 'true' en la llamada al método.");
            VerParam(ProvRSA2, true);

            // 4) Cifrar con clave pública
            byte[] textoCifrado = ProvRSA1.Encrypt(Msg, true); //true: OAEP, false: PKCS#1 v1.5
            Console.WriteLine("\n[Texto cifrado]:");
            a.WriteHex(textoCifrado, textoCifrado.Length);

            // 5) Descifrar con clave privada
            byte[] textoDescifrado = ProvRSA2.Decrypt(textoCifrado, true);
            Console.WriteLine("\n[Texto descifrado]:");
            a.WriteHex(textoDescifrado, textoDescifrado.Length);

            // 6) Firmar con clave privada
            byte[] firma = ProvRSA2.SignData(Msg, "SHA1");
            Console.WriteLine("\nResultado Firma 1: ");
            a.WriteHex(firma, firma.Length);

            // 7) Estropear el Msg / Firma
            //Msg[0] = 0xFF;
            //firma[0] = 0xFF;

            // 8) Verificar con la clave publica
            //ProvRSA1 tiene solo clave publica (puede cifrar y verificar firma pero no descifrar ni firmar)
            //ProvRSA2 tiene clave publica y privada (puede hacer todas)
            bool Verifica = ProvRSA1.VerifyData(Msg, "SHA1", firma);
            Console.WriteLine("\n[FIRMA VERIFICADA]: " + Verifica);
            Console.WriteLine();
        }

        /*
         * Función: Este método permite ver los valores de los componentes principales de una clave RSA.
           Control de exportación: Si ExportaPriv es false, solo se muestra el módulo y el exponente público, pero si es true, también se muestra el exponente privado.
           Salida: La información de la clave se imprime en formato hexadecimal en la consola.
        */
        internal static void VerParam(RSACryptoServiceProvider ProRSA, bool ExportaPriv)
        {
            RSAParameters ParamRSA = new RSAParameters();

            try
            {
                // Exporta los parámetros de la clave pública o ambas claves (pública y privada)
                ParamRSA = ProRSA.ExportParameters(ExportaPriv);
            }
            catch (Exception e)
            {
                // Captura errores de exportación y termina el programa
                Console.WriteLine("EXCEPCION: " + e);
                Environment.Exit(1);
            }

            Console.WriteLine("\n\n--INICIO CLAVE RSA---------------------------\n");

            // Muestra el módulo (n) de la clave
            Console.WriteLine("Modulo (n) {0} bytes", ParamRSA.Modulus.Length);
            for (int i = 0; i < ParamRSA.Modulus.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Modulus[i]);

            // Muestra el exponente público (e)
            Console.WriteLine("\n\nExponente Publico (e) {0} bytes", ParamRSA.Exponent.Length);
            for (int i = 0; i < ParamRSA.Exponent.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Exponent[i]);

            // Si se permite exportar la clave privada, muestra el exponente privado (d)
            if (ExportaPriv)
            {
                Console.WriteLine("\n\nExponente Privado (d) {0} bytes", ParamRSA.D.Length);
                for (int i = 0; i < ParamRSA.D.Length; i++)
                    Console.Write("{0,2:X} ", ParamRSA.D[i]);
            }

            Console.WriteLine("\n\n--FIN CLAVE RSA------------------------------\n");
        }

        static X509Certificate2 ExtraeCert(string Nombre)
        {
            // Obtener el almacen de certificados
            X509Store almacenUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            // Abrimos almacen en modo lectura y solo almacenes existentes
            almacenUser.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

            // Extraemos todos los certificados del almacen del usuario
            X509Certificate2Collection CertsUser = almacenUser.Certificates;

            // Asignamos a cert el certificado solicitado (true solo para certificados válidos)
            X509Certificate2 cert = CertsUser.Find(X509FindType.FindBySubjectName, Nombre, false)[0];
            
            return cert;
        }
    }
}
