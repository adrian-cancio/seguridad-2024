﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VerificaContra
{
    internal class User
    {
        // Constantes
        public const int NAME_MAX_LENGTH = 16;
        public const int SALT_BYTES = 16;
        public const int HASH_BYTES = 32;
        public const int HASH_ITERATIONS = 1000;
        // enum para elgir como generar el hash (con Rfc2898DeriveBytes o con SHA256Managed)
        public enum HASH_METHOD { RFC, SHA256 };

        // Atributos
        public String Name { get; private set; }
        public byte[] Salt { get; private set; }
        public byte[] Hash { get; private set; }
        public HASH_METHOD HashMethod { get; private set; }

        public User()
        {
            Salt = new byte[SALT_BYTES];
            Hash = new byte[HASH_BYTES];
        }

        public User(String name, String password, HASH_METHOD hashMethod = HASH_METHOD.RFC) : this()
        {
            this.HashMethod = hashMethod;
            this.Name = name;
            this.SetPassword(password);
        }

        public User(String name, byte[] salt, byte[] hash, HASH_METHOD hashMethod = HASH_METHOD.RFC) : this()
        {
            this.HashMethod = hashMethod;
            this.Name = name;
            this.Salt = salt;
            this.Hash = hash;
        }

        public void SetPassword(String password)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(this.Salt);
            if (this.HashMethod == HASH_METHOD.RFC)
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.Salt)
                {
                    IterationCount = HASH_ITERATIONS
                };

                this.Hash = rfc.GetBytes(HASH_BYTES);
            }

            if (this.HashMethod == HASH_METHOD.SHA256)
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordAndSalt = new byte[passwordBytes.Length + this.Salt.Length];
                this.Salt.CopyTo(passwordAndSalt, 0);
                passwordBytes.CopyTo(passwordAndSalt, this.Salt.Length);
                SHA256Managed provSHA = new SHA256Managed();

                this.Hash = provSHA.ComputeHash(passwordAndSalt);


            }

        }

        public bool Verifica(string password)
        {
            byte[] hash = new byte[HASH_BYTES];

            if (HashMethod == HASH_METHOD.RFC)
            {

                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, this.Salt)
                {
                    IterationCount = HASH_ITERATIONS
                };

                hash = rfc.GetBytes(HASH_BYTES);
            }

            if (HashMethod == HASH_METHOD.SHA256)
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] passwordAndSalt = new byte[passwordBytes.Length + this.Salt.Length];
                this.Salt.CopyTo(passwordAndSalt, 0);
                passwordBytes.CopyTo(passwordAndSalt, this.Salt.Length);
                SHA256Managed provSHA = new SHA256Managed();

                hash = provSHA.ComputeHash(passwordAndSalt);
            }


            for (int i = 0; i < HASH_BYTES; i++)
            {
                if (this.Hash[i] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }


        override public String ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Name: ");
            stringBuilder.Append(this.Name);
            stringBuilder.Append("\nSalt: ");
            stringBuilder.Append(BitConverter.ToString(this.Salt).Replace("-", ""));
            stringBuilder.Append("\nHash: ");
            stringBuilder.Append(BitConverter.ToString(this.Hash).Replace("-", ""));
            stringBuilder.Append("\nhashMethod: ");
            stringBuilder.Append(this.HashMethod);
            return stringBuilder.ToString();
        }

        public static int VerificaTxt(String nombreFich, String name, String password, HASH_METHOD hashMethod = HASH_METHOD.RFC)
        {
            if (!System.IO.File.Exists(nombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.StreamReader reader = new System.IO.StreamReader(nombreFich, Encoding.ASCII))
            {
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    line = line.Replace("\0", "");

                    if (line == name)
                    {
                        byte[] salt = Convert.FromBase64String(reader.ReadLine());
                        byte[] hash = Convert.FromBase64String(reader.ReadLine());
                        User user = new User(name, password)
                        {
                            HashMethod = hashMethod,
                            Salt = salt,
                            Hash = hash
                        };
                        if (user.Verifica(password))
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    reader.ReadLine();
                    reader.ReadLine();
                }
            }
            return 1;
        }

        public static int VerificaBin(String nombreFich, String name, String password, HASH_METHOD HashMethod = HASH_METHOD.RFC)
        {
            if (!System.IO.File.Exists(nombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.IO.BinaryReader reader = new System.IO.BinaryReader(System.IO.File.Open(nombreFich, System.IO.FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    String line = reader.ReadString();
                    if (line == name)
                    {
                        byte[] salt = reader.ReadBytes(SALT_BYTES);
                        byte[] hash = reader.ReadBytes(HASH_BYTES);
                        User user = new User(name, password)
                        {
                            HashMethod = HashMethod,
                            Salt = salt,
                            Hash = hash
                        };
                        if (user.Verifica(password))
                        {
                            return 0;
                        }
                        else
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        reader.ReadBytes(SALT_BYTES);
                        reader.ReadBytes(HASH_BYTES);
                    }
                }
            }
            return 1;
        }


        public static int VerificaXML(String nombreFich, String name, String password, HASH_METHOD HashMethod = HASH_METHOD.RFC)
        {
            if (!System.IO.File.Exists(nombreFich))
            {
                throw new ArgumentNullException("File not found");
            }

            using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(nombreFich))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "Name")
                    {
                        if (reader.ReadElementContentAsString() == name)
                        {
                            reader.ReadToFollowing("Salt");
                            byte[] salt = Convert.FromBase64String(reader.ReadElementContentAsString());
                            reader.ReadToFollowing("Hash");
                            byte[] hash = Convert.FromBase64String(reader.ReadElementContentAsString());
                            User user = new User(name, password)
                            {
                                HashMethod = HashMethod,
                                Salt = salt,
                                Hash = hash
                            };
                            if (user.Verifica(password))
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
            }
            return 1;
        }

    }
}
