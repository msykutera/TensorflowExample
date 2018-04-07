using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TensorflowExample
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IImageRecognitionService _imageRecognitionService;

        public ValuesController(IImageRecognitionService imageRecognitionService)
        {
            _imageRecognitionService = imageRecognitionService;
        }

        [HttpPost]
        public string Get(IFormFile image)
        {
            try
            {
                var imageBytes = image.ToByteArray();
                var (label, probability) = _imageRecognitionService.Recognize(imageBytes);
                probability = Math.Round(probability, 2);
                return $"Matched element {label} with probability {probability}";
            }
            catch (ImageRecognitionException ex)
            {
                return $"Couldn't match uploaded image. Error details: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"There was an error. Error details: {ex.Message}";
            }
        }
    }
}