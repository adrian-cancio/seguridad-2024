using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Credenciales;

namespace SerializaXML
{
    /*

    Ahora integra el código necesario para guardar el almacén Alma en un fichero utilizando la 
serialización XML. Comienza integrando en el fichero del proyecto la declaración:
using System.Xml.Serialization
En la clase Program crea el método estático SerializaAlmacenXML() que recibe un objeto de la clase 
Almacen y un string con el nombre de un fichero, por ejemplo “AlmacenUsuarios.xml”. Utiliza una 
sentencia using() en la que debes crear un FileStream. En el cuerpo { } de using() crea el objeto 
SerXML de la clase XmlSerializer, pasándole como argumento el tipo de la clase Almacen usando el 
operador typeof(). Invoca el método Serialize() de SerXML pasándole el FileStream y el objeto de la 
clase Almacen a serializar.
     */
    internal class Program
    {
        static void SerializaAlmacenXML(Almacen Alma, string fichero)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(fichero, System.IO.FileMode.Create))
            {
                System.Xml.Serialization.XmlSerializer SerXML = new System.Xml.Serialization.XmlSerializer(typeof(Almacen));
                SerXML.Serialize(fs, Alma);
            }
        }
        static void Main(string[] args)
        {

            Usuario NuevoUsu1 = new Usuario("Pepe", new byte[] { 1, 2, 3 }, new byte[] { 4, 5, 6 });
            Usuario NuevoUsu2 = new Usuario("Juan", new byte[] { 7, 8, 9 }, new byte[] { 10, 11, 12 });
            Usuario NuevoUsu3 = new Usuario("Luis", new byte[] { 13, 14, 15 }, new byte[] { 16, 17, 18 });
            Usuario NuevoUsu4 = new Usuario("Ana", new byte[] { 19, 20, 21 }, new byte[] { 22, 23, 24 });

            Almacen Almacen = new Almacen();
            Almacen.Add(NuevoUsu1);
            Almacen.Add(NuevoUsu2);
            Almacen.Add(NuevoUsu3);
            Almacen.Add(NuevoUsu4);

            Console.WriteLine(Almacen);

            SerializaAlmacenXML(Almacen, "AlmacenUsuarios.xml");

        }
    }
}
