using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Apoyo;

namespace FIRMA
{
    class Program
    {
        static void Main(string[] args)
        {
            Ayuda helper = new Ayuda();

            // Crea el mensaje
            byte[] Msg = new byte[64];
            for (int i = 0; i < 64; i++)
            {
                Msg[i] = (byte)i;
            }
            helper.WriteHex(Msg, Msg.Length);
            helper.GuardaBufer("zzMsgPlano.bin", Msg);

            // Carga los certificados
            X509Certificate2 CerEmi = ExtraeCertificado("zye.as");
            X509Certificate2 CerRec = ExtraeCertificado("zyr.as");
            Console.WriteLine(CerEmi.Subject);
            Console.WriteLine(CerRec.Subject);

            // CIFRAR EL MENSAJE con la clave pública del receptor
            byte[] MsgCifrado; // Declare MsgCifrado here
            using (RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)CerRec.PublicKey.Key)
            {
                MsgCifrado = rsa.Encrypt(Msg, true); // OAEP padding
                helper.WriteHex(MsgCifrado, MsgCifrado.Length);
                helper.GuardaBufer("zzMsgCifrado.bin", MsgCifrado);
            }

            // FIRMAR EL MENSAJE CIFRADO con la clave privada del emisor
            using (RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)CerEmi.PrivateKey)
            {
                SHA512 sha512 = SHA512.Create();
                byte[] Resumen = sha512.ComputeHash(MsgCifrado);
                byte[] firma = rsa.SignHash(Resumen, CryptoConfig.MapNameToOID("SHA512"));
                helper.WriteHex(firma, firma.Length);
                helper.GuardaBufer("zzFirma.bin", firma);
            }
        }

        private static X509Certificate2 ExtraeCertificado(string subjectName)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, false);

            if (certs.Count == 0)
            {
                Console.WriteLine($"Certificado '{subjectName}' no encontrado.");
                Environment.Exit(1);
                return null;
            }
            else if (certs.Count > 1)
            {
                Console.WriteLine($"Se encontró más de un certificado con el nombre '{subjectName}'.");
                Environment.Exit(1);
                return null;
            }
            else
            {
                return certs[0];
            }
        }
    }
}
