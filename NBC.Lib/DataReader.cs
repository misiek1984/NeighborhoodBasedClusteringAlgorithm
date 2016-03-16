using System;
using System.Collections.Generic;
using System.IO;

namespace NBC.Lib
{
    public class DataReader
    {
        private bool _minus;
        private bool _decimalSeperator;
        private double _i;
        private double _value;
        private int _firstVectorLength;
        private int _lines;
        private int _columns;

        private List<List<double>> _vectors;
        private List<double> _currentVector;

        public DataReader()
        {
            _vectors = null;
            _currentVector = null;
        }

        public List<List<double>> ReadData(string inputFile)
        {
            Init();

            using (var reader = new StreamReader(File.OpenRead(inputFile)))
            {
                _currentVector = new List<double>();

                int i;
                while ((i = reader.Read()) != -1)
                {
                    var ch = (char) i;

                    _columns++;

                    if (ch >= Constants.ZeroChar && ch <= Constants.NineChar)
                        HandleNumber(ch);
                    else switch (ch)
                    {
                        case Constants.DecimalSeperator:
                            _decimalSeperator = true;
                            break;
                        case Constants.Seperator:
                            HandleSeperator();
                            break;
                        case Constants.Return:
                            break;
                        case Constants.EndOfLine:
                            HandleNewLine();
                            break;
                        case Constants.MinusChar:
                            _minus = true;
                            break;
                        default:
                            throw new Exception("Unexpected char in line " + _lines + " column " + _columns);
                    }
                }
            }

            return _vectors;
        }

        private void Init()
        {
            Reset();

            _firstVectorLength = -1;
            _lines = 0;
            _columns = 0;

            _vectors = new List<List<double>>();
        }

        private void Reset()
        {
            _minus = false;
            _decimalSeperator = false;
            _i = 0.1;
            _value = 0;
        }

        private void HandleSeperator()
        {
            _value = _minus ? -_value : _value;
            _currentVector.Add(_value);

            Reset();
        }

        private void HandleNumber(char ch)
        {
            if (_decimalSeperator == false)
            {
                _value *= 10;
                _value += ch - Constants.ZeroChar;
            }
            else
            {
                var d = (ch - Constants.ZeroChar) * _i;
                _i *= 0.1;
                _value += d;
            }
        }

        private void HandleNewLine()
        {
            _value = _minus ? -_value : _value;
            _currentVector.Add(_value);

            if (_firstVectorLength == -1)
                _firstVectorLength = _currentVector.Count;
            else if (_firstVectorLength != _currentVector.Count)
                throw new Exception("All vectors must have the same size!");

            _vectors.Add(_currentVector);
            _currentVector = new List<double>();

            Reset();

            _lines++;
            _columns = 0;
        }

    }
}