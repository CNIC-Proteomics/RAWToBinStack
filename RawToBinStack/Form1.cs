using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using QuiXoT.DA_stackRAW;
using QuiXoT.DA_Raw;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;


namespace RawToBinStack
{
    public partial class frmMain : Form
    {
        ArrayList spectrumTypesChoose;
        ArrayList specPositionsChoose;
        ArrayList massUnitsChoose;
        BinStackOptions options;

        public frmMain()
        {
            InitializeComponent();
            importMethodChecks();

            this.Text = "RawToBinStack v." + RawToBinStack.Properties.Settings.Default.version;

            spectrumTypesChoose = new ArrayList();

            spectrumTypesChoose.Add(spectrumTypes.Full);
            spectrumTypesChoose.Add(spectrumTypes.MSMS);
            spectrumTypesChoose.Add(spectrumTypes.ZoomScan);

            foreach(spectrumTypes spectype in spectrumTypesChoose)
            {
                cbxSpecType.Items.Add(spectype.ToString());
            }

            specPositionsChoose = new ArrayList();
            specPositionsChoose.Add(spectrumPositions.previous);
            specPositionsChoose.Add(spectrumPositions.same);
            specPositionsChoose.Add(spectrumPositions.next);

            foreach (spectrumPositions specPos in specPositionsChoose)
            {
                cbxPosition.Items.Add(specPos.ToString());
            }

            bool useppms = true;
            makeListOfUnits(useppms);
        }

        private void makeListOfUnits(bool useppms)
        {
            massUnitsChoose = new ArrayList();
            massUnitsChoose.Add(massUnits.Th);
            massUnitsChoose.Add(massUnits.mTh);
            if (useppms) massUnitsChoose.Add(massUnits.ppm);

            cbxUnits.Items.Clear();
            foreach (massUnits mu in massUnitsChoose)
                cbxUnits.Items.Add(mu.ToString());

            cbxUnits.SelectedIndex = 0;
        }

