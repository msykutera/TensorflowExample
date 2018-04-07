using Microsoft.AspNetCore.Http;
using System.IO;

namespace TensorflowExample
{
    public static class FormFileExtensions
    {
        public static byte[] ToByteArray(this IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var result = stream.ToArray();
                return result;
            }
        }
    }
}
