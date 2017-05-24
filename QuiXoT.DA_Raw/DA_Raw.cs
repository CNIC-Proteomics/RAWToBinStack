using System;
using System.Collections.Generic;
using System.Text;
using QuiXoT.math;
using System.Windows.Forms;
using msnPerFull;
using System.IO;

namespace QuiXoT.DA_Raw
{
    public enum averagingMethod
    {
        none,
        everything_ThermoMethod_noWidth,
        everything_ThermoMethod_Width0001,
        everything_InlabMethod,
        mostIntenseAndInflections_InlabMethod,
        fiveSpectra_InlabMethod,
        mostIntense
    }

    public enum adquisitionMode
    {
        RetentionTime,
        position
    }
    public enum massUnits
    {
        Th,
        mTh,
        ppm,
    }
    public enum spectrumTypes
    {
        Unknown,
        Full,
        ZoomScan,
        MSMS,
        SRM,
        SIM,
        ERROR,
        Other,
    }

    public enum spectrumPositions
    {
        previous,
        same,
        next
    }

    public class BinStackOptions 
    {

        public BinStackOptions()
        {
            useParentalMass = false;
            useWindow = false;
            units = massUnits.Th;
            individualRow = false;
        }

        private spectrumTypes spectrumTypeVal;
        private adquisitionMode modeVal;
        private spectrumPositions spectrumPosVal;
        private float retentionTimeVal;
        private bool useParentalMassVal;
        private bool useWindowVal;
        private float parentalMassVal;
        private float windowStartVal;
        private float windowEndVal;
        private massUnits unitsVal;
        private bool individualRowVal;

        public spectrumTypes spectrumType
        {
            get { return spectrumTypeVal; }
            set { spectrumTypeVal = value; }
        }
        public adquisitionMode mode
        {
            get { return modeVal; }
            set { modeVal = value; }
        }
        public spectrumPositions spectrumPos
        {
            get { return spectrumPosVal; }
            set { spectrumPosVal = value; }
        }
        public float retentionTime
        {
            get { return retentionTimeVal; }
            set { retentionTimeVal = value; }
        }
        public bool useParentalMass
        {
            get { return useParentalMassVal; }
            set { useParentalMassVal = value; }
        }
        public bool useWindow
        {
            get { return useWindowVal; }
            set { useWindowVal = value; }
        }
        public float parentalMass
        {
            get { return parentalMassVal; }
            set { parentalMassVal = value; }
        }
        public float windowStart
        {
            get { return windowStartVal; }
            set { windowStartVal = value; }
        }
        public float windowEnd
        {
            get { return windowEndVal; }
            set { windowEndVal = value; }
        }
        public massUnits units
        {
            get { return unitsVal; }
            set { unitsVal = value; }
        }
        public bool individualRow
        {
            get { return individualRowVal; }
            set { individualRowVal = value; }
        }

        public bool averageSpectra;
        public averagingMethod averagingMethod;
    }

