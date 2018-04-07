using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TensorflowExample
{
    public static class FileStreamExtensions
    {
        public static IEnumerable<string> ReadLines(this FileStream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}