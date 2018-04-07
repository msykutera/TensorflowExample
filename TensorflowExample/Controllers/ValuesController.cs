using ExampleCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TensorFlow;

namespace TensorflowExample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IEnumerable<string> ReadLines(FileStream stream)
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

        [HttpPost]
        public string Get(IFormFile file)
        {
            var graph = new TFGraph();
            var stream = new MemoryStream();

            FileStream fileStream = new FileStream(@"C:\Users\syku\source\repos\TensorflowExample\TensorflowExample\tensorflow_inception_graph.pb", FileMode.Open);
            fileStream.CopyTo(stream);
            var model = stream.ToArray();

            var labelsFile = new FileStream(@"C:\Users\syku\source\repos\TensorflowExample\TensorflowExample\imagenet_comp_graph_label_strings.txt", FileMode.Open);
            var labels = ReadLines(labelsFile).ToList();

            graph.Import(model, "");
            using (var session = new TFSession(graph))
            {
                var tensor = ImageUtil.CreateTensorFromImageFile(file);

                var runner = session.GetRunner();
                runner.AddInput(graph["input"][0], tensor).Fetch(graph["output"][0]);
                var output = runner.Run();
                // output[0].Value() is a vector containing probabilities of
                // labels for each image in the "batch". The batch size was 1.
                // Find the most probably label index.

                var result = output[0];
                var rshape = result.Shape;
                if (result.NumDims != 2 || rshape[0] != 1)
                {
                    var shape = "";
                    foreach (var d in rshape)
                    {
                        shape += $"{d} ";
                    }
                    shape = shape.Trim();
                    Console.WriteLine($"Error: expected to produce a [1 N] shaped tensor where N is the number of labels, instead it produced one with shape [{shape}]");
                    Environment.Exit(1);
                }

                var bestIdx = 0;
                float best = 0;

                var probabilities = ((float[][])result.GetValue(jagged: true))[0];
                for (int i = 0; i < probabilities.Length; i++)
                {
                    if (probabilities[i] > best)
                    {
                        bestIdx = i;
                        best = probabilities[i];
                    }
                }

                return $"{file} best match: [{bestIdx}] {best * 100.0}% {labels[bestIdx]}";
            }
        }
    }
}
