using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WiiApi.Tracking3d;

namespace GEarthNawiigator.NeuralNetwork
{
    public class Util
    {

        public static double Sigmoid(double alpha, double x)
        {
            return 1 / (1 + Math.Exp(-alpha * x));
        }

        public static double Gaussian(double lambda, double[] x, double[] c)
        {
            double diff = 0;
            for (int i = 0; i < x.Length; i++)
            {
                diff += Math.Pow(x[i] - c[i], 2);
            }
            return Math.Exp(-lambda * diff);
        }

        public static void Fit(ref double[] parameters, double[] x, double[] y,
                           double[] sigmaX, double[] sigmaY, int numPoints)
        {
            double s = 0.0, sx = 0.0, sy = 0.0, sxx = 0.0, sxy = 0.0, del;
            // Null sigmaY implies a constant error which drops
	        // out of the divisions of the sums.
            if (sigmaY != null)
            {
                for (int i = 0; i < numPoints; i++)
                {

                    s += 1.0 / (sigmaY[i] * sigmaY[i]);
                    sx += x[i] / (sigmaY[i] * sigmaY[i]);
                    sy += y[i] / (sigmaY[i] * sigmaY[i]);
                    sxx += (x[i] * x[i]) / (sigmaY[i] * sigmaY[i]);
                    sxy += (x[i] * y[i]) / (sigmaY[i] * sigmaY[i]);
                }
            }
            else
            {
                s = x.Length;
                for (int i = 0; i < numPoints; i++)
                {
                    sx += x[i];
                    sy += y[i];
                    sxx += x[i] * x[i];
                    sxy += x[i] * y[i];
                }
            }

            del = s * sxx - sx * sx;

            // Intercept
            parameters[0] = (sxx * sy - sx * sxy) / del;
            // Slope
            parameters[1] = (s * sxy - sx * sy) / del;
            // Errors (sd**2) on the:
            // intercept
            parameters[2] = sxx / del;
            // and slope
            parameters[3] = s / del;
        }

        public static double[] PrepareInput(List<Vector> gestureBuffer)
        {
            double[] res = new double[16];
            int n = gestureBuffer.Count / 16;
            for (int i = 0; i < 16; i++)
            {
                List<Vector> window = new List<Vector>();
                for (int j = i * n; j < (i + 1) * n && j < gestureBuffer.Count; j++)
                {
                    window.Add(gestureBuffer[j]);
                }
                double[] parameters = { 0, 0, 0, 0 };
                double[] x = new double[window.Count];
                double[] y = new double[window.Count];
                for (int k = 0; k < window.Count; k++)
                {
                    x[k] = window[k].X;
                    y[k] = window[k].Y;
                }
                Fit(ref parameters, x, y, null, null, window.Count);
                if (double.IsNaN(parameters[1]))
                {
                    if (window.First().Y <= window.Last().Y)
                        res[i] = Math.PI / 2;
                    else
                        res[i] = -Math.PI / 2;
                }
                else
                {
                    res[i] = Math.Atan(parameters[1]);
                }
                res[i] = (res[i] + (Math.PI / 2)) / Math.PI;
            }
            return res;
        }
    }
}