    public class DA_raw
    {
        public Comb.mzI[] extData;
        public string instrumentName;
        string rawFilePath = "";

        
        /// <summary>
        /// Reads a given set of selected scans of a given raw 
        /// </summary>
        /// <param name="filePath">(string) directory path of the raws</param>
        /// <param name="rawfile">(string) name of the raw to open</param>
        /// <param name="scannumber">(int[]) set of MSMS id scans to read</param>
        /// <param name="options">options selected to create the binStack</param>
        /// <returns>(Comb.mzI[][]) spectrum of each selected scan</returns>
        public Comb.mzI[][] ReadScanRaw(string filePath, 
                                        string rawfile, 
                                        int[] firstScan,
                                        int[] peakStart,
                                        int[] peakEnd,
                                        double[] parentMassList, 
                                        BinStackOptions options,
                                        Label _status,
                                        ref object _parObject,
                                        int _currNumFrame,
                                        int _totNumFrames)
                                        // string specType, string spectrumPosition)
        {
            int stepSearch;
            quadraticEquation _par = new quadraticEquation();

            switch(options.spectrumPos)
            {
                case spectrumPositions.previous:
                     stepSearch = -1;
                     break;
                case  spectrumPositions.next:
                     stepSearch = 1;
                     break;
                default:
                     stepSearch = 0;
                     break;
            }
           

            if (filePath == null || filePath.Length == 0)
                return null;
            if (rawfile == null || rawfile.Length == 0)
                return null;

            Comb.mzI[][] scansRaw = new Comb.mzI[firstScan.GetUpperBound(0) + 1][];

            //open the raw
            try
            {
              
                int iMaxGeneration = GC.MaxGeneration;

                long iTotalMem = GC.GetTotalMemory(true);
                rawFilePath = filePath.ToString().Trim() + "\\" + rawfile.ToString().Trim();
            }
            catch
            {
                return null;
            }

            rawStats frmReader = new rawStats();

            //frmReader.Show();
            frmReader.openRawFast(rawFilePath);
            int nSpec = frmReader.lastSpectrumNumber();
            frmReader.initialiseSpectrumTypes(nSpec);
            //object a2 = frmReader.getSpectrum(1900);
                  

#region mode: retention time (not used)
//            if (options.mode == adquisitionMode.RetentionTime)
//            {
//                double startTime = new double();
//                double endTime = new double();
//                double centralTime = new double();
//                //short actualIndex = new short();
//                //double actualRetentionTime = new double();
//                int firstScan = new int();
//                int lastScan = new int();

//                centralTime = frmReader.getRTfromScanNumber(1900);
//                startTime = frmReader.getRTfromScanNumber(1);
//                endTime = frmReader.getRTfromScanNumber(frmReader.numSpectra());

//                //_Spectra.IndexToRetentionTime(1, ref actualIndex, ref startTime);
//                //_Spectra.IndexToRetentionTime(_Spectra.Count, ref actualIndex, ref endTime);

//                for (int i = 0; i <= scannumber.GetUpperBound(0); i++)
//                {
//                    double parentMass = 0;
//                    if (options.useParentalMass)
//                    {
//                        parentMass = parentMassList[i];
//                    }


//                    centralTime = frmReader.getRTfromScanNumber(scannumber[i, 0]);

//                    //_Spectra.IndexToRetentionTime((short)scannumber[i], ref actualIndex, ref centralTime);

//                    float timeMargin = options.retentionTime / (2 * 60);
//                    double startCheckingTime = centralTime - timeMargin;
//                    double stopCheckingTime = centralTime + timeMargin;

//                    if (startCheckingTime < startTime) { startCheckingTime = startTime; }
//                    if (stopCheckingTime > endTime) { stopCheckingTime = endTime; }

//                    firstScan = frmReader.retentionTimeToIndex(startCheckingTime);
//                    lastScan = frmReader.retentionTimeToIndex(stopCheckingTime);

//                    //_Spectra.RetentionTimeToIndex(startCheckingTime, ref actualRetentionTime, ref firstScan);
//                    //_Spectra.RetentionTimeToIndex(stopCheckingTime, ref actualRetentionTime, ref lastScan);

//                    try
//                    {
//                        for (int j = firstScan; j <= lastScan; j++)
//                        {
//                            int tentativeSpectrum = (int)j;

//                            spectrumTypes ff = frmReader.spectrumTypeFromScanNumber(j);
//                            //XCALIBURFILESLib.XFilter filt = (XCALIBURFILESLib.XFilter)_filter.ScanNumber(tentativeSpectrum);
//                            //string ff = filt.Text;

//                            //XCALIBURFILESLib.XSpectrumRead Xspec = new XCALIBURFILESLib.XSpectrumRead();
//                            double[,] specData = new double[10, 0];

//                            if (options.spectrumType == ff)
//                            {
//                                specData = (double[,])frmReader.getSpectrum(tentativeSpectrum);
//                                scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, specData).Clone();
//                            }

                            
//                            //switch (options.spectrumType)
//                            //{
//                            //    case spectrumTypes.Full:
//                            //        if (ff == spectrumTypes.Full)
//                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
//                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
//                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
//                            //        break;
//                            //    case spectrumTypes.MSMS:
//                            //        if (ff == spectrumTypes.MSMS)
//                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
//                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
//                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
//                            //        break;
//                            //    case spectrumTypes.ZoomScan:
//                            //        if (ff == spectrumTypes.ZoomScan)
//                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
//                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
//                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
//                            //        break;
//                            //}
                            
//                        }
//                    }
//                    catch
//                    {
//                        //_Raw.Close();

//                        //_Detector = null;
//                        //_Spectra = null;
//                        //GC.Collect();
//                        //GC.WaitForPendingFinalizers();

//                        ////MessageBox.Show("Could not open selected raw file: " + e.Message);
//                        ////Application.DoEvents();
//                        //return null;
//                    }
//                }
//            } 
#endregion

#region mode: position

            if (!(options.averagingMethod == averagingMethod.mostIntense
                || options.averagingMethod == averagingMethod.none))
            {
                if (_parObject == "")
                {
                    _par.rawFilePath = rawFilePath;
                    _par.hasBeenCalculated = false;
                    _parObject = (object)_par;
                }
                else
                {
                    _par = (quadraticEquation)_parObject;
                    if (_par.rawFilePath != rawFilePath)
                    {
                        _par.rawFilePath = rawFilePath;
                        _par.hasBeenCalculated = false;
                    }
                }

                string filter = "FTMS + p NSI Full ms"; // filter for fulls
                int totScans = firstScan.Length;

                for (int i = 0; i < totScans; i++)
                {
                    _status.Text = "Averaging scan " + (i + 1).ToString() + "/" + totScans.ToString() +
                        " (frame " + _currNumFrame.ToString() + "/" + _totNumFrames.ToString() + ")";
                    Application.DoEvents();

                    double[,] specData = new double[10, 0];

                    try
                    {
                        int maxNumberOfLateralZeros = 4;
                        double firstMz = 0;
                        double lastMz = 0;

                        if (!options.useParentalMass && !options.useWindow)
                        {
                            double[,] firstSpec = (double[,])frmReader.getSpectrum(peakStart[i]);
                            getMinAndMaxMass(options, parentMassList[i], firstSpec, ref firstMz, ref lastMz);
                        }

                        if (options.useParentalMass)
                            getMinAndMaxMass(options, parentMassList[i], null, ref firstMz, ref lastMz);

                        if(options.useWindow)
                        {
                            firstMz = options.windowStart;
                            lastMz = options.windowEnd;
                        }

                        // fast but bad

                        switch (options.averagingMethod)
                        {
                            case averagingMethod.everything_ThermoMethod_noWidth:
                                {
                                    // mostly fast but very bad
                                    
                                    specData = (double[,])frmReader.getAveragedSpectrumThermoFunction(peakStart[i],
                                                                            peakEnd[i],
                                                                            firstMz,
                                                                            lastMz,
                                                                            filter,
                                                                            0);
                                    break;
                                }

                            case averagingMethod.everything_ThermoMethod_Width0001:
                                {
                                    // mostly fast but somewhat acceptable
                                    double optimisedWidth = 0.0001;
                                    specData = (double[,])frmReader.getAveragedSpectrumThermoFunction(peakStart[i],
                                                                            peakEnd[i],
                                                                            firstMz,
                                                                            lastMz,
                                                                            filter,
                                                                            optimisedWidth);
                                    break;
                                }

                            case averagingMethod.everything_InlabMethod:
                                {
                                    // slow but good
                                    specData = (double[,])frmReader.getAveragedSpectrum(peakStart[i],
                                                                                        peakEnd[i],
                                                                                        firstScan[i],
                                                                                        true, 0,
                                                                                        firstMz,
                                                                                        lastMz,
                                                                                        maxNumberOfLateralZeros,
                                                                                        ref _par,
                                                                                        _status);
                                    _parObject = (object)_par;
                                    break;

                                    //msnPerFull.rawStats.saveIndividualSpectrum(specData, "jv");

                                    
                                }

                            case averagingMethod.mostIntenseAndInflections_InlabMethod:
                                {
                                    specData = (double[,])frmReader.getAveragedSpectrum(peakStart[i],
                                                                                        peakEnd[i],
                                                                                        firstScan[i],
                                                                                        true, 3,
                                                                                        firstMz,
                                                                                        lastMz,
                                                                                        maxNumberOfLateralZeros,
                                                                                        ref _par,
                                                                                        _status);
                                    _parObject = (object)_par;
                                    break;
                                }

                            case averagingMethod.fiveSpectra_InlabMethod:
                                {
                                    specData = (double[,])frmReader.getAveragedSpectrum(peakStart[i],
                                                                                        peakEnd[i],
                                                                                        firstScan[i],
                                                                                        false, 5,
                                                                                        firstMz,
                                                                                        lastMz,
                                                                                        maxNumberOfLateralZeros,
                                                                                        ref _par,
                                                                                        _status);
                                    _parObject = (object)_par;
                                    break;
                                }
                        }

                        //double[,] otherData = (double[,])frmReader.getSpectrum(firstScan[i]);
                        
                        //msnPerFull.rawStats.saveIndividualSpectrum(specData, "thermo_width1-001");
                    }
                    catch
                    {
                        //this catch prevents from non existing scans
                    }

                    if (specData == null)
                    {
                        _status.Text = "Raw file error found.";
                        return null;
                    }

                    try
                    {
                        double parentMass = 0;
                        if (options.useParentalMass)
                            parentMass = parentMassList[i];

                        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, specData).Clone();
                    }
                    catch
                    {
                        //this catch prevents from empty scans 
                    }
                }
            }
            else
            {
                if (options.mode == adquisitionMode.position)
                {
                    //for each identified spectrum (~each row of the QuiXML)
                    for (int i = 0; i <= firstScan.GetUpperBound(0); i++)
                    {
                        bool spectrumFound = false;

                        double parentMass = 0;
                        if (options.useParentalMass)
                        {
                            parentMass = parentMassList[i];
                        }

                        try
                        {
                            int tentativeSpectrum = (int)(firstScan[i] + stepSearch);
                            int spectrumToQuantitate = 0;

                            if (options.spectrumPos == spectrumPositions.same)
                            {
                                spectrumFound = true;
                                spectrumToQuantitate = tentativeSpectrum;
                            }

                            while (!spectrumFound)
                            {
                                if (tentativeSpectrum < 0)
                                {
                                    MessageBox.Show("Error: reached lower bound for scans.");
                                    return null;
                                }
                                
                                if (tentativeSpectrum > nSpec)
                                {
                                    MessageBox.Show("Error: reached upper bound for scans.");
                                    return null;
                                }

                                try
                                {
                                    //XCALIBURFILESLib.XFilter filt = (XCALIBURFILESLib.XFilter)_filter.ScanNumber(tentativeSpectrum);  //(short)scannumber[i]
                                    spectrumTypes st = frmReader.spectrumTypeFromScanNumber(tentativeSpectrum);
                                    //string ff = "";// filt.Text;


                                    if (options.spectrumType == st)
                                    {
                                        spectrumFound = true;
                                        spectrumToQuantitate = tentativeSpectrum;
                                    }

                                }
                                catch
                                {
                                    GC.Collect();
                                    GC.WaitForPendingFinalizers();

                                    //MessageBox.Show("Could not open selected raw file: " + e.Message);
                                    //Application.DoEvents();
                                    return null;
                                }

                                tentativeSpectrum = (int)(tentativeSpectrum + stepSearch);
                            }

                            if (spectrumFound)
                            {
                                double[,] specData = new double[10, 0];
                                try
                                {
                                    specData = (double[,])frmReader.getSpectrum(spectrumToQuantitate);
                                }
                                catch
                                {
                                    //this catch prevents from non existing scans
                                }
                                try
                                {
                                    scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, specData).Clone();
                                }
                                catch
                                {
                                    //this catch prevents from empty scans 
                                }
                            }

                            //XCALIBURFILESLib.XSpectrumRead Xspec = frmReader.getSpectrum(spectrumToQuantitate) as XCALIBURFILESLib.XSpectrumRead;
                            //XCALIBURFILESLib.XSpectrumRead Xspec = _Spectra.Item(spectrumToQuantitate) as XCALIBURFILESLib.XSpectrumRead;

                            #region not-useful
                            //try
                            //{

                            //    XCALIBURFILESLib.XParentScans XparentScans = Xspec.ParentScans as XCALIBURFILESLib.XParentScans;
                            //    short prScansCount = XparentScans.Count;
                            //    short numOfSpectra = _Spectra.Count;
                            //    string[] fll = new string[_filter.Count];
                            //    for (int k=0; k<_filter.Count;k++)
                            //    {
                            //        XCALIBURFILESLib.XFilter fil= (XCALIBURFILESLib.XFilter)_filter.Item(1);
                            //        fll[k] = fil.Text;
                            //        //fil.Validate
                            //    }

                            //}
                            //catch { }


                            //Get the instrument name
                            //XCALIBURFILESLib.XInstrument instrument = _Detector.Instrument as XCALIBURFILESLib.XInstrument;
                            //instrumentName = instrument.Name;
                            //instrument = null;
                            // spectrum data
                            #endregion

                            //***scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec);

                        }
                        catch
                        {

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            //MessageBox.Show("Could not open selected raw file: " + e.Message);
                            //Application.DoEvents();
                            return null;

                        }
                    }
                }
            }
#endregion

 
            GC.Collect();
            frmReader.closeRaw();

