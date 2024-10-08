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
            ExtraeCertificado("");
        }

        static X509Certificate2 ExtraeCertificado(String nombreCert)
        {
            X509Store almacen = new X509Store(StoreName.My, StoreLocation.CurrentUser);

            //Console.WriteLine(almacen.Name);
            //Console.WriteLine(almacen.Location);

            almacen.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection coleCert = almacen.Certificates;
            almacen.Close();
            foreach (X509Certificate cert in coleCert)
            {
                Console.WriteLine(cert.Subject);
            }
            return null;
        }
    }
}
