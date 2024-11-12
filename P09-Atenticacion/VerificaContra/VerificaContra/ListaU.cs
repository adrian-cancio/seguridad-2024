﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
{
    internal class ListaU
    {

        // Contantes
        private const int MAX_USU = 5;

        //Atributos
        public User[] lista { get; }

        public ListaU()
        {
            lista = new User[MAX_USU];
        }

        public void IniUsu(int indice, User usuario)
        {
            lista[indice] = usuario;
        }

        public void VerLista()
        {
            int i = 0;
            foreach (User usuario in lista)
            {
                if (usuario != null)
                {
                    Console.WriteLine("----------------------------------------------------------------------");
                    Console.WriteLine("Usuario " + i + ":");
                    Console.WriteLine(usuario);

                }
                i++;
            }
            Console.WriteLine("----------------------------------------------------------------------");
        }

        public void GuardaListaBin(string NombreFich)
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(NombreFich, System.IO.FileMode.Create)))
            {
                foreach (User usuario in lista)
                {
                    if (usuario != null)
                    {
                        writer.Write(usuario.name);
                        writer.Write(usuario.salt);
                        writer.Write(usuario.hash);
                    }
                }
            }
        }

        public void GuardaListaTxt(string NombreFich)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(NombreFich, false, Encoding.ASCII))
            {
                foreach (User usuario in lista)
                {
                    if (usuario != null)
                    {
                        writer.WriteLine(usuario.name);
                        writer.WriteLine(Convert.ToBase64String(usuario.salt));
                        writer.WriteLine(Convert.ToBase64String(usuario.hash));
                    }
                }
            }
        }

        public int Verifica(string name, string password)
        {
            // comprobar si existe el usuario
            foreach (User usuario in lista)
            {
                Console.WriteLine(usuario);
                if (usuario != null)
                {
                    String nameStr = new String(usuario.name);
                    if (nameStr.Equals(name))
                    {
                        // comprobar si la contraseña es correcta
                        if (usuario.Verifica(password))
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
            }
            return 1;
        }
    }
}

