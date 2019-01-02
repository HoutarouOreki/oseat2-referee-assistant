namespace oSEAT2RefereeAssistant
{
    partial class OSEATHandler
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OSEATHandler));
            this.StageSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ConfirmStage = new System.Windows.Forms.Button();
            this.MatchCodeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MapTableTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.StageMatchTableLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StageSelector
            // 
            this.StageSelector.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.StageSelector.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.StageSelector.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.StageSelector.BackColor = System.Drawing.SystemColors.Window;
            this.StageSelector.FormattingEnabled = true;
            this.StageSelector.Items.AddRange(new object[] {
            "Group Stage",
            "Round of 32",
            "Round of 16",
            "Quarter Finals",
            "Semifinals",
            "Finals",
            "Grand Finals"});
            this.StageSelector.Location = new System.Drawing.Point(12, 35);
            this.StageSelector.Name = "StageSelector";
            this.StageSelector.Size = new System.Drawing.Size(251, 21);
            this.StageSelector.TabIndex = 0;
            this.StageSelector.SelectedIndexChanged += new System.EventHandler(this.StageSelector_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(251, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select stage";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ConfirmStage
            // 
            this.ConfirmStage.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ConfirmStage.AutoSize = true;
            this.ConfirmStage.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.ConfirmStage.Enabled = false;
            this.ConfirmStage.FlatAppearance.BorderSize = 0;
            this.ConfirmStage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConfirmStage.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ConfirmStage.Location = new System.Drawing.Point(12, 268);
            this.ConfirmStage.Name = "ConfirmStage";
            this.ConfirmStage.Size = new System.Drawing.Size(251, 23);
            this.ConfirmStage.TabIndex = 6;
            this.ConfirmStage.Text = "Proceed";
            this.ConfirmStage.UseVisualStyleBackColor = false;
            this.ConfirmStage.Click += new System.EventHandler(this.ConfirmStage_Click);
            // 
            // MatchCodeTextBox
            // 
            this.MatchCodeTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.MatchCodeTextBox.Location = new System.Drawing.Point(12, 101);
            this.MatchCodeTextBox.Name = "MatchCodeTextBox";
            this.MatchCodeTextBox.Size = new System.Drawing.Size(251, 20);
            this.MatchCodeTextBox.TabIndex = 5;
            this.MatchCodeTextBox.TextChanged += new System.EventHandler(this.MatchCodeTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(251, 39);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select match code and both player usernames in the spreadsheet and copy-paste the" +
    "m here";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // MapTableTextBox
            // 
            this.MapTableTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.MapTableTextBox.Location = new System.Drawing.Point(12, 151);
            this.MapTableTextBox.Multiline = true;
            this.MapTableTextBox.Name = "MapTableTextBox";
            this.MapTableTextBox.Size = new System.Drawing.Size(251, 95);
            this.MapTableTextBox.TabIndex = 7;
            this.MapTableTextBox.TextChanged += new System.EventHandler(this.MapTableTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.Location = new System.Drawing.Point(12, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(251, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "Paste the beatmap table below";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // StageMatchTableLabel
            // 
            this.StageMatchTableLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.StageMatchTableLabel.ForeColor = System.Drawing.Color.Red;
            this.StageMatchTableLabel.Location = new System.Drawing.Point(12, 249);
            this.StageMatchTableLabel.Name = "StageMatchTableLabel";
            this.StageMatchTableLabel.Size = new System.Drawing.Size(251, 16);
            this.StageMatchTableLabel.TabIndex = 9;
            this.StageMatchTableLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(123, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 14);
            this.label4.TabIndex = 10;
            this.label4.Text = "171218";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // OSEATHandler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 303);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.StageMatchTableLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MapTableTextBox);
            this.Controls.Add(this.ConfirmStage);
            this.Controls.Add(this.MatchCodeTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StageSelector);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "OSEATHandler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "o!SEAT2 Assistant";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox StageSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ConfirmStage;
        private System.Windows.Forms.TextBox MatchCodeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MapTableTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label StageMatchTableLabel;
        private System.Windows.Forms.Label label4;
    }
}