        private void createBinStackBtn_Click(object sender, EventArgs e)
        {
            options = new BinStackOptions();

            if (tbxQuiXML.Text.Trim() == "")
            {
                MessageBox.Show("No QuiXML selected", "Error");
                return;
            }

            int quiXMLlength = tbxQuiXML.Text.Trim().Length;
            if (quiXMLlength > 251)
            {
                MessageBox.Show(String.Concat("QuiXML file path too long,",
                    "\nplease shorten it at least ",
                    (quiXMLlength - 251).ToString(),
                    " characters\nand paste it here again"), "Error");
                return;
            }

            if (tbxSchema.Text.Trim() == "")
            {
                MessageBox.Show("No QuiXML schema (xsd) selected", "Error");
                return;             
            }
            if (tbxSpecFolder.Text.Trim() == "")
            {
                MessageBox.Show("No spectra folder selected", "Error");
                return;
            }
            if (cbxSpecType.Text.Trim() == "")
            {
                MessageBox.Show("No spectrum type selected", "Error");
                return;
            }

            if (cbxPosition.Text == "")
            {
                MessageBox.Show("No spectrum position selected", "Error");
                return;
            }

            if (rbnImportParental.Checked && tbxParentalMZ.Text == "")
            {
                MessageBox.Show("No parental mass selected", "Error");
                return;
            }

            if (rbnImportWindowBetween.Checked && (tbxWindowEnd.Text == "" || tbxWindowStart.Text == ""))
            {
                MessageBox.Show("No window selected", "Error");
                return;
            }

            options.mode = adquisitionMode.position;

            foreach (spectrumPositions sp in specPositionsChoose)
            {
                if (cbxPosition.Text == sp.ToString())
                {
                    options.spectrumPos = sp;
                    break;
                }
            }
            
            foreach (spectrumTypes spt in spectrumTypesChoose)
            {
                if (cbxSpecType.Text == spt.ToString())
                {
                    options.spectrumType = spt;
                    break;
                }
            }



            if (rbnImportParental.Checked)
            {
                options.useParentalMass = true;
                options.useWindow = false;

                foreach (massUnits mu in massUnitsChoose)
                {
                    if (cbxUnits.Text == mu.ToString())
                    {
                        options.units = mu;
                        break;
                    }
                }
                try 
                {
                    options.parentalMass = float.Parse(tbxParentalMZ.Text.Trim());
                }
                catch 
                {
                    MessageBox.Show("Parental mass is not valid (not a number)", "Error");
                    return;
                }
            }

            if (rbnImportWindowBetween.Checked)
            {
                options.useWindow = true;
                options.useParentalMass = false;

                foreach (massUnits mu in massUnitsChoose)
                {
                    if (cbxUnits.Text == mu.ToString())
                    {
                        options.units = mu;
                        break;
                    }
                }
                try
                {
                    options.windowStart = float.Parse(tbxWindowStart.Text.Trim());
                    options.windowEnd = float.Parse(tbxWindowEnd.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Parental mass is not valid (not a number)", "Error");
                    return;
                }
            }

            if (chbAverageSpectra.Checked)
            {
                options.averageSpectra = true;

                /*Everything between inflection points
Maximum + inflection points (not implemented yet)
Three most intense (not implemented yet)*/

                string methodName = cbxAverageMethod.Text.Split(':')[0];
                switch(methodName)
                {
                    case "THERMO WIDTH": //Everything between inflection points (Thermo method, width = 0.0001)
                        options.averagingMethod = averagingMethod.everything_ThermoMethod_Width0001;
                        break;

                    case "LAB ALL": // Everything between inflection points (Thermo method, no width specified)
                        options.averagingMethod = averagingMethod.everything_ThermoMethod_noWidth;
                        break;

                    case "THERMO ALL": // Everything between inflection points (averaging extrapolations)
                        options.averagingMethod = averagingMethod.everything_InlabMethod;
                        break;

                    case "LAB INFLECMAX": // Most intense + inflection points (averaging extrapolations)
                        options.averagingMethod = averagingMethod.mostIntenseAndInflections_InlabMethod;
                        break;

                    case "LAB5": // Five spectra within the chromatographic peak (averaging extrapolations)
                        options.averagingMethod = averagingMethod.fiveSpectra_InlabMethod;
                        break;

                    case "Three most intense (not implemented yet)":
                        options.averagingMethod = averagingMethod.mostIntense;
                        break;

                    default: options.averagingMethod = averagingMethod.none;
                        break;
                }
            }
            else
            {
                options.averageSpectra = false;
                options.averagingMethod = averagingMethod.none;
            }

            this.lblStatus.Text = "Generating binaries stack... loading quiXML...";
            this.lblStatus.Visible = true;

            genStack(tbxSpecFolder.Text.Trim(), options);
        }


        private void genStack(  string rawPath,
                                BinStackOptions options)
        {
            Application.DoEvents();

            try
            {
                this.lblStatus.Text = "Generating binaries stack... Creating index...";
                this.lblStatus.Visible = true;

                string idFileXml = this.tbxQuiXML.Text.Trim();
                string idSchema = this.tbxSchema.Text.Trim();
                string stackIndexFolder = idFileXml.Substring(0, idFileXml.LastIndexOf(@"\")) + "\\binStack\\";
                string stackIndexFile = stackIndexFolder + "index.idx";
                if (!Directory.Exists(stackIndexFolder))
                {
                    Directory.CreateDirectory(stackIndexFolder);
                }

                //scans by frame
                int scbyframe = 100;


                //load the XML file to a DataSet object
                DataSet quiXML = new DataSet();
                quiXML.ReadXmlSchema(idSchema);
                quiXML.ReadXml(idFileXml, XmlReadMode.Auto);

                DataView quiXMLv = new DataView(quiXML.Tables["peptide_match"]);


                if (!(options.averagingMethod == averagingMethod.mostIntense
                        || options.averagingMethod == averagingMethod.none))
                {
                    if (!quiXMLv.Table.Columns.Contains("PeakStart") ||
                            !quiXMLv.Table.Columns.Contains("PeakEnd"))
                    {
                        MessageBox.Show("This QuiXML file does not seem" +
                            "to contain PeakStart / PeakEnd columns,\n" +
                            "did you forget to use QuiXtoQuiX to get the inflection points?");
                        lblStatus.Text = "Error: no inflection points in QuiXML.";
                        return;
                    }
                }

                //calculate number of frames
                //int inumFrames = binStack.countFrames(idFileXml, scbyframe);
                int inumFrames = binStack.countFrames(quiXMLv, scbyframe);


                //generate index
                object parObject = "";
                binStack[] stackIndex = binStack.genIndex(quiXMLv,
                                                            rawPath,
                                                            scbyframe,
                                                            options);

                //generate and save frames
                for (int i = 1; i <= inumFrames; i++)
                {
                    Application.DoEvents();
                    this.lblStatus.Text = "Generating binaries stack... generating frame " + i.ToString() + "/" + inumFrames.ToString();
                    binFrame currFrame = binStack.genFrame(stackIndex,
                        i, scbyframe, rawPath, options, this.lblStatus, ref parObject, i, inumFrames);

                    if (currFrame == null) return;

                    string frameFile = stackIndexFolder + currFrame.frame.ToString() + ".bfr";
                    FileStream qFr = new FileStream(frameFile, FileMode.Create, FileAccess.Write);
                    BinaryFormatter bFr = new BinaryFormatter();
                    bFr.Serialize(qFr, currFrame);
                    qFr.Close();
                }


                //save index

                //WARNING: very dangerous change, but necessary to maintain old binstacks: in currFrame
                //         we swap the values of scanNumber by spectrumIndex (once we have obtained the desired
                //         spectrum, we use the unique index (spectrumIndex).
                for (int i = stackIndex.GetLowerBound(0); i <= stackIndex.GetUpperBound(0); i++)
                {
                    for (int j = stackIndex[i].scan.GetLowerBound(0); j <= stackIndex[i].scan.GetUpperBound(0); j++)
                    {
                        int scanNumber_t = stackIndex[i].scan[j].FirstScan;
                        stackIndex[i].scan[j].FirstScan = stackIndex[i].scan[j].spectrumIndex;

                        //this is not necessary, but I want to maintain the scanNumber elsewhere...
                        stackIndex[i].scan[j].spectrumIndex = scanNumber_t;
                    }
                }
                FileStream q = new FileStream(stackIndexFile, FileMode.Create, FileAccess.Write);
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(q, stackIndex);
                q.Close();

                string bsXml = Path.GetDirectoryName(idFileXml) + "\\" +
                    Path.GetFileNameWithoutExtension(idFileXml) + "_bs.xml";

                //Save the quiXML changes (spectrumIndex is now written)
                quiXML.WriteXml(bsXml);

                MessageBox.Show("In order to preserve the old QuiXML, " +
                    "the new QuiXML containing the spectrumIndex has been saved to:\n\n" +
                    bsXml + "\n\n" + "Please, remember to use this new QuiXML or its derivations in further analysis.");
            }
            catch (System.Data.DataException e)
            {
                MessageBox.Show("Unable to generate binaries stack. System Data Exception thrown : " + e.Message);
            }
            catch
            {
                MessageBox.Show("Unable to generate binaries stack");
            }



            this.lblStatus.Text = "";
            this.lblStatus.Visible = false;
            
        }



        #region dragDrops & checkedChangeds

        private void positionRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxPosition.Enabled) cbxPosition.Enabled = true;
        }

        private void retTimeRBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxPosition.Enabled) cbxPosition.Enabled = false;
        }

