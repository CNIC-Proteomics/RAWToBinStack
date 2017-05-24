using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QuiXoT.DA_Raw;
using MSFileReaderLib;

namespace RawToBinStack
{
    // copies from rawStats in QuiXtoQuiX v2.13
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
}
