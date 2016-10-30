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
            this.uiDownload = new System.Windows.Forms.LinkLabel();
            this.uiRelated = new System.Windows.Forms.LinkLabel();
            this.uiImage = new System.Windows.Forms.PictureBox();
            this.uiText = new System.Windows.Forms.RichTextBox();
            this.uiType = new System.Windows.Forms.Label();
            this.uiTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.uiToggleFavourite = new System.Windows.Forms.LinkLabel();
            this.uiRemove = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.uiImage)).BeginInit();
            this.SuspendLayout();
            // 
            // uiDownload
            // 
            this.uiDownload.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiDownload.AutoSize = true;
            this.uiDownload.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiDownload.Location = new System.Drawing.Point(151, 16);
            this.uiDownload.Margin = new System.Windows.Forms.Padding(0);
            this.uiDownload.Name = "uiDownload";
            this.uiDownload.Size = new System.Drawing.Size(55, 13);
            this.uiDownload.TabIndex = 3;
            this.uiDownload.TabStop = true;
            this.uiDownload.Text = "Download";
            this.uiTooltip.SetToolTip(this.uiDownload, "Send this track to the download queue (right-click image).");
            this.uiDownload.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnDownloadClicked);
            // 
            // uiRelated
            // 
            this.uiRelated.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiRelated.AutoSize = true;
            this.uiRelated.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiRelated.Location = new System.Drawing.Point(107, 16);
            this.uiRelated.Margin = new System.Windows.Forms.Padding(0);
            this.uiRelated.Name = "uiRelated";
            this.uiRelated.Size = new System.Drawing.Size(44, 13);
            this.uiRelated.TabIndex = 4;
            this.uiRelated.TabStop = true;
            this.uiRelated.Text = "Related";
            this.uiTooltip.SetToolTip(this.uiRelated, "Search related tracks.");
            this.uiRelated.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRelatedClicked);
            // 
            // uiImage
            // 
            this.uiImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiImage.Location = new System.Drawing.Point(0, 0);
            this.uiImage.Margin = new System.Windows.Forms.Padding(0);
            this.uiImage.Name = "uiImage";
            this.uiImage.Size = new System.Drawing.Size(64, 64);
            this.uiImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.uiImage.TabIndex = 0;
            this.uiImage.TabStop = false;
            this.uiTooltip.SetToolTip(this.uiImage, "Click to queue, double-click to play.");
            this.uiImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnImageMouseDown);
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
            this.uiType.Size = new System.Drawing.Size(0, 13);
            this.uiType.TabIndex = 5;
            // 
            // uiToggleFavourite
            // 
            this.uiToggleFavourite.AutoSize = true;
            this.uiToggleFavourite.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiToggleFavourite.Location = new System.Drawing.Point(70, 16);
            this.uiToggleFavourite.Margin = new System.Windows.Forms.Padding(0);
            this.uiToggleFavourite.Name = "uiToggleFavourite";
            this.uiToggleFavourite.Size = new System.Drawing.Size(37, 13);
            this.uiToggleFavourite.TabIndex = 6;
            this.uiToggleFavourite.TabStop = true;
            this.uiToggleFavourite.Text = "Unlike";
            this.uiTooltip.SetToolTip(this.uiToggleFavourite, "Toggle track favourite status.");
            this.uiToggleFavourite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnToggleFavouriteClicked);
            // 
            // uiRemove
            // 
            this.uiRemove.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.uiRemove.AutoSize = true;
            this.uiRemove.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiRemove.Location = new System.Drawing.Point(323, 16);
            this.uiRemove.Margin = new System.Windows.Forms.Padding(0);
            this.uiRemove.Name = "uiRemove";
            this.uiRemove.Size = new System.Drawing.Size(47, 13);
            this.uiRemove.TabIndex = 8;
            this.uiRemove.TabStop = true;
            this.uiRemove.Text = "Remove";
            this.uiTooltip.SetToolTip(this.uiRemove, "Remove this track from the playlist.");
            this.uiRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnRemoveClicked);
            // 
            // ResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.uiRemove);
            this.Controls.Add(this.uiToggleFavourite);
            this.Controls.Add(this.uiType);
            this.Controls.Add(this.uiRelated);
            this.Controls.Add(this.uiDownload);
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
        private System.Windows.Forms.LinkLabel uiDownload;
        private System.Windows.Forms.LinkLabel uiRelated;
        private System.Windows.Forms.ToolTip uiTooltip;
        private System.Windows.Forms.LinkLabel uiToggleFavourite;
        private System.Windows.Forms.LinkLabel uiRemove;
    }
}
