using MetabolismSimulations.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mTORC.Models
{
    public class RandomGenerator
    {
        Random b = new Random();

        public double NextEqualDistribution()
        {
            return b.NextDouble();
        }

        public double NextGaussian()
        {
            return b.NextGaussian(1);
        }

        public double[] NextGaussianTable(int length)
        {
            return next_gaussian_table(length).ToArray();   
        }
        private IEnumerable<double> next_gaussian_table(int length)
        {
            for (int i = 0; i < length; i++)
                yield return NextGaussian();
        }
    }
}
