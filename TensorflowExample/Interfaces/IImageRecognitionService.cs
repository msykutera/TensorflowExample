namespace TensorflowExample
{
    public interface IImageRecognitionService
    {
        (string label, double probability) Recognize(byte[] image);
    }
}
