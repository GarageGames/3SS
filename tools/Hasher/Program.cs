using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Hasher
{
    class Program
    {
        static void Main(string[] args)
        {
            string hs = GetHash(".\\Modules\\{WiXTool}\\1\\WiX\\BlankFile.zip");

            string[] files = Directory.GetFiles(".", "*.*", SearchOption.AllDirectories);

            FileStream fs = File.Open("manifest.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (string file in files)
            {
                string hash = GetHash(file);
                sw.WriteLine(file + "," + hash);
            }
            sw.Close();
        }

        static string GetHash(string filename)
        {
            MD5 md5 = MD5.Create();
            byte[] data = File.ReadAllBytes(filename);


            byte[] hash = md5.ComputeHash(data);

            string hashStr = "";
            for (int i = 0; i < hash.Length; i++)
            {
                hashStr += hash[i].ToString("x2");
            }
            return hashStr;
        }
    }
}
