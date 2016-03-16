using System.Collections.Generic;

namespace NBC.Lib
{
    public class VectorData
    {
        public List<int> KNeighborhood { get; set; }

        public int ReverseKNeighborhood { get; set; }

        public double NeighborhoodDensityFactor { get; set; }

        public VectorData()
        {
            KNeighborhood = new List<int>();
            ReverseKNeighborhood = 0;
            NeighborhoodDensityFactor = 0;
        }
    }
}