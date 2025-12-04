using Apoyo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace UsoCertificado
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string nombreCert = "CN=zpUSU.as";
            X509Certificate2 cert = ExtraeCertificado(nombreCert);
            if (cert != null)
            {
                Console.WriteLine("Certificado encontrado");
                Console.WriteLine();
                Console.WriteLine("Sujeto: {0}", cert.SubjectName.Name);
                Console.WriteLine("Emisor: {0}", cert.Issuer);
                Console.WriteLine("Número de serie: {0}", cert.SerialNumber);
                Console.WriteLine("Válido desde: {0}", cert.NotBefore);
                Console.WriteLine("Válido hasta: {0}", cert.NotAfter);
                Console.WriteLine("Huella digital: {0}", cert.Thumbprint);
                Console.WriteLine("¿Tiene clave privada?: {0}", (cert.HasPrivateKey ? "Sí" : "No"));
                Console.WriteLine();
            }

            
            RSACryptoServiceProvider provRSA1 = (RSACryptoServiceProvider) cert.PublicKey.Key;
            //VerParam(provRSA1, false);
            

            RSACryptoServiceProvider provRSA2 = (RSACryptoServiceProvider)cert.PrivateKey;
            if (provRSA2 == null)
            {
                Console.WriteLine("El certificado seleccionado no tiene clave privada");
                Environment.Exit(0);
            }
            Console.WriteLine("Se ha creado un proveedor con una clave privada:");
            VerParam(provRSA2 , true);
            Console.WriteLine();

            // Cifrado con clave publica 

            byte[] msg = new byte[64];
            for (int i = 0; i < msg.Length; i++)
            {
                msg[i] = (byte)i;
            }

            Ayuda ayuda = new Ayuda();

            Console.WriteLine("Mensaje sin cifrar: ");
            ayuda.WriteHex(msg, msg.Length);
            Console.WriteLine();

            byte[] msgEnc = provRSA1.Encrypt(msg, true);

            Console.WriteLine("Mensaje cifrado: ");
            ayuda.WriteHex(msgEnc, msgEnc.Length);
            Console.WriteLine();

            // Descifrado con clave publica

            byte[] msgDec = provRSA2.Decrypt(msgEnc, true);
            Console.WriteLine("Mensaje descifrado: ");
            ayuda.WriteHex(msgDec, msgDec.Length);
            Console.WriteLine();

            // Firma con clave privada

            byte[] firma = provRSA2.SignData(msg, "SHA1");
            Console.WriteLine("Firma del mensaje: ");
            ayuda.WriteHex(firma, firma.Length);
            Console.WriteLine();

            // Verificando la firma con la clave pública
            
            // msg[0] = 0xFF;
            // [0] = 0xFF;

            bool verifica = provRSA1.VerifyData(msg, "SHA1", firma);
            Console.WriteLine("¿Firma verificada?: {0}", (verifica ? "Sí" : "No"));
            Console.WriteLine();



        }

        static X509Certificate2 ExtraeCertificado(String nombreCert)
        {
            X509Certificate2 certBuscado = null;
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            //Console.WriteLine(almacen.Name);
            //Console.WriteLine(almacen.Location);

            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection coleCert = almacen.Certificates;
            almacen.Close();

            /*
            foreach (X509Certificate cert in coleCert)
            {
                Console.WriteLine(cert.Subject);
            }
            */

            Console.WriteLine("Numero de certificados: {0}", coleCert.Count);

            //X509Certificate2Collection certsEncontrados = coleCert.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
            //X509Certificate2Collection certsEncontrados = coleCert.Find(X509FindType.FindBySubjectName, nombreCert, false);

            X509Certificate2Collection certsEncontrados = coleCert.Find(X509FindType.FindBySubjectDistinguishedName, nombreCert, false);
            switch (certsEncontrados.Count)
            {
                case 1:
                    certBuscado = certsEncontrados[0];
                    break;
                case 0:
                    Console.WriteLine("No se han encontrado certificados");
                    break;
                default:
                    Console.WriteLine("Se ha encontrado mas de un certificado");
                    break;
            }


            return certBuscado;
        }

        internal static void VerParam(RSACryptoServiceProvider ProRSA, bool ExportaPriv)
        {
            RSAParameters ParamRSA = new RSAParameters();
            try
            {
                ParamRSA = ProRSA.ExportParameters(ExportaPriv);
            }
            catch (Exception e)
            {
                Console.WriteLine("EXCEPCION: " + e);
                Environment.Exit(1);
            }

            Console.WriteLine("\n--INICIO CLAVE RSA---------------------------");

            Console.WriteLine("Modulo (n) {0} bytes", ParamRSA.Modulus.Length);
            for (int i = 0; i < ParamRSA.Modulus.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Modulus[i]);

            Console.WriteLine("\nExponente Publico (e) {0} bytes", ParamRSA.Exponent.Length);
            for (int i = 0; i < ParamRSA.Exponent.Length; i++)
                Console.Write("{0,2:X} ", ParamRSA.Exponent[i]);

            if (ExportaPriv)
            {
                Console.WriteLine("\nExponente Privado (d) {0} bytes", ParamRSA.D.Length);
                for (int i = 0; i < ParamRSA.D.Length; i++)
                    Console.Write("{0,2:X} ", ParamRSA.D[i]);
            }

            Console.WriteLine("\n--FIN CLAVE RSA------------------------------");
        }
    }


}
