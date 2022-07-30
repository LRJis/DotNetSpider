using System;
using System.IO;

namespace LoadJS
{
    public class LocalFile
    {
        public static string Load(string path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }

        public static async void WriteIn(string path, string content)
        {
            while (true)
            {
                try
                {
                    Stream stream = File.Create(path);
                    StreamWriter streamWriter = new StreamWriter(stream);
                    await streamWriter.WriteLineAsync(content);
                    break;
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid char!");
                    path = $@"invalid\{new Random().Next()}.html";
                }
            }
        }
    }
}