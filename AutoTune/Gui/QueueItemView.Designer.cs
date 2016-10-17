namespace AutoTune.Gui {
    partial class QueueItemView {
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
            this.uiColumns = new System.Windows.Forms.TableLayoutPanel();
            this.uiPlay = new System.Windows.Forms.LinkLabel();
            this.uiState = new System.Windows.Forms.Label();
            this.uiTitle = new System.Windows.Forms.Label();
            this.uiImage = new System.Windows.Forms.PictureBox();
            this.uiTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.uiColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiImage)).BeginInit();
            this.SuspendLayout();
            // 
            // uiColumns
            // 
            this.uiColumns.ColumnCount = 4;
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.uiColumns.Controls.Add(this.uiPlay, 3, 0);
            this.uiColumns.Controls.Add(this.uiState, 2, 0);
            this.uiColumns.Controls.Add(this.uiTitle, 1, 0);
            this.uiColumns.Controls.Add(this.uiImage, 0, 0);
            this.uiColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiColumns.Location = new System.Drawing.Point(0, 0);
            this.uiColumns.Margin = new System.Windows.Forms.Padding(0);
            this.uiColumns.Name = "uiColumns";
            this.uiColumns.RowCount = 1;
            this.uiColumns.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiColumns.Size = new System.Drawing.Size(300, 24);
            this.uiColumns.TabIndex = 0;
            // 
            // uiPlay
            // 
            this.uiPlay.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiPlay.AutoSize = true;
            this.uiPlay.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiPlay.Location = new System.Drawing.Point(263, 5);
            this.uiPlay.Name = "uiPlay";
            this.uiPlay.Size = new System.Drawing.Size(27, 13);
            this.uiPlay.TabIndex = 2;
            this.uiPlay.TabStop = true;
            this.uiPlay.Text = "Play";
            this.uiTooltip.SetToolTip(this.uiPlay, "Open this track in the media player.");
            this.uiPlay.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnPlayClicked);
            // 
            // uiState
            // 
            this.uiState.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiState.AutoSize = true;
            this.uiState.Location = new System.Drawing.Point(203, 5);
            this.uiState.Name = "uiState";
            this.uiState.Size = new System.Drawing.Size(45, 13);
            this.uiState.TabIndex = 0;
            this.uiState.Text = "Queued";
            // 
            // uiTitle
            // 
            this.uiTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiTitle.AutoSize = true;
            this.uiTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTitle.Location = new System.Drawing.Point(27, 5);
            this.uiTitle.Name = "uiTitle";
            this.uiTitle.Size = new System.Drawing.Size(73, 13);
            this.uiTitle.TabIndex = 1;
            this.uiTitle.Text = "Artist - Title";
            // 
            // uiImage
            // 
            this.uiImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiImage.Location = new System.Drawing.Point(0, 0);
            this.uiImage.Margin = new System.Windows.Forms.Padding(0);
            this.uiImage.Name = "uiImage";
            this.uiImage.Size = new System.Drawing.Size(24, 24);
            this.uiImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.uiImage.TabIndex = 3;
            this.uiImage.TabStop = false;
            // 
            // QueueItemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uiColumns);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.Name = "QueueItemView";
            this.Size = new System.Drawing.Size(300, 24);
            this.uiColumns.ResumeLayout(false);
            this.uiColumns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel uiColumns;
        private System.Windows.Forms.Label uiState;
        private System.Windows.Forms.Label uiTitle;
        private System.Windows.Forms.LinkLabel uiPlay;
        private System.Windows.Forms.ToolTip uiTooltip;
        private System.Windows.Forms.PictureBox uiImage;
    }
}
