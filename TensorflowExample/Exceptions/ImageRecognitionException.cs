using System;

namespace TensorflowExample
{
    public class ImageRecognitionException : Exception
    {
        public ImageRecognitionException(string message) : base(message) { }
    }
}