            return scansRaw;

        }

        private Comb.mzI[][] newScansRaw(BinStackOptions _options, 
                                        Comb.mzI[][] _scansRaw, 
                                        int _spectrumToQuantitate, 
                                        double _parentMass, 
                                        double[,] _specData)
        {
            Comb.mzI[][] scansRaw = (Comb.mzI[][])_scansRaw.Clone();

            double[,] data = _specData as double[,];

            double minMass = 0;
            double maxMass = 0;

            int minMassPos = 0;
            int maxMassPos = data.GetUpperBound(1);
            
            getMinAndMaxMass(_options, _parentMass, data, ref minMass, ref maxMass);

            //determine index of minimum mass
            for (int pos = 0; pos <= data.GetUpperBound(1); pos++)
            {
                if (data[0, pos] < minMass) minMassPos = pos + 1;
                else break;
            }

            //determine index of maximum mass
            for (int pos = data.GetUpperBound(1); pos >= 0; pos--)
            {
                if (data[0, pos] > maxMass) maxMassPos = pos - 1;
                else break;
            }

            int diffPos = maxMassPos - minMassPos;

            extData = new Comb.mzI[diffPos + 1];

            int counter = 0;
            for (int k = minMassPos; k <= maxMassPos; k++)
            {
                extData[counter].mz = data[0, k];
                extData[counter].I = data[1, k];
                counter++;
            }

            int i = 0;
            try
            {
                if(scansRaw[_spectrumToQuantitate]==null)
                scansRaw[_spectrumToQuantitate] = new Comb.mzI[1000];

                switch(_options.mode)
                {
                    case adquisitionMode.position:
                    scansRaw[_spectrumToQuantitate] = (Comb.mzI[])extData.Clone();
                    break;

                    

                    case adquisitionMode.RetentionTime:
                    for(i=extData.GetLowerBound(0);i<extData.GetUpperBound(0);i++)
                    {
                        scansRaw[_spectrumToQuantitate][i].I = extData[i].I;
                        scansRaw[_spectrumToQuantitate][i].mz = extData[i].mz;
                    }
                    break;
                }
            }
            catch
            {
                //blank spectrum
                extData = new Comb.mzI[1];
                //scansRaw[_spectrumToQuantitate] = (Comb.mzI[])extData.Clone();
                scansRaw[_spectrumToQuantitate] = null;
                //return null;

            }

            return scansRaw;
        }

        private static void getMinAndMaxMass(BinStackOptions _options,
                                                double _parentMass,
                                                double[,] data,
                                                ref double minMass,
                                                ref double maxMass)
        {
            // this is just for safety, but depending on the machine, more might be better
            double safetyMinMass = 250;
            double safetyMaxMass = 2500;

            if (data != null)
            {
                minMass = data[0, 0];
                maxMass = data[0, data.GetUpperBound(1)];
            }
            else
            {
                minMass = safetyMinMass;
                maxMass = safetyMaxMass;
            }

            if (_options.useWindow)
            {
                //Mass conversions (mz data in the raw file is expressed in Th)
                switch (_options.units)
                {
                    case massUnits.Th:
                        minMass = _options.windowStart;
                        maxMass = _options.windowEnd;
                        break;
                    case massUnits.mTh:
                        // Th = mTh * 1e3
                        minMass = _options.windowStart * 1000;
                        maxMass = _options.windowEnd * 1000;
                        break;
                    case massUnits.ppm:
                        // Th = ppm * parentalMass * 1e-6
                        //no meaning for window, already took care of it
                        break;
                }

                if (data != null)
                {
                    if (minMass < data[0, 0]) minMass = data[0, 0];
                    if (maxMass > data[0, data.GetUpperBound(1)]) maxMass = data[0, data.GetUpperBound(1)];
                }
                else
                {
                    if (minMass < safetyMinMass) minMass = safetyMinMass;
                    if (maxMass > safetyMaxMass) maxMass = safetyMaxMass;
                }
            }

            if (_options.useParentalMass)
            {
                //Mass conversions (mz data in the raw file is expressed in Th)
                switch (_options.units)
                {
                    case massUnits.Th:
                        minMass = _parentMass - _options.parentalMass / 2;
                        maxMass = _parentMass + _options.parentalMass / 2;
                        break;
                    case massUnits.mTh:
                        // Th = mTh * 1e3
                        minMass = _parentMass - _options.parentalMass * 1000 / 2;
                        maxMass = _parentMass + _options.parentalMass * 1000 / 2;
                        break;
                    case massUnits.ppm:
                        // Th = ppm * parentalMass * 1e-6
                        minMass = _parentMass - _options.parentalMass * _parentMass * 1e-6;
                        maxMass = _parentMass - _options.parentalMass * _parentMass * 1e-6;
                        break;
                }

                if (data != null)
                {
                    if (minMass < data[0, 0]) minMass = data[0, 0];
                    if (maxMass > data[0, data.GetUpperBound(1)]) maxMass = data[0, data.GetUpperBound(1)];
                }
                else
                {
                    if (minMass < safetyMinMass) minMass = safetyMinMass;
                    if (maxMass > safetyMaxMass) maxMass = safetyMaxMass;
                }
            }
        }

    }

}
