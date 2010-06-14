using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

namespace GEarthNawiigator.NeuralNetwork
{
    public class RBFNetwork
    {
        private int _inputSize;
        public int InputSize
        {
            get { return _inputSize; }
            set { _inputSize = value; }
        }

        private int _hiddenSize;
        public int HiddenSize
        {
            get { return _hiddenSize; }
            set { _hiddenSize = value; }
        }

        private int _outputSize;
        public int OutputSize
        {
            get { return _outputSize; }
            set { _outputSize = value; }
        }

        private double[][] _centers;
        public double[][] Centers
        {
            get { return _centers; }
            set { _centers = value; }
        }

        private int[][] _weightIndex;
        public int[][] WeightIndex
        {
            get { return _weightIndex; }
            set { _weightIndex = value; }
        }

        double[] _weights;
        public double[] Weights
        {
            get { return _weights; }
            set { _weights = value; }
        }

        double[] _lambdas;
        public double[] Lambdas
        {
            get { return _lambdas; }
            set { _lambdas = value; }
        }

        double[] _alphas;
        public double[] Alphas
        {
            get { return _alphas; }
            set { _alphas = value; }
        }

        double _bias = 0;

        public bool Bias
        {
            get { return _bias != 0; }
            set { _bias = (value) ? 1 : 0; }
        }

        public RBFNetwork(int inputSize, int hiddenSize, int outputSize, double[][] centers, int[][] weightIndex, double[] weights, double[] lambdas, double[] alphas, bool bias)
        {
            _inputSize = inputSize;
            _hiddenSize = hiddenSize;
            _outputSize = outputSize;
            _centers = centers;
            _weightIndex = weightIndex;
            _weights = weights;
            _lambdas = lambdas;
            _alphas = alphas;
            if (bias)
                _bias = 1;
            else
                _bias = 0;
        }

        public RBFNetwork(string xmlFile)
        {
            FromXml(xmlFile);
        }

        public void FromXml(string xmlFile)
        {
            int inputSize;
            int hiddenSize;
            int outputSize;
            double[][] centers;
            int[][] weightIndex;
            double[] weights;
            double[] lambdas;
            double[] alphas;
            bool bias;

            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);

            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            NumberFormatInfo format = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));

            XmlNode inputSizeNode = doc.SelectSingleNode("//InputSize");
            inputSize = int.Parse(inputSizeNode.FirstChild.Value, format);

            XmlNode hiddenSizeNode = doc.SelectSingleNode("//HiddenSize");
            hiddenSize = int.Parse(hiddenSizeNode.FirstChild.Value, format);

            XmlNode outputSizeNode = doc.SelectSingleNode("//OutputSize");
            outputSize = int.Parse(outputSizeNode.FirstChild.Value, format);

            XmlNode biasNode = doc.SelectSingleNode("//Bias");
            bias = bool.Parse(biasNode.FirstChild.Value);

            XmlNode centersNode = doc.SelectSingleNode("//Centers");
            centers = new double[centersNode.ChildNodes.Count][];
            for (int i = 0; i < centers.Length; i++)
            {
                XmlNode centerNode = centersNode.ChildNodes[i];
                int coords = centerNode.ChildNodes.Count;
                centers[i] = new double[coords];
                for (int j = 0; j < coords; j++)
                {
                    double center = double.Parse(centerNode.ChildNodes[j].FirstChild.Value, format);
                    centers[i][j] = center;
                }
            }

            XmlNode weightIndexNode = doc.SelectSingleNode("//WeightIndex");
            weightIndex = new int[weightIndexNode.ChildNodes.Count][];
            for (int i = 0; i < weightIndex.Length; i++)
            {
                XmlNode outputNeuronNode = weightIndexNode.ChildNodes[i];
                int num = outputNeuronNode.ChildNodes.Count;
                weightIndex[i] = new int[num];
                for (int j = 0; j < num; j++)
                {
                    int index = int.Parse(outputNeuronNode.ChildNodes[j].FirstChild.Value, format);
                    weightIndex[i][j] = index;
                }
            }

            XmlNode weightsNode = doc.SelectSingleNode("//Weights");
            weights = new double[weightsNode.ChildNodes.Count];
            for (int i = 0; i < weights.Length; i++)
            {
                double weight = double.Parse(weightsNode.ChildNodes[i].FirstChild.Value, format);
                weights[i] = weight;
            }

            XmlNode lambdasNode = doc.SelectSingleNode("//Lambdas");
            lambdas = new double[lambdasNode.ChildNodes.Count];
            for (int i = 0; i < lambdas.Length; i++)
            {
                double lambda = double.Parse(lambdasNode.ChildNodes[i].FirstChild.Value, format);
                lambdas[i] = lambda;
            }

            XmlNode alphasNode = doc.SelectSingleNode("//Alphas");
            alphas = new double[alphasNode.ChildNodes.Count];
            for (int i = 0; i < alphas.Length; i++)
            {
                double alpha = double.Parse(alphasNode.ChildNodes[i].FirstChild.Value, format);
                alphas[i] = alpha;
            }

            _inputSize = inputSize;
            _hiddenSize = hiddenSize;
            _outputSize = outputSize;
            _centers = centers;
            _weightIndex = weightIndex;
            _weights = weights;
            _lambdas = lambdas;
            _alphas = alphas;
            if (bias)
                _bias = 1;
            else
                _bias = 0;
        }

        public double[] Calculate(double[] input)
        {
            double[] inter = new double[_hiddenSize + 1];
            inter[0] = _bias;
            for (int i = 1; i <= _hiddenSize; i++)
            {
                inter[i] = Util.Gaussian(_lambdas[i - 1], input, _centers[i - 1]);
            }
            double[] output = new double[_outputSize];
            for (int i = 0; i < _outputSize; i++)
            {
                output[i] = 0;
                for (int j = 0; j <= _hiddenSize; j++)
                {
                    output[i] += _weights[_weightIndex[i][j]] * inter[j];
                }
                output[i] = Util.Sigmoid(_alphas[i], output[i]);
            }
            return output;
        }
    }
}
