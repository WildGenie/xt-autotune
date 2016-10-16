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
            System.Windows.Forms.TableLayoutPanel uiRows;
            System.Windows.Forms.FlowLayoutPanel uiFlow;
            this.uiColumns = new System.Windows.Forms.TableLayoutPanel();
            this.uiState = new System.Windows.Forms.Label();
            this.uiPlay = new System.Windows.Forms.LinkLabel();
            this.uiTitle = new System.Windows.Forms.Label();
            this.uiTooltip = new System.Windows.Forms.ToolTip(this.components);
            uiRows = new System.Windows.Forms.TableLayoutPanel();
            uiFlow = new System.Windows.Forms.FlowLayoutPanel();
            uiRows.SuspendLayout();
            this.uiColumns.SuspendLayout();
            uiFlow.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiRows
            // 
            uiRows.ColumnCount = 1;
            uiRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            uiRows.Controls.Add(this.uiColumns, 0, 1);
            uiRows.Controls.Add(this.uiTitle, 0, 0);
            uiRows.Dock = System.Windows.Forms.DockStyle.Fill;
            uiRows.Location = new System.Drawing.Point(3, 3);
            uiRows.Margin = new System.Windows.Forms.Padding(0);
            uiRows.Name = "uiRows";
            uiRows.RowCount = 2;
            uiRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            uiRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            uiRows.Size = new System.Drawing.Size(294, 42);
            uiRows.TabIndex = 1;
            // 
            // uiColumns
            // 
            this.uiColumns.ColumnCount = 2;
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.uiColumns.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67F));
            this.uiColumns.Controls.Add(this.uiState, 0, 0);
            this.uiColumns.Controls.Add(uiFlow, 1, 0);
            this.uiColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiColumns.Location = new System.Drawing.Point(0, 21);
            this.uiColumns.Margin = new System.Windows.Forms.Padding(0);
            this.uiColumns.Name = "uiColumns";
            this.uiColumns.RowCount = 1;
            this.uiColumns.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.uiColumns.Size = new System.Drawing.Size(294, 21);
            this.uiColumns.TabIndex = 0;
            // 
            // uiState
            // 
            this.uiState.AutoSize = true;
            this.uiState.Dock = System.Windows.Forms.DockStyle.Left;
            this.uiState.Location = new System.Drawing.Point(3, 0);
            this.uiState.Name = "uiState";
            this.uiState.Size = new System.Drawing.Size(45, 21);
            this.uiState.TabIndex = 0;
            this.uiState.Text = "Queued";
            // 
            // uiFlow
            // 
            uiFlow.Controls.Add(this.uiPlay);
            uiFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            uiFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            uiFlow.Location = new System.Drawing.Point(100, 3);
            uiFlow.Name = "uiFlow";
            uiFlow.Size = new System.Drawing.Size(191, 15);
            uiFlow.TabIndex = 1;
            uiFlow.WrapContents = false;
            // 
            // uiPlay
            // 
            this.uiPlay.AutoSize = true;
            this.uiPlay.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiPlay.Location = new System.Drawing.Point(161, 0);
            this.uiPlay.Name = "uiPlay";
            this.uiPlay.Size = new System.Drawing.Size(27, 13);
            this.uiPlay.TabIndex = 2;
            this.uiPlay.TabStop = true;
            this.uiPlay.Text = "Play";
            this.uiTooltip.SetToolTip(this.uiPlay, "Open this track in the media player.");
            this.uiPlay.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnPlayClicked);
            // 
            // uiTitle
            // 
            this.uiTitle.AutoSize = true;
            this.uiTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTitle.Location = new System.Drawing.Point(3, 0);
            this.uiTitle.Name = "uiTitle";
            this.uiTitle.Size = new System.Drawing.Size(288, 21);
            this.uiTitle.TabIndex = 1;
            this.uiTitle.Text = "Artist - Title";
            // 
            // QueueItemView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(uiRows);
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.Name = "QueueItemView";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(300, 48);
            uiRows.ResumeLayout(false);
            uiRows.PerformLayout();
            this.uiColumns.ResumeLayout(false);
            this.uiColumns.PerformLayout();
            uiFlow.ResumeLayout(false);
            uiFlow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel uiColumns;
        private System.Windows.Forms.Label uiState;
        private System.Windows.Forms.Label uiTitle;
        private System.Windows.Forms.LinkLabel uiPlay;
        private System.Windows.Forms.ToolTip uiTooltip;
    }
}
