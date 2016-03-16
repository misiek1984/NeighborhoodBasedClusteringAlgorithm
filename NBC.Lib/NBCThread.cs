using System;
using System.Collections.Generic;
using System.IO;

using MK.Logging;

namespace NBC.Lib
{
    public class NBCThread
    {
        private readonly int _k;
        private readonly string _outputFile;
        private readonly List<List<double>> _vectors;

        public NBCThread(int k, String outputFile, List<List<double>> vectors)
        {
            _k = k;
            _outputFile = outputFile;
            _vectors = vectors;
        }

        public void Run()
        {
            try
            {
                var alg = new NBCAlgorithm(_vectors);
                var results = alg.Start(_k);

                Log.LogMessage(String.Format("({0}) Number of clusters: {1}", _k, alg.NumberOfClusters));

                WriteOutput(results);
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private void WriteOutput(IEnumerable<int> results)
        {
            var fileName = _outputFile + "_" + _k;
            using (var writer = new StreamWriter(File.OpenWrite(fileName)))
            {
                Log.LogMessage("Writing result to output file: " + fileName);

                foreach (var res in results)
                    writer.WriteLine(res);
            }
        }
    }
}