        private void parentalMassChb_CheckedChanged(object sender, EventArgs e)
        {
            if (tbxParentalMZ.Enabled)
            {
                tbxParentalMZ.Text = "";
                tbxParentalMZ.Enabled = false;
            }
            else tbxParentalMZ.Enabled = true;

            if (cbxUnits.Enabled) cbxUnits.Enabled = false;
            else cbxUnits.Enabled = true;
        }


        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            tbxQuiXML.Text = files[0].ToString().Trim();
 
        }

        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xml file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    char[] seps = new char[] { '.' };
                    string[] fileSplit = files[0].Split(seps);

                    if (fileSplit[fileSplit.GetUpperBound(0)] == "xml")
                    {
                        e.Effect = DragDropEffects.All;
                    }

                }

            }
        }

        private void quiXMLtxt_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            tbxQuiXML.Text = files[0].ToString().Trim();

        }

        private void quiXMLtxt_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xml file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    char[] seps = new char[] { '.' };
                    string[] fileSplit = files[0].Split(seps);

                    if (fileSplit[fileSplit.GetUpperBound(0)] == "xml")
                    {
                        e.Effect = DragDropEffects.All;
                    }

                }

            }

        }

        private void label2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            tbxSpecFolder.Text = files[0].ToString().Trim();

        }

        private void label2_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xml file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) ==0)
                {
                    //Check wether the file is a folder and it contains RAW files

                    DirectoryInfo myDir = new DirectoryInfo(files[0]);
                      
                    string myPath = "";

                    foreach (FileInfo file in myDir.GetFiles())
                    {
                        myPath = file.ToString();
                        string ext;
                        ext = Path.GetExtension(myPath);
                        if (ext.ToUpper() == ".RAW") e.Effect = DragDropEffects.All;
                        break;                        
                    }
                }
            }
        }

        private void specFolderTxt_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            tbxSpecFolder.Text = files[0].ToString().Trim();

        }

        private void specFolderTxt_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xml file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    //Check wether the file is a folder and it contains RAW files

                    DirectoryInfo myDir = new DirectoryInfo(files[0]);

                    string myPath = "";

                    foreach (FileInfo file in myDir.GetFiles())
                    {
                        myPath = file.ToString();
                        string ext;
                        ext = Path.GetExtension(myPath);
                        if (ext.ToUpper() == ".RAW")
                        {
                            e.Effect = DragDropEffects.All;
                            break;
                        }
                    }
                }
            }

        }

        private void xsdTxt_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            tbxSchema.Text = files[0].ToString().Trim();
            
        }

        private void xsdTxt_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xsd file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    char[] seps = new char[] { '.' };
                    string[] fileSplit = files[0].Split(seps);

                    if (fileSplit[fileSplit.GetUpperBound(0)] == "xsd")
                    {
                        e.Effect = DragDropEffects.All;
                    }

                }

            }

        }

        private void label5_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            tbxSchema.Text = files[0].ToString().Trim();
        }

        private void label5_DragEnter(object sender, DragEventArgs e)
        {
            // make sure they're actually dropping files (not text or anything else)
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                // make sure the file is a xsd file and is unique.
                // (without this, the cursor stays a "NO" symbol)
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.GetUpperBound(0) == 0)
                {
                    char[] seps = new char[] { '.' };
                    string[] fileSplit = files[0].Split(seps);

                    if (fileSplit[fileSplit.GetUpperBound(0)] == "xsd")
                    {
                        e.Effect = DragDropEffects.All;
                    }
                }
            }
        }


        #endregion

        private void cbxAverageSpectra_CheckedChanged(object sender, EventArgs e)
        {
            lblAverageMethod.Enabled = chbAverageSpectra.Checked;
            cbxAverageMethod.Enabled = chbAverageSpectra.Checked;

            if (chbAverageSpectra.Checked) cbxPosition.Text = "same";

            cbxPosition.Enabled = !chbAverageSpectra.Checked;

            if (cbxAverageMethod.Text == "") cbxAverageMethod.SelectedIndex = 0;
        }

        private void rbnImportParental_CheckedChanged(object sender, EventArgs e)
        {
            importMethodChecks();
        }

        private void importMethodChecks()
        {
            tbxParentalMZ.Enabled = rbnImportParental.Checked;
            tbxParentalMZ.Visible = rbnImportParental.Checked;
            cbxUnits.Enabled = rbnImportParental.Checked || rbnImportWindowBetween.Checked;
            cbxUnits.Visible = rbnImportParental.Checked || rbnImportWindowBetween.Checked;

            tbxWindowEnd.Enabled = rbnImportWindowBetween.Checked;
            tbxWindowEnd.Visible = rbnImportWindowBetween.Checked;
            tbxWindowStart.Enabled = rbnImportWindowBetween.Checked;
            tbxWindowStart.Visible = rbnImportWindowBetween.Checked;

            if (rbnImportWindowBetween.Checked)
            {
                cbxUnits.Location = new Point(597, 74);
                makeListOfUnits(false);
            }
            else
            {
                cbxUnits.Location = new Point(542, 51);
                makeListOfUnits(true);
            }
        }

        private void rbnImportAll_CheckedChanged(object sender, EventArgs e)
        {
            importMethodChecks();
        }

        private void rbnImportWindowBetween_CheckedChanged(object sender, EventArgs e)
        {
            importMethodChecks();
        }
    }
}
