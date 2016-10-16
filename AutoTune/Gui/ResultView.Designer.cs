namespace AutoTune.Gui {
    partial class ResultView {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.uiPlay = new System.Windows.Forms.LinkLabel();
            this.uiDownload = new System.Windows.Forms.LinkLabel();
            this.uiSimilar = new System.Windows.Forms.LinkLabel();
            this.uiImage = new System.Windows.Forms.PictureBox();
            this.uiText = new System.Windows.Forms.RichTextBox();
            this.uiType = new System.Windows.Forms.Label();
            this.uiTooltip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.uiImage)).BeginInit();
            this.SuspendLayout();
            // 
            // uiPlay
            // 
            this.uiPlay.AutoSize = true;
            this.uiPlay.BackColor = System.Drawing.SystemColors.Window;
            this.uiPlay.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiPlay.Location = new System.Drawing.Point(67, 16);
            this.uiPlay.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.uiPlay.Name = "uiPlay";
            this.uiPlay.Size = new System.Drawing.Size(27, 13);
            this.uiPlay.TabIndex = 2;
            this.uiPlay.TabStop = true;
            this.uiPlay.Text = "Play";
            this.uiTooltip.SetToolTip(this.uiPlay, "Open this track in the media player.");
            this.uiPlay.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnPlayClicked);
            // 
            // uiDownload
            // 
            this.uiDownload.AutoSize = true;
            this.uiDownload.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiDownload.Location = new System.Drawing.Point(94, 16);
            this.uiDownload.Margin = new System.Windows.Forms.Padding(0);
            this.uiDownload.Name = "uiDownload";
            this.uiDownload.Size = new System.Drawing.Size(55, 13);
            this.uiDownload.TabIndex = 3;
            this.uiDownload.TabStop = true;
            this.uiDownload.Text = "Download";
            this.uiTooltip.SetToolTip(this.uiDownload, "Send this track to the download queue.");
            this.uiDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadClicked);
            // 
            // uiSimilar
            // 
            this.uiSimilar.AutoSize = true;
            this.uiSimilar.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiSimilar.Location = new System.Drawing.Point(149, 16);
            this.uiSimilar.Margin = new System.Windows.Forms.Padding(0);
            this.uiSimilar.Name = "uiSimilar";
            this.uiSimilar.Size = new System.Drawing.Size(37, 13);
            this.uiSimilar.TabIndex = 4;
            this.uiSimilar.TabStop = true;
            this.uiSimilar.Text = "Similar";
            this.uiTooltip.SetToolTip(this.uiSimilar, "Search similar tracks.");
            this.uiSimilar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnSimilarClicked);
            // 
            // uiImage
            // 
            this.uiImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiImage.Location = new System.Drawing.Point(0, 0);
            this.uiImage.Margin = new System.Windows.Forms.Padding(0);
            this.uiImage.Name = "uiImage";
            this.uiImage.Size = new System.Drawing.Size(64, 64);
            this.uiImage.TabIndex = 0;
            this.uiImage.TabStop = false;
            // 
            // uiText
            // 
            this.uiText.BackColor = System.Drawing.SystemColors.Window;
            this.uiText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uiText.DetectUrls = false;
            this.uiText.Location = new System.Drawing.Point(70, 29);
            this.uiText.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.uiText.Name = "uiText";
            this.uiText.ReadOnly = true;
            this.uiText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.uiText.Size = new System.Drawing.Size(300, 35);
            this.uiText.TabIndex = 1;
            this.uiText.Text = "";
            this.uiText.WordWrap = false;
            // 
            // uiType
            // 
            this.uiType.AutoSize = true;
            this.uiType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiType.Location = new System.Drawing.Point(67, 3);
            this.uiType.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.uiType.Name = "uiType";
            this.uiType.Size = new System.Drawing.Size(39, 13);
            this.uiType.TabIndex = 5;
            this.uiType.Text = "Spotify";
            // 
            // ResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.uiType);
            this.Controls.Add(this.uiSimilar);
            this.Controls.Add(this.uiDownload);
            this.Controls.Add(this.uiPlay);
            this.Controls.Add(this.uiText);
            this.Controls.Add(this.uiImage);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.Name = "ResultView";
            this.Size = new System.Drawing.Size(370, 64);
            ((System.ComponentModel.ISupportInitialize)(this.uiImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox uiImage;
        private System.Windows.Forms.RichTextBox uiText;
        private System.Windows.Forms.Label uiType;
        private System.Windows.Forms.LinkLabel uiPlay;
        private System.Windows.Forms.LinkLabel uiDownload;
        private System.Windows.Forms.LinkLabel uiSimilar;
        private System.Windows.Forms.ToolTip uiTooltip;
    }
}
