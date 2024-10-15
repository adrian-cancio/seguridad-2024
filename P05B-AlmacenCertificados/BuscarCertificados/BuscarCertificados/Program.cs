using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace BuscarCertificados
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string nombreSujetoCer = "CN=zpUSU.as";
            X509Certificate2 cert = ExtraeCertificado(nombreSujetoCer);
            if (cert != null)
            {
                Console.WriteLine("Certificado encontrado");
                Console.WriteLine();
                Console.WriteLine("Sujeto: {0}", cert.SubjectName);
                Console.WriteLine("Emisor: {0}", cert.Issuer);
                Console.WriteLine("Número de serie: {0}", cert.SerialNumber);
                Console.WriteLine("Válido desde: {0}", cert.NotBefore);
                Console.WriteLine("Válido hasta: {0}", cert.NotAfter);
                Console.WriteLine("Huella digital: {0}", cert.Thumbprint);
            }
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
    }
}
