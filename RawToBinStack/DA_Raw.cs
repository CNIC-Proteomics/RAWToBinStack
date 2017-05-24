using System;
using System.Collections.Generic;
using System.Text;
using QuiXoT.math;
using System.Windows.Forms;

namespace RawToBinStack
{

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

    // updated from QuiXtoQuiX 2.11
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
            parentalMassUnits = massUnits.Th;
            individualRow = false;
        }

        public bool sumSpectra;
        private spectrumTypes spectrumTypeVal;
        private adquisitionMode modeVal;
        private spectrumPositions spectrumPosVal;
        private float retentionTimeVal;
        private bool useParentalMassVal;
        private float parentalMassVal;
        private massUnits parentalMassUnitsVal;
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
        public float parentalMass
        {
            get { return parentalMassVal; }
            set { parentalMassVal = value; }
        }
        public massUnits parentalMassUnits
        {
            get { return parentalMassUnitsVal; }
            set { parentalMassUnitsVal = value; }
        }
        public bool individualRow
        {
            get { return individualRowVal; }
            set { individualRowVal = value; }
        }


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
                                        int[] scannumber, 
                                        double[] parentMassList, 
                                        BinStackOptions options) 
                                        // string specType, string spectrumPosition)
        {
            int stepSearch;

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

            Comb.mzI[][] scansRaw = new Comb.mzI[scannumber.GetUpperBound(0) + 1][];

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

            QuiXoT.Forms.frmInvisible frmReader = new QuiXoT.Forms.frmInvisible();

            //frmReader.Show();
            frmReader.openRaw(rawFilePath);
            int nSpec = frmReader.numSpectra();
            //object a2 = frmReader.getSpectrum(1900);

#region mode: retention time
            if (options.mode == adquisitionMode.RetentionTime)
            {
                double startTime = new double();
                double endTime = new double();
                double centralTime = new double();
                //short actualIndex = new short();
                //double actualRetentionTime = new double();
                int firstScan = new int();
                int lastScan = new int();

                centralTime = frmReader.indexToRetentionTime(1900);
                startTime = frmReader.indexToRetentionTime(1);
                endTime = frmReader.indexToRetentionTime(frmReader.spectrumCount());

                //_Spectra.IndexToRetentionTime(1, ref actualIndex, ref startTime);
                //_Spectra.IndexToRetentionTime(_Spectra.Count, ref actualIndex, ref endTime);

                for (int i = 0; i <= scannumber.GetUpperBound(0); i++)
                {
                    double parentMass = 0;
                    if (options.useParentalMass)
                    {
                        parentMass = parentMassList[i];
                    }


                    centralTime = frmReader.indexToRetentionTime(scannumber[i]);

                    //_Spectra.IndexToRetentionTime((short)scannumber[i], ref actualIndex, ref centralTime);

                    float timeMargin = options.retentionTime / (2 * 60);
                    double startCheckingTime = centralTime - timeMargin;
                    double stopCheckingTime = centralTime + timeMargin;

                    if (startCheckingTime < startTime) { startCheckingTime = startTime; }
                    if (stopCheckingTime > endTime) { stopCheckingTime = endTime; }

                    firstScan = frmReader.retentionTimeToIndex(startCheckingTime);
                    lastScan = frmReader.retentionTimeToIndex(stopCheckingTime);

                    //_Spectra.RetentionTimeToIndex(startCheckingTime, ref actualRetentionTime, ref firstScan);
                    //_Spectra.RetentionTimeToIndex(stopCheckingTime, ref actualRetentionTime, ref lastScan);

                    try
                    {
                        for (int j = firstScan; j <= lastScan; j++)
                        {
                            int tentativeSpectrum = (int)j;

                            spectrumTypes ff = frmReader.spectrumTypeFromScanNumber(j);
                            //XCALIBURFILESLib.XFilter filt = (XCALIBURFILESLib.XFilter)_filter.ScanNumber(tentativeSpectrum);
                            //string ff = filt.Text;

                            //XCALIBURFILESLib.XSpectrumRead Xspec = new XCALIBURFILESLib.XSpectrumRead();
                            double[,] specData=new double[10,0];

                            if (options.spectrumType == ff)
                            {
                                specData = (double[,])frmReader.getSpectrum(tentativeSpectrum);
                                scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, specData).Clone();
                            }

                            
                            //switch (options.spectrumType)
                            //{
                            //    case spectrumTypes.Full:
                            //        if (ff == spectrumTypes.Full)
                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
                            //        break;
                            //    case spectrumTypes.MSMS:
                            //        if (ff == spectrumTypes.MSMS)
                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
                            //        break;
                            //    case spectrumTypes.ZoomScan:
                            //        if (ff == spectrumTypes.ZoomScan)
                            //            Xspec = (XCALIBURFILESLib.XSpectrumRead)frmReader.getSpectrum(tentativeSpectrum);
                            //            //Xspec = _Spectra.Item(tentativeSpectrum) as XCALIBURFILESLib.XSpectrumRead;
                            //        scansRaw = (Comb.mzI[][])newScansRaw(options, scansRaw, i, parentMass, Xspec).Clone();
                            //        break;
                            //}
                            
                        }
                    }
                    catch
                    {
                        //_Raw.Close();

                        //_Detector = null;
                        //_Spectra = null;
                        //GC.Collect();
                        //GC.WaitForPendingFinalizers();

                        ////MessageBox.Show("Could not open selected raw file: " + e.Message);
                        ////Application.DoEvents();
                        //return null;
                    }
                }
            } 
#endregion

#region mode: position

            if (options.mode == adquisitionMode.position)
            {

                //for each identified spectrum (~each row of the QuiXML)
                for (int i = 0; i <= scannumber.GetUpperBound(0); i++)
                {


                    bool spectrumFound = false;

                    double parentMass = 0;
                    if (options.useParentalMass)
                    {
                        parentMass = parentMassList[i];
                    }

                    try
                    {
                        int tentativeSpectrum = (int)(scannumber[i] + stepSearch);
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

                            if (tentativeSpectrum > frmReader.spectrumCount())
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
#endregion

 
            GC.Collect();
            frmReader.Close();

            return scansRaw;

        }

        private Comb.mzI[][] newScansRaw(BinStackOptions _options, 
                                        Comb.mzI[][] _scansRaw, 
                                        int _spectrumToQuantitate, 
                                        double _parentMass, 
                                        double[,] _specData)
        {
            double[,] data = _specData as double[,];
            
            double minMass = data[0, 0];
            double maxMass = data[0, data.GetUpperBound(1)];
            int minMassPos = 0;
            int maxMassPos = data.GetUpperBound(1);
            
            Comb.mzI[][] scansRaw = (Comb.mzI[][])_scansRaw.Clone();

            //if (minMassPos == 0)
            //{
            //    return scansRaw;
            //}

            if (_options.useParentalMass)
            {
                //Mass conversions (mz data in the raw file is expressed in Th)
                switch (_options.parentalMassUnits)
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

                if (minMass < data[0, 0]) minMass = data[0, 0];
                if (maxMass > data[0, data.GetUpperBound(1)]) maxMass = data[0, data.GetUpperBound(1)];
            }

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

    }

}
