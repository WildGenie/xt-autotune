namespace AutoTune.Gui {
    partial class QueueView  {
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
            this.uiControls = new System.Windows.Forms.FlowLayoutPanel();
            this.uiClearQueue = new System.Windows.Forms.LinkLabel();
            this.uiHideFinished = new System.Windows.Forms.LinkLabel();
            this.uiHideCompleted = new System.Windows.Forms.LinkLabel();
            this.uiPause = new System.Windows.Forms.LinkLabel();
            this.uiItems = new System.Windows.Forms.FlowLayoutPanel();
            this.uiTooltip = new System.Windows.Forms.ToolTip(this.components);
            uiRows = new System.Windows.Forms.TableLayoutPanel();
            uiRows.SuspendLayout();
            this.uiControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiRows
            // 
            uiRows.ColumnCount = 1;
            uiRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiRows.Controls.Add(this.uiControls, 0, 0);
            uiRows.Controls.Add(this.uiItems, 0, 1);
            uiRows.Dock = System.Windows.Forms.DockStyle.Fill;
            uiRows.Location = new System.Drawing.Point(0, 0);
            uiRows.Margin = new System.Windows.Forms.Padding(0);
            uiRows.Name = "uiRows";
            uiRows.RowCount = 2;
            uiRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            uiRows.RowStyles.Add(new System.Windows.Forms.RowStyle());
            uiRows.Size = new System.Drawing.Size(372, 100);
            uiRows.TabIndex = 0;
            // 
            // uiControls
            // 
            this.uiControls.Controls.Add(this.uiClearQueue);
            this.uiControls.Controls.Add(this.uiHideFinished);
            this.uiControls.Controls.Add(this.uiHideCompleted);
            this.uiControls.Controls.Add(this.uiPause);
            this.uiControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiControls.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.uiControls.Location = new System.Drawing.Point(3, 3);
            this.uiControls.Name = "uiControls";
            this.uiControls.Size = new System.Drawing.Size(366, 19);
            this.uiControls.TabIndex = 1;
            this.uiControls.WrapContents = false;
            // 
            // uiClearQueue
            // 
            this.uiClearQueue.AutoSize = true;
            this.uiClearQueue.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiClearQueue.Location = new System.Drawing.Point(299, 0);
            this.uiClearQueue.Name = "uiClearQueue";
            this.uiClearQueue.Size = new System.Drawing.Size(64, 13);
            this.uiClearQueue.TabIndex = 1;
            this.uiClearQueue.TabStop = true;
            this.uiClearQueue.Text = "Clear queue";
            this.uiTooltip.SetToolTip(this.uiClearQueue, "Empty the queue. Currently queued tracks will not be processed.");
            this.uiClearQueue.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnClearQueueClicked);
            // 
            // uiHideFinished
            // 
            this.uiHideFinished.AutoSize = true;
            this.uiHideFinished.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiHideFinished.Location = new System.Drawing.Point(225, 0);
            this.uiHideFinished.Name = "uiHideFinished";
            this.uiHideFinished.Size = new System.Drawing.Size(68, 13);
            this.uiHideFinished.TabIndex = 3;
            this.uiHideFinished.TabStop = true;
            this.uiHideFinished.Text = "Hide finished";
            this.uiTooltip.SetToolTip(this.uiHideFinished, "Hide all completed tracks from the queue.");
            this.uiHideFinished.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnHideFinishedClick);
            // 
            // uiHideCompleted
            // 
            this.uiHideCompleted.AutoSize = true;
            this.uiHideCompleted.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiHideCompleted.Location = new System.Drawing.Point(138, 0);
            this.uiHideCompleted.Name = "uiHideCompleted";
            this.uiHideCompleted.Size = new System.Drawing.Size(81, 13);
            this.uiHideCompleted.TabIndex = 4;
            this.uiHideCompleted.TabStop = true;
            this.uiHideCompleted.Text = "Hide completed";
            this.uiTooltip.SetToolTip(this.uiHideCompleted, "Hide successfully completed tracks from the queue.");
            this.uiHideCompleted.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnHideCompletedClick);
            // 
            // uiPause
            // 
            this.uiPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.uiPause.AutoSize = true;
            this.uiPause.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiPause.Location = new System.Drawing.Point(95, 0);
            this.uiPause.Name = "uiPause";
            this.uiPause.Size = new System.Drawing.Size(37, 13);
            this.uiPause.TabIndex = 2;
            this.uiPause.TabStop = true;
            this.uiPause.Text = "Pause";
            this.uiTooltip.SetToolTip(this.uiPause, "Pause or resume the queue. All processing will be suspended when paused.");
            this.uiPause.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnPauseClicked);
            // 
            // uiItems
            // 
            this.uiItems.AutoScroll = true;
            this.uiItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.uiItems.Location = new System.Drawing.Point(3, 28);
            this.uiItems.Name = "uiItems";
            this.uiItems.Size = new System.Drawing.Size(366, 69);
            this.uiItems.TabIndex = 2;
            this.uiItems.WrapContents = false;
            // 
            // QueueView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(uiRows);
            this.MinimumSize = new System.Drawing.Size(372, 100);
            this.Name = "QueueView";
            this.Size = new System.Drawing.Size(372, 100);
            this.Resize += new System.EventHandler(this.OnResized);
            uiRows.ResumeLayout(false);
            this.uiControls.ResumeLayout(false);
            this.uiControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.LinkLabel uiClearQueue;
        private System.Windows.Forms.FlowLayoutPanel uiControls;
        private System.Windows.Forms.LinkLabel uiPause;
        private System.Windows.Forms.FlowLayoutPanel uiItems;
        private System.Windows.Forms.LinkLabel uiHideFinished;
        private System.Windows.Forms.LinkLabel uiHideCompleted;
        private System.Windows.Forms.ToolTip uiTooltip;
    }
}
