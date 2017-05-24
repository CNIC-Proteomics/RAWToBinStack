using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using QuiXoT.DA_Raw;
using MSFileReaderLib;
using Mathlet;

namespace msnPerFull
{
    class rawStats
    {
        private MSFileReaderLib.IXRawfile4 rawFile;
        private int[] freqMSn = new int[40];

        public spectrumTypes[] spectrumTypeCache = new spectrumTypes[0];
        public string workingRAWFilePath = "";

        public void initialiseSpectrumTypes(int numOfScans)
        {
            spectrumTypeCache = new spectrumTypes[numOfScans + 1];
        }

        public rawStats()
        {
            rawFile = (IXRawfile4)new MSFileReaderLib.MSFileReader_XRawfile();

            //rawFile.Open(fileName);
            //rawFile.SetCurrentController(0, 1);
            //rawFile.GetFirstSpectrumNumber(ref firstSpectrumNumber);
            //rawFile.GetNumSpectra(ref numSpectscrum);
        }

        public void openRawFast(string rawFilePath)
        {
            if (rawFilePath != workingRAWFilePath)
            {
                closeRaw();
                workingRAWFilePath = rawFilePath;
                openRaw(workingRAWFilePath);
            }
        }

        private void openRaw(string rawFilePath)
        {
            workingRAWFilePath = rawFilePath;
            rawFile.Open(rawFilePath);
            rawFile.SetCurrentController(0, 1);
        }

        public void closeRaw()
        {
            workingRAWFilePath = "";

            //long t1 = DateTime.Now.Ticks;

            rawFile.Close();

            //long t2 = DateTime.Now.Ticks;

            //StreamWriter w = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\tiempos.txt", true);
            //w.WriteLine(string.Concat(t1, "\t", t2));
            //w.Close();
        }

        public int lastSpectrumNumber()
        {
            int lastScanNumber = 0;

            rawFile.GetLastSpectrumNumber(ref lastScanNumber);

            return lastScanNumber;
        }

        public int numSpectra()
        {
            int numberOfSpectra = 0;

            rawFile.GetNumSpectra(ref numberOfSpectra);

            return numberOfSpectra;
        }

        public int retentionTimeToIndex(double _retentionTime)
        {
            int index = 0;

            rawFile.ScanNumFromRT(_retentionTime, ref index);

            return index;
        }

        public int getScanNumberFromRT(double _RT)
        {
            int scanNumber = 0;
            rawFile.ScanNumFromRT(_RT, ref scanNumber);

            return scanNumber;
        }

        public double getRTfromScanNumber(int _scanNumber)
        {
            double RT = 0;
            rawFile.RTFromScanNum(_scanNumber, ref RT);

            return RT;
        }

        public int getScanNumberOfPrevOrNextSpectrumByType(int _scan, spectrumPosition _position, spectrumTypes _typeOfSpec)
        {
            double RT = 0;
            rawFile.RTFromScanNum(_scan, ref RT);

            return getScanNumberOfPrevOrNextSpectrumByType(RT, _position, _typeOfSpec);
        }

        public int getScanNumberOfPrevOrNextSpectrumByType(double _RT,
            spectrumPosition _position,
            spectrumTypes _typeOfSpec)
        {
            int scanNumber = 0;
            rawFile.ScanNumFromRT(_RT, ref scanNumber);
            int direction = 0;
            int lastScanNumber = 0;
            rawFile.GetLastSpectrumNumber(ref lastScanNumber);

            switch (_position)
            {
                case spectrumPosition.next:
                    {
                        scanNumber++;
                        direction = 1;
                        break;
                    }
                case spectrumPosition.sameOrNext:
                    {
                        direction = 1;
                        break;
                    }
                case spectrumPosition.previous:
                    {
                        scanNumber--;
                        direction = -1;
                        break;
                    }
                case spectrumPosition.sameOrPrevious:
                    {
                        direction = -1;
                        break;
                    }
                case spectrumPosition.nearestInScanNumber:
                    {
                        int thisScanNumber = 0;
                        rawFile.ScanNumFromRT(_RT, ref thisScanNumber);
                        int previousSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrPrevious, _typeOfSpec);
                        int nextSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrNext, _typeOfSpec);

                        // usually this happens when the en of the war is reached
                        if (nextSN == 0) return previousSN;
                        if (previousSN == 0) return nextSN;

                        if (thisScanNumber - previousSN < nextSN - thisScanNumber)
                            return previousSN;
                        else
                            return nextSN;
                    }
                case spectrumPosition.nearestInRT:
                    {
                        int previousSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrPrevious, _typeOfSpec);
                        int nextSN = getScanNumberOfPrevOrNextSpectrumByType(_RT, spectrumPosition.sameOrNext, _typeOfSpec);

                        // usually this happens when the en of the war is reached
                        if (nextSN == 0) return previousSN;
                        if (previousSN == 0) return nextSN;

