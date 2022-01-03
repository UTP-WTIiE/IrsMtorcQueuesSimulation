using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MetabolismSimulations.Extensions
{
    public static class RandomExtension
    {
        public static bool SHOULD_CONSTAIT_GAUSSIAN = false;
        public static double CONSTRAIT_GAUSSIAN_MIN = -0.1;
        public static double CONSTRAIT_GAUSSIAN_MAX = 0.1;

        private static double nextGaussian(this Random rand)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return randStdNormal;
        }

        public static double NextGaussian(this Random rand, double per)
        {
            if (SHOULD_CONSTAIT_GAUSSIAN)
                return NextGaussianWithConstrains(rand, CONSTRAIT_GAUSSIAN_MIN, CONSTRAIT_GAUSSIAN_MAX);
            else
                return nextGaussian(rand) * per;
        }

        public static IEnumerable<double> NextGaussianArray(this Random rand, int count, double per)
        {
            for (int i = 0; i < count; i++)
                yield return rand.NextGaussian(per);
        }

        public static double NextGaussianWithConstrains(this Random rand, double min, double max)
        {
            double g = 0;
            do
            {
                g = rand.nextGaussian();
            }
            while (g < min || g > max);
            return g;
        }
    }
}
