using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TensorFlow;

namespace TensorflowExample
{
    public class ImageRecognitionService : IImageRecognitionService
    {
        private readonly TFGraph _graph = new TFGraph();
        private readonly List<string> _labels = new List<string>();
        private readonly string _basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public ImageRecognitionService()
        {
            LoadGraph();
            LoadLabels();
        }

        private void LoadGraph()
        {
            var path = GetAssetPath("tensorflow_inception_graph.pb");
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var stream = new MemoryStream())
                {
                    fileStream.CopyTo(stream);
                    var model = stream.ToArray();
                    _graph.Import(model);
                }
            }
        }

        private void LoadLabels()
        {
            var path = GetAssetPath("imagenet_comp_graph_label_strings.txt");
            using (var labelsFile = new FileStream(path, FileMode.Open))
            {
                var labels = labelsFile.ReadLines();
                _labels.AddRange(labels);
            }
        }

        private bool ValidateResult(TFTensor result, out string errorMessage)
        {
            var rshape = result.Shape;

            if (result.NumDims != 2 || rshape[0] != 1)
            {
                var shape = "";
                foreach (var d in rshape) shape += $"{d} ";
                shape = shape.Trim();

                errorMessage = $"Expected to produce a [1 N] shaped tensor where N is the number of labels, instead it produced one with shape [{shape}]";
                return false;
            }

            errorMessage = string.Empty;
            return true;
        }

        private string GetAssetPath(string assetName)
        {
            var result = $"{_basePath}/Assets/{assetName}";
            return result;
        }

        public (string label, double probability) Recognize(byte[] image)
        {
            var tensor = ImageUtil.CreateTensorFromImageFile(image);
            using (var session = new TFSession(_graph))
            {
                var runner = session.GetRunner();
                runner.AddInput(_graph["input"][0], tensor).Fetch(_graph["output"][0]);
                var output = runner.Run();
                var result = output.First();

                var valid = ValidateResult(result, out string errorMessage);
                if (!valid) throw new ImageRecognitionException(errorMessage);

                var probabilities = ((float[][]) result.GetValue(jagged: true))[0].ToList();
                var bestProbability = probabilities.Max(x => x);
                var bestIndex = probabilities.IndexOf(bestProbability);
                var label = _labels[bestIndex];

                return (label, bestProbability * 100.0);
            }
        }
    }
}