                        double previousRT = 0;
                        double nextRT = 0;
                        rawFile.RTFromScanNum(previousSN, ref previousRT);
                        rawFile.RTFromScanNum(nextSN, ref nextRT);

                        if (_RT - previousRT < nextRT - _RT)
                            return previousSN;
                        else
                            return nextSN;
                    }
                case spectrumPosition.same:
                    { // attention! this will not always be a fullScan!
                        return scanNumber;
                    }
            }

            while (scanNumber > 0 && scanNumber < lastScanNumber)
            {
                if (spectrumTypeFromScanNumber(scanNumber) == _typeOfSpec)
                    return scanNumber;
                else
                    scanNumber += direction;
            }

            return 0;
        }

        public double[,] getRTandMaxFromScanNumberOfPrevOrNextFull(int _scanNumber,
                                                            double _referenceMZ,
                                                            double _tolerance,
                                                            spectrumPosition _position)
        {
            int direction = 0;
            int lastSpectrum = 0;
            int newScanNumber = _scanNumber;

            switch (_position)
            {
                case spectrumPosition.previous:
                    {
                        direction = -1;
                        break;
                    }
                case spectrumPosition.next:
                    {
                        direction = 1;
                        break;
                    }
                default:
                    return getRTandMaxFromScanNumber(_scanNumber, _referenceMZ, _tolerance);
            }

            rawFile.GetLastSpectrumNumber(ref lastSpectrum);

            newScanNumber += direction;
            while (newScanNumber > 0 && newScanNumber <= lastSpectrum)
            {
                if (spectrumTypeFromScanNumber(newScanNumber) == spectrumTypes.Full)
                    return getRTandMaxFromScanNumber(newScanNumber, _referenceMZ, _tolerance);
                newScanNumber += direction;
            }

            return null;
        }

        public double[,] getRTandMaxFromScanNumber(int _scanNumber,
                                                        double _referenceMZ,
                                                        double _tolerance) // in ppms
        {
            double[,] RTintensity = new double[2, 1];
            double MZstart = _referenceMZ * (1 - _tolerance / 1e6);
            double MZend = _referenceMZ * (1 + _tolerance / 1e6);
            double maxIntensity = 0;
            double MZforMaxIntensity = 0;
            double RTatMaxIntensity = 0;

            if (_scanNumber > 0)
            {
                rawFile.RTFromScanNum(_scanNumber, ref RTatMaxIntensity);

                object provisionalSpectrum = getSpectrum(_scanNumber);
                double[,] spectrum = (double[,])provisionalSpectrum;

                int spectrumLength = spectrum.GetUpperBound(1) + 1;
                for (int i = 0; i < spectrumLength; i++)
                {
                    // we assume spectrum is ordered ascending in mz
                    if (spectrum[0, i] > MZstart)
                    {
                        if (spectrum[0, i] > MZend) break;

                        if (spectrum[1, i] > maxIntensity)
                        {
                            MZforMaxIntensity = spectrum[0, i];
                            maxIntensity = spectrum[1, i];
                        }
                    }
                }

                RTintensity[0, 0] = RTatMaxIntensity;
                RTintensity[1, 0] = maxIntensity;
            }
            else
            {
                RTintensity[0, 0] = 0;
                RTintensity[1, 0] = 0;
            }



            return RTintensity;
        }

        public double[,] getChromatogramSectionData(double _RTreference,
                                        double _RTsectionWidth,
                                        double _referenceMZ,
                                        double _tolerance)
        {
            double RTstart = _RTreference - _RTsectionWidth / 2;
            double RTend = _RTreference + _RTsectionWidth / 2;

            int[] scanNumber = getScanNumbersBetweenRTs(RTstart, RTend, spectrumTypes.Full);
            double[,] chromatogramSection = new double[2, scanNumber.Length];

            for (int i = 0; i < scanNumber.Length; i++)
            {
                double[,] provScan = getRTandMaxFromScanNumber(scanNumber[i], _referenceMZ, _tolerance);
                chromatogramSection[0, i] = provScan[0, 0];
                chromatogramSection[1, i] = provScan[1, 0];
            }

            return chromatogramSection;
        }

        public void writeFileWithChromatogramSection(string _fileName,
                                                    double[,] _chromatogramSection)
        {
            StreamWriter writer = new StreamWriter(_fileName);

            int chromatogramLength = _chromatogramSection.GetUpperBound(1) + 1;
            for (int i = 0; i < chromatogramLength; i++)
            {
                string line = _chromatogramSection[0, i] + "\t" + _chromatogramSection[1, i];
                writer.WriteLine(line);
            }

            writer.Close();
        }

        public int[] getScanNumbersBetweenRTs(double _RTstart, double _RTend, spectrumTypes _scanType)
        {
            int SNstart = 0;
            int SNend = 0;

            double maxRT = getMaxRT();

            if (_RTstart < 0) _RTstart = 0;
            if (_RTend > maxRT) _RTend = maxRT;

            rawFile.ScanNumFromRT(_RTstart, ref SNstart);
            rawFile.ScanNumFromRT(_RTend, ref SNend);

            int[] provScanNumberList = new int[SNend - SNstart + 1];

            int totScans = 0;
            for (int scan = SNstart; scan <= SNend; scan++)
            {
                spectrumTypes provScanType = spectrumTypeFromScanNumber(scan);
                if (provScanType == _scanType)
                {
                    provScanNumberList[totScans] = scan;
                    totScans++;
                }
            }

            // this is to eliminate the empty elements
            int[] scanNumberList = new int[totScans];
            for (int i = 0; i < totScans; i++)
                scanNumberList[i] = provScanNumberList[i];

            return scanNumberList;
        }

        public double getMaxRT()
        {
            double maxRT = 0;
            int lastSpectrum = 0;
            rawFile.GetLastSpectrumNumber(ref lastSpectrum);
            rawFile.RTFromScanNum(lastSpectrum, ref maxRT);
            return maxRT;
        }

        public spectrumTypes spectrumTypeFromScanNumber(int _myScanNumber)
        {
            spectrumTypes type = new spectrumTypes();

            // Definitions:
            // ScanType not defined --> -1
            // ScanTypeFull 0
            // ScanTypeSIM 1
            // ScanTypeZoom 2
            // ScanTypeSRM 3
            int pnScanType = -1;

            if (spectrumTypeCache[_myScanNumber] != spectrumTypes.Unknown)
                return spectrumTypeCache[_myScanNumber];

            rawFile.GetScanTypeForScanNum(_myScanNumber, ref pnScanType);

            switch (pnScanType)
            {
                case 0: // full scan, MS or MSn
                    int pnMSOrder = 0;
                    rawFile.GetMSOrderForScanNum(_myScanNumber, ref pnMSOrder);
                    switch (pnMSOrder)
                    {
                        case 1:
                            type = spectrumTypes.Full;
                            break;
                        case 2:
                            type = spectrumTypes.MSMS;
                            break;
                        default:
                            type = spectrumTypes.Other;
                            break;
                    }
                    break;
                case 1:
                    type = spectrumTypes.SIM;
                    break;
                case 2:
                    type = spectrumTypes.ZoomScan;
                    break;
                case 3:
                    type = spectrumTypes.SRM;
                    break;
                default: // including the default -1
                    type = spectrumTypes.ERROR;
                    break;
            }

            spectrumTypeCache[_myScanNumber] = type;

            return type;
        }

        public double[,] getAveragedSpectrum(int _peakStart,
                                            int _peakEnd,
                                            int _peakMax,
                                            bool _takeInflections, // must be an odd number
                                            int _totSpectraTaken, // 0 means all of them
                                            double _firstMz,
                                            double _lastMz,
                                            int _maxNumberOfZeros,
                                            ref quadraticEquation _par,
                                            Label _status)
        {
            ArrayList spectra = new ArrayList();

            if (_totSpectraTaken == 0) // 0 means all of them
                spectra = takeAllFullsInRange(_peakStart, _peakEnd);
            else
            {
                double RTstart = getRTfromScanNumber(_peakStart);
                double RTmax = getRTfromScanNumber(_peakMax);
                double RTend = getRTfromScanNumber(_peakEnd);

                int totSpectraWithoutInflections = _totSpectraTaken;
                if (_takeInflections) totSpectraWithoutInflections -= 2;
                int lateralTot = (totSpectraWithoutInflections - 1) / 2;

                int lastSN = 0; // to avoid duplications

                // add left inflection if needed
                if(_takeInflections)
                {
                    spectra.Add(getSpectrum(_peakStart));
                    lastSN = _peakStart;
                }

                // add scans before the maximum
                double RTLateralSize = RTmax - RTstart;
                for (int j = lateralTot; j >= 1; j--)
                {
                    double RTnew = RTmax - RTLateralSize * ((double)j / ((double)lateralTot + 1));
                    int SNnew = getScanNumberOfPrevOrNextSpectrumByType(RTnew,
                        spectrumPosition.sameOrPrevious, spectrumTypes.Full);

                    if (lastSN != SNnew)
                    {
                        spectra.Add(getSpectrum(SNnew));
                        lastSN = SNnew;
                    }
                }

                // add maximum
                if (lastSN != _peakMax)
                {
                    spectra.Add(getSpectrum(_peakMax));
                    lastSN = _peakMax;
                }

                // add scans after the maximum
                RTLateralSize = RTend - RTmax;
                for (int j = 1; j <= lateralTot; j++)
                {
                    double RTnew = RTmax + RTLateralSize * ((double)j / ((double)lateralTot + 1));
                    int SNnew = getScanNumberOfPrevOrNextSpectrumByType(RTnew,
                        spectrumPosition.sameOrNext, spectrumTypes.Full);

                    if (lastSN != SNnew)
                    {
                        spectra.Add(getSpectrum(SNnew));
                        lastSN = SNnew;
                    }
                }

                // add right inflection if needed
                if (_takeInflections && lastSN != _peakEnd)
                    spectra.Add(getSpectrum(_peakEnd));
 
                //if (_totSpectraTaken == 3 && _takeInflections)
                //{
                //    spectra.Add(getSpectrum(_peakStart));
                //    spectra.Add(getSpectrum(_peakMax));
                //    spectra.Add(getSpectrum(_peakEnd));
                //}
            }

            double[,] regularBins = new double[0, 0];

            if (spectra == null) return null;

            double[,] firstSpectrum = (double[,])spectra[0];

            //double[,] secondSpectrum = (double[,])spectra[1];
            //double[,] thirdSpectrum = (double[,])spectra[2];

            if (!_par.hasBeenCalculated)
            {
                _status.Text = "Calculating bin size parameters...";
                Application.DoEvents();
                string rawFile = _par.rawFilePath;
                _par = getBinParameters(firstSpectrum);
                _par.rawFilePath = rawFile;
            }

            if (regularBins.Length == 0)
                regularBins = getRegularBins(_firstMz, _lastMz, _par);

            int allowedMargin = 4;

            ArrayList spectraNewBins = getSpectraInNewBins(regularBins, spectra, allowedMargin);
            double[,] spectrumAverage = calculateAveragedSpectrum(spectraNewBins, allowedMargin);

            //saveIndividualSpectrum(firstSpectrum, "x1");
            //saveIndividualSpectrum(secondSpectrum, "x2");
            //saveIndividualSpectrum(thirdSpectrum, "x3");
            //saveIndividualSpectrum(spectrumAverage, "xaverage");
            
            return spectrumAverage;
        }

        private static double[,] getRegularBins(double _firstMz, double _lastMz, quadraticEquation _par)
        {
            ArrayList regularBinsArrayList = new ArrayList();

            double x = _firstMz;
            while (x <= _lastMz)
            {
                regularBinsArrayList.Add(x);
                x += _par.A + _par.B * (x - _par.x0) + _par.C * Math.Pow(x - _par.x0, 2);
            }

            double[,] regularBins = new double[2, regularBinsArrayList.Count];
            for (int i = 0; i < regularBinsArrayList.Count; i++)
                regularBins[0, i] = (double)regularBinsArrayList[i];
            return regularBins;
        }

        private static ArrayList getSpectraInNewBins(double[,] _regularBins, ArrayList _spectra, int _allowedBack)
        {
            ArrayList spectraNewBins = new ArrayList();
            for (int i = 0; i < _spectra.Count; i++)
            {
                double[,] spectrumlet = (double[,])rearrangeSpectrum((double[,])_spectra[i], _regularBins, _allowedBack);
                spectraNewBins.Add(spectrumlet);
            }
            return spectraNewBins;
        }

        private static double[,] calculateAveragedSpectrum(ArrayList _spectraNewBins, int _maxLateralZeros)
        {
            int accumulatedZeros = _maxLateralZeros;
            int spectrumAveragePosition = 0;

            double[,] spectrumletFirst = (double[,])_spectraNewBins[0];
            int spectrumletFirstSize = spectrumletFirst.GetUpperBound(1) + 1;
            double[,] spectrumAverageProv = new double[2, spectrumletFirstSize];
            double[,] spectrumAverageInterm = new double[2, spectrumletFirstSize];

            // copy mzs (they should be all the same, so we take them just from the first one)
            for (int i = 0; i < spectrumletFirstSize; i++)
                spectrumAverageProv[0, i] = spectrumletFirst[0, i];

            // get spectrum intensities with averages
            int totSpectra = _spectraNewBins.Count;
            for (int k = 0; k < totSpectra; k++)
            {
                double[,] spectrum = (double[,])_spectraNewBins[k];
                for (int i = 0; i < spectrum.GetUpperBound(1) + 1; i++)
                    spectrumAverageProv[1, i] += spectrum[1, i] / (double)totSpectra;
            }

            // get spectrum removing trivial zeros
            for (int i = 0; i < spectrumletFirstSize + _maxLateralZeros; i++)
            {
                if (i < spectrumletFirstSize)
                {
                    if (spectrumAverageProv[1, i] == 0) accumulatedZeros++;
                    else accumulatedZeros = 0;
                }

                if (accumulatedZeros < 2 * _maxLateralZeros + 1 && i >= _maxLateralZeros)
                {
                    spectrumAverageInterm[0, spectrumAveragePosition] = spectrumAverageProv[0, i - _maxLateralZeros];
                    spectrumAverageInterm[1, spectrumAveragePosition] = spectrumAverageProv[1, i - _maxLateralZeros];
                    spectrumAveragePosition++;
                }
            }

            // remove tail of remaining zeros by copying to a shorter array
            double[,] spectrumAverage = new double[2, spectrumAveragePosition];
            for (int i = 0; i < spectrumAveragePosition; i++)
                for (int j = 0; j <= 1; j++)
                    spectrumAverage[j, i] = spectrumAverageInterm[j, i];

            return spectrumAverage;
        }

        private static quadraticEquation getBinParameters(double[,] firstSpectrum)
        {
            quadraticEquation par = new quadraticEquation();
            par.x0 = firstSpectrum[0, 0];

            int totElements = ((double[,])firstSpectrum).GetUpperBound(1) + 1;
            double[] mzs = new double[totElements];
            for (int i = 0; i < totElements; i++)
                mzs[i] = firstSpectrum[0, i];

            double[] distances = new double[totElements];
            for (int i = 0; i < totElements - 1; i++)
                distances[i] = mzs[i + 1] - mzs[i];

            // get local median

            int localSize = 200; // this must be an even number
            double[,] medianDistances = new double[2, totElements - localSize];
            for (int i = localSize / 2 + 1; i < totElements - (localSize / 2 - 1); i++)
            {
                double[] localDistances = new double[localSize];
                for (int j = 0; j < localSize; j++)
                    localDistances[j] = distances[j + (i - localSize / 2 - 1)];

                // bubble sort
                for (int k = localSize; k >= 0; k--)
                    for (int l = localSize - 1; l > 0; l--)
                    {
                        if (localDistances[l] < localDistances[l - 1])
                        {
                            double prov = localDistances[l - 1];
                            localDistances[l - 1] = localDistances[l];
                            localDistances[l] = prov;
                        }
                    }

                // get median

                medianDistances[0, i - (localSize / 2 + 1)] = firstSpectrum[0, i];
                medianDistances[1, i - (localSize / 2 + 1)] = (localDistances[localSize / 2 - 2]
                    + localDistances[localSize / 2 - 1]) / 2;
            }

            Smoother smoothy = new Smoother();
            smoothy.original = medianDistances;
            double[] coeffs = smoothy.quadraticCoefficientsABC;

            par.A = coeffs[0];
            par.B = coeffs[1];
            par.C = coeffs[2];
            par.initialValue = smoothy.initialValue;
            par.hasBeenCalculated = true;
            return par;
        }

        private static double[,] rearrangeSpectrum(double[,] _originalSpectra,
                                                double[,] _regularBins,
                                                int _allowedCheckBack)
        {
            double[,] filledBins = (double[,])_regularBins.Clone();

            ArrayList spectrumletFirst = new ArrayList();
            int starting = getStartingIndex(_originalSpectra, filledBins[0, 0]);
            int finishing = getStartingIndex(_originalSpectra, filledBins[0, filledBins.GetUpperBound(1)]) + 1;

            int filledPrevious = -1;

            for (int k = starting; k <= finishing; k++) //*** ahora k es el índice de _originalSpectra, no de filledBins
            {
                double[,] previousOrSameDot = new double[2, 1];
                double[,] nextDot = new double[2, 1];
                double[,] extrapolated = new double[2, 1];

                // this was slow, it is better to use the searching algorithm (next)
                //int i2 = 0;
                //while (true)
                //{
                //    if (i2 <= _originalSpectra.GetUpperBound(1))
                //        if (_originalSpectra[0, i2] <= filledBins[0, k]) i2++;
                //        else break;
                //}

                int referenceInFilled = getStartingIndex(filledBins, _originalSpectra[0, k]) + 1;

                int backed = 0;

                while (backed < _allowedCheckBack)
                {
                    if (filledPrevious + 1 < referenceInFilled - backed) backed++;
                    else break;
                }

                for (int m = 0; m <= backed; m++)
                {
                    int previousOrSameIndex = getStartingIndex(_originalSpectra, filledBins[0, referenceInFilled - m]);
                    int nextIndex = previousOrSameIndex + 1;

                    for (int j = 0; j <= 1; j++)
                    {
                        if (previousOrSameIndex > 0) previousOrSameDot[j, 0] = _originalSpectra[j, previousOrSameIndex];
                        else previousOrSameDot[j, 0] = 0;

                        if (nextIndex <= _originalSpectra.GetUpperBound(1)) nextDot[j, 0] = _originalSpectra[j, nextIndex];
                        else nextDot[j, 0] = 0;
                    }

                    if (previousOrSameDot[1, 0] != 0 || nextDot[1, 0] != 0)
                        filledBins[1, referenceInFilled - m] = linearExtrapolation(previousOrSameDot, nextDot, (double)filledBins[0, referenceInFilled - m]);
                }

                filledPrevious = referenceInFilled;
            }

            return filledBins;
        }

        private static int getStartingIndex(double[,] _array, double _value)
        {
            if (_value > _array[0, _array.GetUpperBound(1)]) return _array.GetUpperBound(1) - 1;

            double start = (Math.Truncate(((double)_array.GetUpperBound(1) + 1) / 2));
            double step = start;
            while (!(double.Parse(_array[0, (int)start].ToString()) <= _value
                && double.Parse(_array[0, (int)start + 1].ToString()) > _value))
            {
                if (step == 0) return -1;
                step = step / 2;
                if (double.Parse(_array[0, (int)start].ToString()) > _value) start -= step;
                else start += step;
            }

            return (int)Math.Truncate(start);
        }

        private static double linearExtrapolation(double[,] x1, double[,] x2, double x)
        {
            double slope = (x2[1, 0] - x1[1, 0]) / (x2[0, 0] - x1[0, 0]);
            double ordinate = x1[1, 0] - slope * x1[0, 0];

            return slope * x + ordinate;
        }

        public static void saveIndividualSpectrum(double[,] _spectrum, string fileNameExtension)
        {
            StreamWriter wspec = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\rawtobinstack averagingInflections\spectrum_" + fileNameExtension + ".txt");
            for (int i = 0; i <= _spectrum.GetUpperBound(1); i++)
                wspec.WriteLine(_spectrum[0, i].ToString() + "\t" + _spectrum[1, i].ToString());
            wspec.Close();
        }

        public object getAveragedSpectrumOldTry(int _peakStart,
                                            int _peakEnd)
        {
            object spectrum = new object();
            ArrayList spectra = takeAllFullsInRange(_peakStart, _peakEnd);

            double[,] firstSpectrum = (double[,])spectra[0];

            int totElements = ((double[,])firstSpectrum).GetUpperBound(1) + 1;
            double[] mzs = new double[totElements];
            for (int i = 0; i < totElements; i++)
                mzs[i] = firstSpectrum[0, i];

            double[] distances = new double[totElements];
            for (int i = 0; i < totElements - 1; i++)
                distances[i] = mzs[i + 1] - mzs[i];

            // get local median

            int localSize = 200; // this must be an even number
            double[,] medianDistances = new double[2, totElements - localSize];
            for (int i = localSize / 2 + 1; i < totElements - (localSize / 2 - 1); i++)
            {
                double[] localDistances = new double[localSize];
                for (int j = 0; j < localSize; j++)
                    localDistances[j] = distances[j + (i - localSize / 2 - 1)];

                // bubble sort
                for (int k = localSize; k >= 0; k--)
                    for (int l = localSize - 1; l > 0; l--)
                    {
                        if (localDistances[l] < localDistances[l - 1])
                        {
                            double prov = localDistances[l - 1];
                            localDistances[l - 1] = localDistances[l];
                            localDistances[l] = prov;
                        }
                    }

                // get median

                medianDistances[0, i - (localSize / 2 + 1)] = firstSpectrum[0, i];
                medianDistances[1, i - (localSize / 2 + 1)] = (localDistances[localSize / 2 - 2]
                    + localDistances[localSize / 2 - 1]) / 2;
            }

            // get bin size equation

            double[,] newBins = new double[2, medianDistances.GetUpperBound(1) + 1];

            ArrayList resultingSpec = new ArrayList();
            Smoother smoothy = new Smoother();
            smoothy.original = medianDistances;
            double[] coeffs = smoothy.quadraticCoefficientsABC;
            double A = coeffs[0];
            double B = coeffs[1];
            double C = coeffs[2];
            //double[,] q = smoothy.quadratic;
            newBins[0, 0] = medianDistances[0, 0];
            for (int i = 1; i < newBins.GetUpperBound(1); i++)
            {
                double x = medianDistances[0, i];
                newBins[0, i] = x;
                newBins[1, i] = A + B * (x - smoothy.initialValue) + C * Math.Pow(x - smoothy.initialValue, 2);
                newBins[0, i] = newBins[0, i - 1] + newBins[1, i];
                //newBins[1, i] = A * (x - smoothy.initialValue) + B * Math.Pow(x - smoothy.initialValue, 2) / 2 + C * Math.Pow(x - smoothy.initialValue, 3) / 3;
            }

            // meter el resultado en spectrum

            //StreamWriter wspec = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\rawtobinstack averagingInflections\firstSpectrum.txt");
            //for (int i = 0; i <= firstSpectrum.GetUpperBound(1); i++)
            //    wspec.WriteLine(firstSpectrum[0, i].ToString() + "\t" + firstSpectrum[1, i].ToString());
            //wspec.Close();

            //StreamWriter wmedianDistances = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\rawtobinstack averagingInflections\medianDistances.txt");
            //for (int i = 0; i <= medianDistances.GetUpperBound(1); i++)
            //    wmedianDistances.WriteLine(medianDistances[0, i].ToString() + "\t" + medianDistances[1, i].ToString());
            //wmedianDistances.Close();

            //StreamWriter wnewBins = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\rawtobinstack averagingInflections\newBins.txt");
            //for (int i = 0; i <= newBins.GetUpperBound(1); i++)
            //    wnewBins.WriteLine(newBins[0, i].ToString() + "\t" + newBins[1, i].ToString());
            //wnewBins.Close();

            return spectrum;
        }

        private ArrayList takeAllFullsInRange(int _peakStart, int _peakEnd)
        {
            ArrayList spectra = new ArrayList();

            int specNumber = _peakStart;
            specNumber = getScanNumberOfPrevOrNextSpectrumByType(specNumber,
                                                                spectrumPosition.sameOrNext,
                                                                spectrumTypes.Full);

            while (specNumber <= _peakEnd)
            {
                specNumber = getScanNumberOfPrevOrNextSpectrumByType(specNumber,
                                                                spectrumPosition.next,
                                                                spectrumTypes.Full);

                if (specNumber == 0)
                {
                    MessageBox.Show("Something went wrong: the raw file appear to be corrupt or inaccessible."+
                        "\nNote this usually means the path is wrong." +
                        "\nRaw file path: " + workingRAWFilePath);
                    Application.DoEvents();
                    return null;
                }

                if (specNumber <= _peakEnd)
                {
                    object provSpectrum = getSpectrum(specNumber);
                    spectra.Add(provSpectrum);
                }
            }
            return spectra;
        }

        public object getAveragedSpectrumThermoFunction(int _peakStart,
                                    int _peakEnd,
                                    double _firstMz,
                                    double _lastMz,
                                    string _filter,
                                    double _centroidPeakWidth)
        {
            // FTMS + p NSI Full ms [###.##-####.##]
            string newFilter = String.Format(String.Concat(_filter, " [{0}-{1}]"), _firstMz, _lastMz);
            double[,] provMassList;

            if (_centroidPeakWidth <= 0)
                provMassList = (double[,])getAveragedSpectrumThermoFunction(_peakStart,
                                    _peakEnd, newFilter);
            else
                provMassList = (double[,])getAveragedSpectrumThermoFunction(_peakStart,
                                    _peakEnd, newFilter, _centroidPeakWidth);

            int lowerIndex = getStartingIndex(provMassList, _firstMz);
            int upperIndex = getStartingIndex(provMassList, _lastMz) + 1;
            upperIndex = Math.Min(upperIndex, provMassList.GetUpperBound(1));
            int newSize = upperIndex - lowerIndex + 1;

            double[,] myMassList = new double[2, newSize];

            for (int j = 0; j <= 1; j++)
            {
                int k = 0;
                for (int i = lowerIndex; i <= upperIndex; i++)
                {
                    myMassList[j, k] = provMassList[j, i];
                    k++;
                }
            }

            return myMassList;
        }

        public object getAveragedSpectrumThermoFunction(int _peakStart,
                                    int _peakEnd,
                                    string _filter)
        {
            object myPeakFlags = null;
            int myArraySize = 0;
            object myMassList = null;
            int bg1First = 0;
            int bg1Last = 0;
            int bg2First = 0;
            int bg2Last = 0;
            double centroidPeakWidth = 0;

            rawFile.GetAverageMassList(ref _peakStart,
                                        ref _peakEnd,
                                        ref bg1First,
                                        ref bg1Last,
                                        ref bg2First,
                                        ref bg2Last,
                                        _filter,
                                        0, 0, 0, 0,
                                        ref centroidPeakWidth,
                                        ref myMassList,
                                        ref myPeakFlags,
                                        ref myArraySize);

            //rawFile.GetAveragedMassSpectrum(ref _scanNumbers,
            //                                _scanNumbers.Length,
            //                                false,
            //                                ref myMassList,
            //                                ref myPeakFlags,
            //                                ref myArraySize);


            //string fileName = @"C:\Documents and Settings\mtrevisan\Desktop\borrar\espectro_todos_"
            //    + _peakStart.ToString() + "-" + _peakEnd.ToString() + ".txt";
            //double[,] list = (double[,])myMassList;
            //StreamWriter w = new StreamWriter(fileName, true);

            //for (int i = 0; i <= list.GetUpperBound(1); i++)
            //    w.WriteLine(string.Concat(list[0, i].ToString(), "\t", list[1, i].ToString()));

            //w.Close();

            return myMassList;
        }

        public object getAveragedSpectrumThermoFunction(int _peakStart,
                                    int _peakEnd,
                                    string _filter,
                                    double _centroidPeakWidth)
        {
            object myPeakFlags = null;
            int myArraySize = 0;
            object myMassList = null;
            int bg1First = 0;
            int bg1Last = 0;
            int bg2First = 0;
            int bg2Last = 0;

            rawFile.GetAverageMassList(ref _peakStart,
                                        ref _peakEnd,
                                        ref bg1First,
                                        ref bg1Last,
                                        ref bg2First,
                                        ref bg2Last,
                                        _filter,
                                        0, 0, 0, 1,
                                        ref _centroidPeakWidth,
                                        ref myMassList,
                                        ref myPeakFlags,
                                        ref myArraySize);

            //rawFile.GetAveragedMassSpectrum(ref _scanNumbers,
            //                                _scanNumbers.Length,
            //                                false,
            //                                ref myMassList,
            //                                ref myPeakFlags,
            //                                ref myArraySize);


            //string fileName = @"C:\Documents and Settings\mtrevisan\Desktop\borrar\espectro_todos_"
            //    + _peakStart.ToString() + "-" + _peakEnd.ToString() + ".txt";
            //double[,] list = (double[,])myMassList;
            //StreamWriter w = new StreamWriter(fileName, true);

            //for (int i = 0; i <= list.GetUpperBound(1); i++)
            //    w.WriteLine(string.Concat(list[0, i].ToString(), "\t", list[1, i].ToString()));

            //w.Close();

            return myMassList;
        }

        public object getSpectrum(int _scanNumber)
        {
            object myPeakFlags = null;
            int myArraySize = 0;
            object myMassList = null;
            double myCentroidPeakWidth = 0;
            #region GetMassListRangeFromScanNum usage
            //        string bstrFiler = "";
            //        int nIntensityCutoffType = 0;
            //        int intensityCutoffValue = 0;
            //        int nMaxNumberOfPeaks = 0;
            //        int bCentroidResult = 0;
            //        double pdCentroidPeakWidth = 0;
            //        object pvarMassList = 0;
            //        object pvarPeakFlags = 0;
            //        string szMassRange1 = "";
            //        int pnArraySize = 0;
            //        rawFile.GetMassListRangeFromScanNum(ref _scanNumber, bstrFiler, nIntensityCutoffType, intensityCutoffValue,
            //nMaxNumberOfPeaks, bCentroidResult, ref pdCentroidPeakWidth, ref pvarMassList, ref pvarPeakFlags,
            //szMassRange1, ref pnArraySize);

            #endregion
            //long t1 = DateTime.Now.Ticks;

            rawFile.GetMassListFromScanNum(ref _scanNumber,
                                            "", 0, 0, 0, 0,
                                            ref myCentroidPeakWidth,
                                            ref myMassList,
                                            ref myPeakFlags,
                                            ref myArraySize);

            //long t2 = DateTime.Now.Ticks;
            ////double milliseconds = (t2 - t1) / 10000.0;

            //StreamWriter w = new StreamWriter(@"D:\DATUMARO\trabajo\tareas en curso\tareas1 ug\pruebas QuiXtoQuiX\tiempos.txt", true);
            //w.WriteLine(string.Concat(t1, "\t", t2));
            //w.Close();

            //string fileName = @"C:\Documents and Settings\mtrevisan\Desktop\borrar\espectro_"
            //    + _scanNumber.ToString() + ".txt";
            //double[,] list = (double[,])myMassList;
            //StreamWriter w = new StreamWriter(fileName, true);

            //for (int i = 0; i <= list.GetUpperBound(1); i++)
            //    w.WriteLine(string.Concat(list[0, i].ToString(), "\t", list[1, i].ToString()));

            //w.Close();


            return myMassList;
        }

        public int[] fullScansAround(int _scanMSMS, int _numOfScans)
        {
            int[] scansAround = new int[_numOfScans];

            int i = 1;
            int n = _numOfScans / 2 - 1;
            while (n >= 0)
            {
                if (_scanMSMS - i >= 0)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS - i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS - i;
                        n--;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            i = 1;
            n = _numOfScans / 2;
            while (n < _numOfScans)
            {
                if (_scanMSMS + i < spectrumTypeCache.Length)
                {
                    if (spectrumTypeFromScanNumber(_scanMSMS + i) == spectrumTypes.Full)
                    {
                        scansAround[n] = _scanMSMS + i;
                        n++;
                        i++;
                    }
                    else
                        i++;
                }
                else
                    break;
            }

            return scansAround;
        }

        public int[] fullScansBetween(int _scanStart, int _scanEnd)
        {
            int numOfScans = _scanEnd - _scanStart + 1;
            int[] scansBetween = new int[numOfScans];

            int n = 0;

            for (int i = _scanStart; i <= _scanEnd; i++)
            {
                if (spectrumTypeFromScanNumber(i) == spectrumTypes.Full)
                {
                    scansBetween[n] = i;
                    n++;
                }
            }

            return scansBetween;
        }

        public enum spectrumPosition
        {
            same,
            sameOrNext,
            next,
            sameOrPrevious,
            previous,
            nearestInScanNumber,
            nearestInRT
        }
    }

    class quadraticEquation
    {
        public string rawFilePath = "";
        public bool hasBeenCalculated = false;
        public double A;
        public double B;
        public double C;
        public double initialValue;
        public double x0;
    }
}