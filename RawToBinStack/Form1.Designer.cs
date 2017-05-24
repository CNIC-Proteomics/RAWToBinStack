namespace RawToBinStack
{
    partial class frmMain
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbxQuiXML = new System.Windows.Forms.TextBox();
            this.tbxSpecFolder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbxSchema = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbnImportWindowBetween = new System.Windows.Forms.RadioButton();
            this.rbnImportParental = new System.Windows.Forms.RadioButton();
            this.rbnImportAll = new System.Windows.Forms.RadioButton();
            this.tbxWindowEnd = new System.Windows.Forms.TextBox();
            this.tbxWindowStart = new System.Windows.Forms.TextBox();
            this.lblAverageMethod = new System.Windows.Forms.Label();
            this.cbxAverageMethod = new System.Windows.Forms.ComboBox();
            this.chbAverageSpectra = new System.Windows.Forms.CheckBox();
            this.lblPosition = new System.Windows.Forms.Label();
            this.cbxUnits = new System.Windows.Forms.ComboBox();
            this.tbxParentalMZ = new System.Windows.Forms.TextBox();
            this.cbxPosition = new System.Windows.Forms.ComboBox();
            this.cbxSpecType = new System.Windows.Forms.ComboBox();
            this.lblSpecType = new System.Windows.Forms.Label();
            this.createBinStackBtn = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbxQuiXML
            // 
            this.tbxQuiXML.AllowDrop = true;
            this.tbxQuiXML.Location = new System.Drawing.Point(105, 34);
            this.tbxQuiXML.Name = "tbxQuiXML";
            this.tbxQuiXML.Size = new System.Drawing.Size(532, 20);
            this.tbxQuiXML.TabIndex = 0;
            this.tbxQuiXML.DragDrop += new System.Windows.Forms.DragEventHandler(this.quiXMLtxt_DragDrop);
            this.tbxQuiXML.DragEnter += new System.Windows.Forms.DragEventHandler(this.quiXMLtxt_DragEnter);
            // 
            // tbxSpecFolder
            // 
            this.tbxSpecFolder.AllowDrop = true;
            this.tbxSpecFolder.Location = new System.Drawing.Point(105, 92);
            this.tbxSpecFolder.Name = "tbxSpecFolder";
            this.tbxSpecFolder.Size = new System.Drawing.Size(532, 20);
            this.tbxSpecFolder.TabIndex = 2;
            this.tbxSpecFolder.DragDrop += new System.Windows.Forms.DragEventHandler(this.specFolderTxt_DragDrop);
            this.tbxSpecFolder.DragEnter += new System.Windows.Forms.DragEventHandler(this.specFolderTxt_DragEnter);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbxSchema);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbxQuiXML);
            this.groupBox1.Controls.Add(this.tbxSpecFolder);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(658, 130);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "data source";
            // 
            // tbxSchema
            // 
            this.tbxSchema.AllowDrop = true;
            this.tbxSchema.Location = new System.Drawing.Point(105, 64);
            this.tbxSchema.Name = "tbxSchema";
            this.tbxSchema.Size = new System.Drawing.Size(532, 20);
            this.tbxSchema.TabIndex = 1;
            this.tbxSchema.DragDrop += new System.Windows.Forms.DragEventHandler(this.xsdTxt_DragDrop);
            this.tbxSchema.DragEnter += new System.Windows.Forms.DragEventHandler(this.xsdTxt_DragEnter);
            // 
            // label5
            // 
            this.label5.AllowDrop = true;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "QuiXML schema :";
            this.label5.DragDrop += new System.Windows.Forms.DragEventHandler(this.label5_DragDrop);
            this.label5.DragEnter += new System.Windows.Forms.DragEventHandler(this.label5_DragEnter);
            // 
            // label2
            // 
            this.label2.AllowDrop = true;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "spectra folder :";
            this.label2.DragDrop += new System.Windows.Forms.DragEventHandler(this.label2_DragDrop);
            this.label2.DragEnter += new System.Windows.Forms.DragEventHandler(this.label2_DragEnter);
            // 
            // label1
            // 
            this.label1.AllowDrop = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "QuiXML file :";
            this.label1.DragDrop += new System.Windows.Forms.DragEventHandler(this.label1_DragDrop);
            this.label1.DragEnter += new System.Windows.Forms.DragEventHandler(this.label1_DragEnter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbnImportWindowBetween);
            this.groupBox2.Controls.Add(this.rbnImportParental);
            this.groupBox2.Controls.Add(this.rbnImportAll);
            this.groupBox2.Controls.Add(this.tbxWindowEnd);
            this.groupBox2.Controls.Add(this.tbxWindowStart);
            this.groupBox2.Controls.Add(this.lblAverageMethod);
            this.groupBox2.Controls.Add(this.cbxAverageMethod);
            this.groupBox2.Controls.Add(this.chbAverageSpectra);
            this.groupBox2.Controls.Add(this.lblPosition);
            this.groupBox2.Controls.Add(this.cbxUnits);
            this.groupBox2.Controls.Add(this.tbxParentalMZ);
            this.groupBox2.Controls.Add(this.cbxPosition);
            this.groupBox2.Controls.Add(this.cbxSpecType);
            this.groupBox2.Controls.Add(this.lblSpecType);
            this.groupBox2.Location = new System.Drawing.Point(12, 164);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(652, 168);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "options";
            // 
            // rbnImportWindowBetween
            // 
            this.rbnImportWindowBetween.AutoSize = true;
            this.rbnImportWindowBetween.Location = new System.Drawing.Point(270, 74);
            this.rbnImportWindowBetween.Name = "rbnImportWindowBetween";
            this.rbnImportWindowBetween.Size = new System.Drawing.Size(147, 17);
            this.rbnImportWindowBetween.TabIndex = 15;
            this.rbnImportWindowBetween.TabStop = true;
            this.rbnImportWindowBetween.Text = "import between these mzs";
            this.rbnImportWindowBetween.UseVisualStyleBackColor = true;
            this.rbnImportWindowBetween.CheckedChanged += new System.EventHandler(this.rbnImportWindowBetween_CheckedChanged);
            // 
            // rbnImportParental
            // 
            this.rbnImportParental.AutoSize = true;
            this.rbnImportParental.Location = new System.Drawing.Point(270, 52);
            this.rbnImportParental.Name = "rbnImportParental";
            this.rbnImportParental.Size = new System.Drawing.Size(207, 17);
            this.rbnImportParental.TabIndex = 14;
            this.rbnImportParental.TabStop = true;
            this.rbnImportParental.Text = "import only window around parental mz";
            this.rbnImportParental.UseVisualStyleBackColor = true;
            this.rbnImportParental.CheckedChanged += new System.EventHandler(this.rbnImportParental_CheckedChanged);
            // 
            // rbnImportAll
            // 
            this.rbnImportAll.AutoSize = true;
            this.rbnImportAll.Location = new System.Drawing.Point(270, 29);
            this.rbnImportAll.Name = "rbnImportAll";
            this.rbnImportAll.Size = new System.Drawing.Size(148, 17);
            this.rbnImportAll.TabIndex = 13;
            this.rbnImportAll.TabStop = true;
            this.rbnImportAll.Text = "import the whole spectrum";
            this.rbnImportAll.UseVisualStyleBackColor = true;
            this.rbnImportAll.CheckedChanged += new System.EventHandler(this.rbnImportAll_CheckedChanged);
            // 
            // tbxWindowEnd
            // 
            this.tbxWindowEnd.Enabled = false;
            this.tbxWindowEnd.Location = new System.Drawing.Point(541, 74);
            this.tbxWindowEnd.Name = "tbxWindowEnd";
            this.tbxWindowEnd.Size = new System.Drawing.Size(50, 20);
            this.tbxWindowEnd.TabIndex = 12;
            // 
            // tbxWindowStart
            // 
            this.tbxWindowStart.Enabled = false;
            this.tbxWindowStart.Location = new System.Drawing.Point(486, 74);
            this.tbxWindowStart.Name = "tbxWindowStart";
            this.tbxWindowStart.Size = new System.Drawing.Size(50, 20);
            this.tbxWindowStart.TabIndex = 10;
            // 
            // lblAverageMethod
            // 
            this.lblAverageMethod.AutoSize = true;
            this.lblAverageMethod.Enabled = false;
            this.lblAverageMethod.Location = new System.Drawing.Point(36, 131);
            this.lblAverageMethod.Name = "lblAverageMethod";
            this.lblAverageMethod.Size = new System.Drawing.Size(92, 13);
            this.lblAverageMethod.TabIndex = 8;
            this.lblAverageMethod.Text = "averaging method";
            // 
            // cbxAverageMethod
            // 
            this.cbxAverageMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAverageMethod.DropDownWidth = 620;
            this.cbxAverageMethod.Enabled = false;
            this.cbxAverageMethod.FormattingEnabled = true;
            this.cbxAverageMethod.Items.AddRange(new object[] {
            "THERMO ALL: Everything between inflection points (averaging extrapolations) [Good" +
                " up to 140K resolution, but BAD FOR 240K!]",
            "THERMO WIDTH: Everything between inflection points (Thermo method, width = 0.0001" +
                ") [Acceptable for 240K resolution]",
            "LAB5: Five spectra within the chromatographic peak (averaging extrapolations) [Fi" +
                "ne for any resolution, but slow]",
            "LAB ALL: Everything between inflection points (Thermo method, no width specified)" +
                " [Good for any resolution, but very slow]",
            "LAB INFLECMAX: Most intense + inflection points (averaging extrapolations) [Fine " +
                "for any resolution, but slow]"});
            this.cbxAverageMethod.Location = new System.Drawing.Point(134, 126);
            this.cbxAverageMethod.Name = "cbxAverageMethod";
            this.cbxAverageMethod.Size = new System.Drawing.Size(505, 21);
            this.cbxAverageMethod.TabIndex = 7;
            // 
            // chbAverageSpectra
            // 
            this.chbAverageSpectra.AutoSize = true;
            this.chbAverageSpectra.Location = new System.Drawing.Point(25, 101);
            this.chbAverageSpectra.Name = "chbAverageSpectra";
            this.chbAverageSpectra.Size = new System.Drawing.Size(103, 17);
            this.chbAverageSpectra.TabIndex = 6;
            this.chbAverageSpectra.Text = "average spectra";
            this.chbAverageSpectra.UseVisualStyleBackColor = true;
            this.chbAverageSpectra.CheckedChanged += new System.EventHandler(this.cbxAverageSpectra_CheckedChanged);
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(23, 60);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(46, 13);
            this.lblPosition.TabIndex = 5;
            this.lblPosition.Text = "position:";
            // 
            // cbxUnits
            // 
            this.cbxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxUnits.Enabled = false;
            this.cbxUnits.FormattingEnabled = true;
            this.cbxUnits.Location = new System.Drawing.Point(542, 51);
            this.cbxUnits.Name = "cbxUnits";
            this.cbxUnits.Size = new System.Drawing.Size(49, 21);
            this.cbxUnits.TabIndex = 4;
            // 
            // tbxParentalMZ
            // 
            this.tbxParentalMZ.Enabled = false;
            this.tbxParentalMZ.Location = new System.Drawing.Point(486, 51);
            this.tbxParentalMZ.Name = "tbxParentalMZ";
            this.tbxParentalMZ.Size = new System.Drawing.Size(50, 20);
            this.tbxParentalMZ.TabIndex = 3;
            // 
            // cbxPosition
            // 
            this.cbxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPosition.FormattingEnabled = true;
            this.cbxPosition.Location = new System.Drawing.Point(108, 57);
            this.cbxPosition.Name = "cbxPosition";
            this.cbxPosition.Size = new System.Drawing.Size(136, 21);
            this.cbxPosition.TabIndex = 1;
            // 
            // cbxSpecType
            // 
            this.cbxSpecType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSpecType.FormattingEnabled = true;
            this.cbxSpecType.Location = new System.Drawing.Point(108, 28);
            this.cbxSpecType.Name = "cbxSpecType";
            this.cbxSpecType.Size = new System.Drawing.Size(141, 21);
            this.cbxSpecType.TabIndex = 0;
            // 
            // lblSpecType
            // 
            this.lblSpecType.AutoSize = true;
            this.lblSpecType.Location = new System.Drawing.Point(23, 31);
            this.lblSpecType.Name = "lblSpecType";
            this.lblSpecType.Size = new System.Drawing.Size(76, 13);
            this.lblSpecType.TabIndex = 4;
            this.lblSpecType.Text = "spectrum type:";
            // 
            // createBinStackBtn
            // 
            this.createBinStackBtn.Location = new System.Drawing.Point(273, 386);
            this.createBinStackBtn.Name = "createBinStackBtn";
            this.createBinStackBtn.Size = new System.Drawing.Size(108, 29);
            this.createBinStackBtn.TabIndex = 2;
            this.createBinStackBtn.Text = "create binStack";
            this.createBinStackBtn.UseVisualStyleBackColor = true;
            this.createBinStackBtn.Click += new System.EventHandler(this.createBinStackBtn_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(19, 354);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(77, 20);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Status...";
            this.lblStatus.Visible = false;
            // 
            // frmMain
            // 
            this.AcceptButton = this.createBinStackBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 427);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.createBinStackBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "RawToBinStack v";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxQuiXML;
        private System.Windows.Forms.TextBox tbxSpecFolder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cbxSpecType;
        private System.Windows.Forms.Label lblSpecType;
        private System.Windows.Forms.ComboBox cbxPosition;
        private System.Windows.Forms.Button createBinStackBtn;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cbxUnits;
        private System.Windows.Forms.TextBox tbxParentalMZ;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxSchema;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.CheckBox chbAverageSpectra;
        private System.Windows.Forms.Label lblAverageMethod;
        private System.Windows.Forms.ComboBox cbxAverageMethod;
        private System.Windows.Forms.RadioButton rbnImportAll;
        private System.Windows.Forms.TextBox tbxWindowEnd;
        private System.Windows.Forms.TextBox tbxWindowStart;
        private System.Windows.Forms.RadioButton rbnImportWindowBetween;
        private System.Windows.Forms.RadioButton rbnImportParental;
    }
}

