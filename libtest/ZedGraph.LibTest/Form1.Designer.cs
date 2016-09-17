namespace ZedGraph.LibTest
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      this.panel1 = new System.Windows.Forms.Panel();
      this.CrossAutoBox = new System.Windows.Forms.CheckBox();
      this.AxisSelection = new System.Windows.Forms.ComboBox();
      this.LabelsInsideBox = new System.Windows.Forms.CheckBox();
      this.ReverseBox = new System.Windows.Forms.CheckBox();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.CrossAutoBox);
      this.panel1.Controls.Add(this.AxisSelection);
      this.panel1.Controls.Add(this.LabelsInsideBox);
      this.panel1.Controls.Add(this.ReverseBox);
      this.panel1.Controls.Add(this.trackBar1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(570, 65);
      this.panel1.TabIndex = 9;
      // 
      // CrossAutoBox
      // 
      this.CrossAutoBox.Location = new System.Drawing.Point(424, 12);
      this.CrossAutoBox.Name = "CrossAutoBox";
      this.CrossAutoBox.Size = new System.Drawing.Size(69, 17);
      this.CrossAutoBox.TabIndex = 15;
      this.CrossAutoBox.Text = "crossAuto";
      // 
      // AxisSelection
      // 
      this.AxisSelection.Items.AddRange(new object[] {
            "X Axis",
            "Y Axis",
            "Y2 Axis"});
      this.AxisSelection.Location = new System.Drawing.Point(425, 31);
      this.AxisSelection.Name = "AxisSelection";
      this.AxisSelection.Size = new System.Drawing.Size(121, 21);
      this.AxisSelection.TabIndex = 14;
      // 
      // LabelsInsideBox
      // 
      this.LabelsInsideBox.Location = new System.Drawing.Point(324, 35);
      this.LabelsInsideBox.Name = "LabelsInsideBox";
      this.LabelsInsideBox.Size = new System.Drawing.Size(84, 17);
      this.LabelsInsideBox.TabIndex = 13;
      this.LabelsInsideBox.Text = "Labels Inside";
      // 
      // ReverseBox
      // 
      this.ReverseBox.Location = new System.Drawing.Point(324, 12);
      this.ReverseBox.Name = "ReverseBox";
      this.ReverseBox.Size = new System.Drawing.Size(70, 17);
      this.ReverseBox.TabIndex = 12;
      this.ReverseBox.Text = "IsReverse";
      // 
      // trackBar1
      // 
      this.trackBar1.Location = new System.Drawing.Point(6, 12);
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(311, 45);
      this.trackBar1.TabIndex = 11;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(570, 438);
      this.Controls.Add(this.panel1);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
      this.Resize += new System.EventHandler(this.Form1_Resize);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      this.ResumeLayout(false);

		}

    #endregion
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.CheckBox CrossAutoBox;
    private System.Windows.Forms.ComboBox AxisSelection;
    private System.Windows.Forms.CheckBox LabelsInsideBox;
    private System.Windows.Forms.CheckBox ReverseBox;
    private System.Windows.Forms.TrackBar trackBar1;
  }
}

