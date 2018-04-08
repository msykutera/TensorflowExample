# TensorflowExample
Example of how to use Tensorflow model in .NET Core 2 web app

.NET Core 2 API project that uses TensorflowSharp library in order to run Tensorflow model to recognize objects on uploaded images. For images to test it, pick label from [labels file](https://github.com/msykutera/TensorflowExample/blob/master/TensorflowExample/Assets/imagenet_comp_graph_label_strings.txt) and then put it in  Google Image Search. Look into [ExampleRequest file](https://github.com/msykutera/TensorflowExample/blob/master/TensorflowExample/TensorflowExample.postman_collection.json) for example Postman request.

In case you see the following error:

<i>Unhandled Exception: System.DllNotFoundException: Unable to load DLL 'libtensorflow': The specified module could not be found.</i>

Copy the libtensorflow.dll file from the relevant %userprofile%.nuget\packages\tensorflowsharp runtimes folder of your OS into the same folder as the compiled executable.

Code is partly based on [this repo](https://github.com/daltskin/CustomVision-TensorFlow-CSharp).
