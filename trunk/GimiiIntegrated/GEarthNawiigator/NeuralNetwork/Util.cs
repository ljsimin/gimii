using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEarthNawiigator.NeuralNetwork
{
    public class Util
    {

        internal static double Sigmoid(double alpha, double x)
        {
            return 1 / (1 + Math.Exp(-alpha * x));
        }

        internal static double Gaussian(double lambda, double[] x, double[] c)
        {
            double diff = 0;
            for (int i = 0; i < x.Length; i++)
            {
                diff += Math.Pow(x[i] - c[i], 2);
            }
            return Math.Exp(-lambda * diff);
        }
    }
}
