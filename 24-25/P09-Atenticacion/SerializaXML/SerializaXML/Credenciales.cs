using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credenciales
{
    [Serializable] public class Almacen
    {
        public List<Usuario> Lista;
        public Almacen()
        {
            Lista = new List<Usuario>();
        }

        public void Add(Usuario usu)
        {
            Lista.Add(usu);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Usuario usu in Lista)
            {
                sb.AppendLine(usu.ToString());
            }
            return sb.ToString();
        }
    }

    [Serializable] public class Usuario
    {
        public string Nombre;
        public byte[] Salt;
        public byte[] ResuContra;

        public Usuario()
        {
        }

        public Usuario(string nombre, byte[] salt, byte[] resuContra)
        {
            Nombre = nombre;
            Salt = salt;
            ResuContra = resuContra;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Nombre: {0}",Nombre));
            
            sb.AppendLine("Salt: ");
            for (int i = 0; i < Salt.Length; i++)
            {
                sb.Append(Salt[i]+" ");
            }
            sb.AppendLine();

            sb.AppendLine("Resumen de la contraseña: ");
            for (int i = 0; i < ResuContra.Length; i++)
            {
                sb.Append(ResuContra[i]+" ");
            }
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
