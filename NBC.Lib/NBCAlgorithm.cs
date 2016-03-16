using System;
using System.Collections.Generic;

using MK.Logging;

namespace NBC.Lib
{
    public class NBCAlgorithm
    {
        private readonly List<List<double>> _vectors;

        private readonly List<VectorData> _vectorsData = new List<VectorData>();

        private readonly List<int> _results = new List<int>();

        private int _k;

        private readonly List<Tuple<int, double>> _distances = new List<Tuple<int, double>>();

        public int NumberOfClusters { get; private set; }

        public NBCAlgorithm(List<List<double>> v)
        {
            _vectors = v;
        }

        public List<int> Start(int k)
        {
            Log.StartTiming();

            NumberOfClusters = 0;
            _k = k > 0 ? k : 5;

            CalcualteNeighborhoodDensityFactor();
            Clustering();

            Log.StopTiming();

            return _results;
        }

        private void CalcualteNeighborhoodDensityFactor()
        {
            try
            {
                Log.StartTiming();

                _vectors.ForEach(a => _vectorsData.Add(new VectorData()));

                for (var i = 0; i < _vectors.Count; ++i)
                    CalculateKNeighborhood(i);

                foreach (var vectorData in _vectorsData)
                    vectorData.NeighborhoodDensityFactor = vectorData.ReverseKNeighborhood / (double)vectorData.KNeighborhood.Count;

                Log.StopTiming();
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private void CalculateKNeighborhood(int indexOfVector)
        {
            _distances.Clear();

            CalculateDistances(indexOfVector);

            _distances.Sort((tuple, tuple1) =>
            {
                if (tuple.Item2 > tuple1.Item2)
                    return 1;

                if (tuple.Item2 < tuple1.Item2)
                    return -1;

                return 0;
            });

            ChooseKNearest(indexOfVector);
        }

        private void CalculateDistances(int indexOfVector)
        {
            for (var j = 0; j < _vectors.Count; ++j)
            {
                if (indexOfVector != j)
                {
                    var dist = CalculateDistance(indexOfVector, j);
                    _distances.Add(new Tuple<int, double>(j, dist));
                }
            }
        }

        private double CalculateDistance(int index1, int index2)
        {
            var v1 = _vectors[index1];
            var v2 = _vectors[index2];

            if (v1.Count != v2.Count)
                throw new Exception("All vectors should have the same length");

            double res = 0;
            for (var i = 0; i < v1.Count; ++i)
                res += (v1[i] - v2[i]) * (v1[i] - v2[i]);

            return Math.Pow(res, 0.5);
        }

        private void ChooseKNearest(int indexOfVector)
        {
            var d = _vectorsData[indexOfVector];

            double dist = -1;
            if (_distances.Count >= _k)
                dist = _distances[_k - 1].Item2;

            for (var i = 0; i < _k && i < _distances.Count; ++i)
            {
                var currentPair = _distances[i];

                if (i < _k || dist == currentPair.Item2)
                {
                    d.KNeighborhood.Add(currentPair.Item1);

                    // If vector A belong to the kNB set of the vector B it means that
                    // vector B
                    // belong to the R-kNB set of the vector A
                    _vectorsData[currentPair.Item1].ReverseKNeighborhood++;
                }
                else
                    break;
            }
        }

        private void Clustering()
        {
            Log.StartTiming();
            _results.Clear();

            // Allocate memory for results
            _vectors.ForEach(v => _results.Add(-1));

            for (var i = 0; i < _vectorsData.Count; ++i)
            {
                var d = _vectorsData[i];
                if (d.NeighborhoodDensityFactor < 1 || _results[i] != -1)
                    continue;

                _results[i] = NumberOfClusters;

                var toCheck = new LinkedList<int>();
                foreach (var temp in d.KNeighborhood)
                {
                    _results[temp] = NumberOfClusters;

                    if (_vectorsData[temp].NeighborhoodDensityFactor >= 1)
                        toCheck.AddFirst(temp);
                }

                // Expanding cluster
                while (toCheck.Count > 0)
                {
                    d = _vectorsData[toCheck.Last.Value];
                    toCheck.RemoveLast();

                    foreach (var temp in d.KNeighborhood)
                    {
                        if (_results[temp] != -1)
                            continue;

                        _results[temp] = NumberOfClusters;

                        if (_vectorsData[temp].NeighborhoodDensityFactor >= 1)
                            toCheck.AddFirst(temp);
                    }
                }

                NumberOfClusters = NumberOfClusters + 1;
            }

            Log.StopTiming();
        }

    }
}
