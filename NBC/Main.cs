using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MK.Logging;

namespace NBC.Lib
{
    public class Runner
    {
        private int _fromK;
        private int _toK;
        private int _step;
        private string _inputFile;
        private string _outputFile;
        private bool _runExperimentsInThreads;
        private int _numberOfThreads;

        public static void Main(string[] argv)
        {
            Log.UseDefaultConfig();
            Log.EnableTimning = true;

            new Runner().StartProgram(argv);

            Console.ReadLine();
        }

        public void StartProgram(String[] argv)
        {
            if (!ProcessArguments(argv))
            {
                PrintHelp();
                return;
            }

            try
            {
                var reader = new DataReader();

                Log.LogMessage("Reading data from input file...");

                var vectors = reader.ReadData(_inputFile);

                Log.LogMessage("Number of vectors: " + vectors.Count);

                var tasks = new List<Task>();

                for (var i = _fromK; i < _toK; i = i + _step)
                {
                    Log.LogMessage("Calculations with k = " + i);

                    var th = new NBCThread(i, _outputFile, vectors);

                    if (_runExperimentsInThreads)
                        tasks.Add(Task.Factory.StartNew(th.Run));
                    else
                        th.Run();
                }

                if (_runExperimentsInThreads)
                    Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine("nbc k inputFile outputFile");
            Console.WriteLine("nbc k inputFile outputFile numberOfThreads");
            Console.WriteLine("nbc fromK toK step inputFile outputFile runExperimentsInThreads");
        }

        private bool ProcessArguments(String[] argv)
        {
            if (argv.Length == 3)
            {
                _fromK = Int32.Parse(argv[0]);
                _toK = _fromK + 1;
                _step = 1;
                _inputFile = argv[1];
                _outputFile = argv[2];
                _runExperimentsInThreads = false;
                _numberOfThreads = 0;
            }
            else if (argv.Length == 4)
            {
                _fromK = Int32.Parse(argv[0]);
                _toK = _fromK + 1;
                _step = 1;
                _inputFile = argv[1];
                _outputFile = argv[2];
                _runExperimentsInThreads = false;
                _numberOfThreads = Int32.Parse(argv[3]);
            }
            else if (argv.Length == 6)
            {
                _fromK = Int32.Parse(argv[0]);
                _toK = Int32.Parse(argv[1]);
                _step = Int32.Parse(argv[2]);
                _inputFile = argv[3];
                _outputFile = argv[4];
                _runExperimentsInThreads = argv[5].Equals("Y", StringComparison.OrdinalIgnoreCase);
                _numberOfThreads = 0;
            }
            else
                return false;

            if (_fromK < 0 || _toK < 0)
                return false;

            if (_fromK >= _toK)
                return false;

            if (_step <= 0)
                return false;

            return _numberOfThreads >= 0;
        }
    }
}