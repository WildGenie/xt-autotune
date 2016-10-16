namespace AutoTune.Gui {
    partial class MainWindow {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.SplitContainer uiSplitPlayerToggleSearch;
            System.Windows.Forms.SplitContainer uiSplitPlayerToggleNotications;
            System.Windows.Forms.TableLayoutPanel uiNotificationsPanel;
            System.Windows.Forms.TableLayoutPanel uiLogContainer;
            System.Windows.Forms.FlowLayoutPanel uiLogLevelContainer;
            System.Windows.Forms.TableLayoutPanel uiSearchPanel;
            System.Windows.Forms.TableLayoutPanel uiResultsContinueContainer;
            System.Windows.Forms.Panel uiPadLog;
            System.Windows.Forms.TableLayoutPanel uiNotificationsCurrentContainer;
            this.uiToggleSearch = new System.Windows.Forms.LinkLabel();
            this.uiBrowserContainer = new System.Windows.Forms.Panel();
            this.uiToggleNotifications = new System.Windows.Forms.LinkLabel();
            this.uiDownloadGroup = new System.Windows.Forms.GroupBox();
            this.uiPostProcessingGroup = new System.Windows.Forms.GroupBox();
            this.uiToggleLog = new System.Windows.Forms.LinkLabel();
            this.uiLog = new System.Windows.Forms.TextBox();
            this.uiLogLevel = new System.Windows.Forms.ComboBox();
            this.uiLogLevelLabel = new System.Windows.Forms.Label();
            this.uiQuery = new System.Windows.Forms.TextBox();
            this.uiResults = new System.Windows.Forms.FlowLayoutPanel();
            this.uiLoadMore = new System.Windows.Forms.LinkLabel();
            this.uiLogGroup = new System.Windows.Forms.GroupBox();
            this.uiSplitNotificationsLog = new System.Windows.Forms.SplitContainer();
            this.uiCurrentGroup = new System.Windows.Forms.GroupBox();
            this.uiSplitNotifications = new System.Windows.Forms.SplitContainer();
            this.uiGroupSearch = new System.Windows.Forms.GroupBox();
            this.uiSplitSearch = new System.Windows.Forms.SplitContainer();
            this.uiDownloadQueue = new AutoTune.Gui.QueueView();
            this.uiPostProcessingQueue = new AutoTune.Gui.QueueView();
            this.uiCurrentResult = new AutoTune.Gui.ResultView();
            uiSplitPlayerToggleSearch = new System.Windows.Forms.SplitContainer();
            uiSplitPlayerToggleNotications = new System.Windows.Forms.SplitContainer();
            uiNotificationsPanel = new System.Windows.Forms.TableLayoutPanel();
            uiLogContainer = new System.Windows.Forms.TableLayoutPanel();
            uiLogLevelContainer = new System.Windows.Forms.FlowLayoutPanel();
            uiSearchPanel = new System.Windows.Forms.TableLayoutPanel();
            uiResultsContinueContainer = new System.Windows.Forms.TableLayoutPanel();
            uiPadLog = new System.Windows.Forms.Panel();
            uiNotificationsCurrentContainer = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(uiSplitPlayerToggleSearch)).BeginInit();
            uiSplitPlayerToggleSearch.Panel1.SuspendLayout();
            uiSplitPlayerToggleSearch.Panel2.SuspendLayout();
            uiSplitPlayerToggleSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(uiSplitPlayerToggleNotications)).BeginInit();
            uiSplitPlayerToggleNotications.Panel1.SuspendLayout();
            uiSplitPlayerToggleNotications.Panel2.SuspendLayout();
            uiSplitPlayerToggleNotications.SuspendLayout();
            uiNotificationsPanel.SuspendLayout();
            this.uiDownloadGroup.SuspendLayout();
            this.uiPostProcessingGroup.SuspendLayout();
            uiLogContainer.SuspendLayout();
            uiLogLevelContainer.SuspendLayout();
            uiSearchPanel.SuspendLayout();
            uiResultsContinueContainer.SuspendLayout();
            uiPadLog.SuspendLayout();
            this.uiLogGroup.SuspendLayout();
            uiNotificationsCurrentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitNotificationsLog)).BeginInit();
            this.uiSplitNotificationsLog.Panel1.SuspendLayout();
            this.uiSplitNotificationsLog.Panel2.SuspendLayout();
            this.uiSplitNotificationsLog.SuspendLayout();
            this.uiCurrentGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitNotifications)).BeginInit();
            this.uiSplitNotifications.Panel1.SuspendLayout();
            this.uiSplitNotifications.Panel2.SuspendLayout();
            this.uiSplitNotifications.SuspendLayout();
            this.uiGroupSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitSearch)).BeginInit();
            this.uiSplitSearch.Panel1.SuspendLayout();
            this.uiSplitSearch.Panel2.SuspendLayout();
            this.uiSplitSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiSplitPlayerToggleSearch
            // 
            uiSplitPlayerToggleSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            uiSplitPlayerToggleSearch.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            uiSplitPlayerToggleSearch.IsSplitterFixed = true;
            uiSplitPlayerToggleSearch.Location = new System.Drawing.Point(0, 0);
            uiSplitPlayerToggleSearch.Margin = new System.Windows.Forms.Padding(0);
            uiSplitPlayerToggleSearch.Name = "uiSplitPlayerToggleSearch";
            // 
            // uiSplitPlayerToggleSearch.Panel1
            // 
            uiSplitPlayerToggleSearch.Panel1.Controls.Add(this.uiToggleSearch);
            uiSplitPlayerToggleSearch.Panel1MinSize = 6;
            // 
            // uiSplitPlayerToggleSearch.Panel2
            // 
            uiSplitPlayerToggleSearch.Panel2.Controls.Add(uiSplitPlayerToggleNotications);
            uiSplitPlayerToggleSearch.Size = new System.Drawing.Size(565, 300);
            uiSplitPlayerToggleSearch.SplitterDistance = 25;
            uiSplitPlayerToggleSearch.TabIndex = 0;
            // 
            // uiToggleSearch
            // 
            this.uiToggleSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.uiToggleSearch.AutoSize = true;
            this.uiToggleSearch.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.uiToggleSearch.Location = new System.Drawing.Point(5, 177);
            this.uiToggleSearch.Margin = new System.Windows.Forms.Padding(0);
            this.uiToggleSearch.Name = "uiToggleSearch";
            this.uiToggleSearch.Size = new System.Drawing.Size(13, 13);
            this.uiToggleSearch.TabIndex = 2;
            this.uiToggleSearch.TabStop = true;
            this.uiToggleSearch.Text = "<";
            this.uiToggleSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uiToggleSearch.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ToggleSearchClicked);
            // 
            // uiSplitPlayerToggleNotications
            // 
            uiSplitPlayerToggleNotications.Dock = System.Windows.Forms.DockStyle.Fill;
            uiSplitPlayerToggleNotications.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            uiSplitPlayerToggleNotications.IsSplitterFixed = true;
            uiSplitPlayerToggleNotications.Location = new System.Drawing.Point(0, 0);
            uiSplitPlayerToggleNotications.Margin = new System.Windows.Forms.Padding(0);
            uiSplitPlayerToggleNotications.Name = "uiSplitPlayerToggleNotications";
            uiSplitPlayerToggleNotications.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // uiSplitPlayerToggleNotications.Panel1
            // 
            uiSplitPlayerToggleNotications.Panel1.Controls.Add(this.uiBrowserContainer);
            // 
            // uiSplitPlayerToggleNotications.Panel2
            // 
            uiSplitPlayerToggleNotications.Panel2.Controls.Add(this.uiToggleNotifications);
            uiSplitPlayerToggleNotications.Panel2MinSize = 0;
            uiSplitPlayerToggleNotications.Size = new System.Drawing.Size(536, 300);
            uiSplitPlayerToggleNotications.SplitterDistance = 271;
            uiSplitPlayerToggleNotications.TabIndex = 0;
            // 
            // uiBrowserContainer
            // 
            this.uiBrowserContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiBrowserContainer.Location = new System.Drawing.Point(0, 0);
            this.uiBrowserContainer.Margin = new System.Windows.Forms.Padding(0);
            this.uiBrowserContainer.Name = "uiBrowserContainer";
            this.uiBrowserContainer.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.uiBrowserContainer.Size = new System.Drawing.Size(536, 271);
            this.uiBrowserContainer.TabIndex = 5;
            // 
            // uiToggleNotifications
            // 
            this.uiToggleNotifications.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.uiToggleNotifications.AutoSize = true;
            this.uiToggleNotifications.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.uiToggleNotifications.Location = new System.Drawing.Point(263, -1);
            this.uiToggleNotifications.Margin = new System.Windows.Forms.Padding(0);
            this.uiToggleNotifications.Name = "uiToggleNotifications";
            this.uiToggleNotifications.Size = new System.Drawing.Size(13, 13);
            this.uiToggleNotifications.TabIndex = 0;
            this.uiToggleNotifications.TabStop = true;
            this.uiToggleNotifications.Text = "<";
            this.uiToggleNotifications.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ToggleNotificationsClicked);
            // 
            // uiNotificationsPanel
            // 
            uiNotificationsPanel.ColumnCount = 3;
            uiNotificationsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            uiNotificationsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            uiNotificationsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            uiNotificationsPanel.Controls.Add(this.uiDownloadGroup, 0, 0);
            uiNotificationsPanel.Controls.Add(this.uiPostProcessingGroup, 1, 0);
            uiNotificationsPanel.Controls.Add(this.uiToggleLog, 2, 0);
            uiNotificationsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            uiNotificationsPanel.Location = new System.Drawing.Point(0, 0);
            uiNotificationsPanel.Margin = new System.Windows.Forms.Padding(0);
            uiNotificationsPanel.Name = "uiNotificationsPanel";
            uiNotificationsPanel.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            uiNotificationsPanel.RowCount = 1;
            uiNotificationsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiNotificationsPanel.Size = new System.Drawing.Size(189, 156);
            uiNotificationsPanel.TabIndex = 0;
            // 
            // uiDownloadGroup
            // 
            this.uiDownloadGroup.AutoSize = true;
            this.uiDownloadGroup.Controls.Add(this.uiDownloadQueue);
            this.uiDownloadGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDownloadGroup.Location = new System.Drawing.Point(3, 3);
            this.uiDownloadGroup.Margin = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this.uiDownloadGroup.Name = "uiDownloadGroup";
            this.uiDownloadGroup.Padding = new System.Windows.Forms.Padding(5);
            this.uiDownloadGroup.Size = new System.Drawing.Size(72, 153);
            this.uiDownloadGroup.TabIndex = 0;
            this.uiDownloadGroup.TabStop = false;
            this.uiDownloadGroup.Text = "Downloads";
            // 
            // uiPostProcessingGroup
            // 
            this.uiPostProcessingGroup.Controls.Add(this.uiPostProcessingQueue);
            this.uiPostProcessingGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPostProcessingGroup.Location = new System.Drawing.Point(84, 3);
            this.uiPostProcessingGroup.Margin = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this.uiPostProcessingGroup.Name = "uiPostProcessingGroup";
            this.uiPostProcessingGroup.Padding = new System.Windows.Forms.Padding(5);
            this.uiPostProcessingGroup.Size = new System.Drawing.Size(72, 153);
            this.uiPostProcessingGroup.TabIndex = 1;
            this.uiPostProcessingGroup.TabStop = false;
            this.uiPostProcessingGroup.Text = "Post processing";
            // 
            // uiToggleLog
            // 
            this.uiToggleLog.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.uiToggleLog.AutoSize = true;
            this.uiToggleLog.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.uiToggleLog.Location = new System.Drawing.Point(170, 73);
            this.uiToggleLog.Name = "uiToggleLog";
            this.uiToggleLog.Size = new System.Drawing.Size(13, 13);
            this.uiToggleLog.TabIndex = 2;
            this.uiToggleLog.TabStop = true;
            this.uiToggleLog.Text = ">";
            this.uiToggleLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnToggleLogClicked);
            // 
            // uiLogContainer
            // 
            uiLogContainer.ColumnCount = 1;
            uiLogContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiLogContainer.Controls.Add(this.uiLog, 0, 1);
            uiLogContainer.Controls.Add(uiLogLevelContainer, 0, 0);
            uiLogContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            uiLogContainer.Location = new System.Drawing.Point(5, 18);
            uiLogContainer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            uiLogContainer.Name = "uiLogContainer";
            uiLogContainer.RowCount = 2;
            uiLogContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            uiLogContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            uiLogContainer.Size = new System.Drawing.Size(356, 130);
            uiLogContainer.TabIndex = 6;
            // 
            // uiLog
            // 
            this.uiLog.BackColor = System.Drawing.SystemColors.Window;
            this.uiLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLog.Location = new System.Drawing.Point(3, 33);
            this.uiLog.Multiline = true;
            this.uiLog.Name = "uiLog";
            this.uiLog.ReadOnly = true;
            this.uiLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.uiLog.Size = new System.Drawing.Size(350, 94);
            this.uiLog.TabIndex = 5;
            // 
            // uiLogLevelContainer
            // 
            uiLogLevelContainer.Controls.Add(this.uiLogLevel);
            uiLogLevelContainer.Controls.Add(this.uiLogLevelLabel);
            uiLogLevelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            uiLogLevelContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            uiLogLevelContainer.Location = new System.Drawing.Point(3, 3);
            uiLogLevelContainer.Name = "uiLogLevelContainer";
            uiLogLevelContainer.Size = new System.Drawing.Size(350, 24);
            uiLogLevelContainer.TabIndex = 6;
            uiLogLevelContainer.WrapContents = false;
            // 
            // uiLogLevel
            // 
            this.uiLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiLogLevel.FormattingEnabled = true;
            this.uiLogLevel.Location = new System.Drawing.Point(265, 3);
            this.uiLogLevel.Name = "uiLogLevel";
            this.uiLogLevel.Size = new System.Drawing.Size(82, 21);
            this.uiLogLevel.TabIndex = 1;
            this.uiLogLevel.SelectedIndexChanged += new System.EventHandler(this.OnLogLevelSelectionChanged);
            // 
            // uiLogLevelLabel
            // 
            this.uiLogLevelLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.uiLogLevelLabel.AutoSize = true;
            this.uiLogLevelLabel.Location = new System.Drawing.Point(180, 7);
            this.uiLogLevelLabel.Margin = new System.Windows.Forms.Padding(0);
            this.uiLogLevelLabel.Name = "uiLogLevelLabel";
            this.uiLogLevelLabel.Size = new System.Drawing.Size(82, 13);
            this.uiLogLevelLabel.TabIndex = 0;
            this.uiLogLevelLabel.Text = "Show output of:";
            // 
            // uiSearchPanel
            // 
            uiSearchPanel.ColumnCount = 1;
            uiSearchPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiSearchPanel.Controls.Add(this.uiQuery, 0, 0);
            uiSearchPanel.Controls.Add(uiResultsContinueContainer, 0, 1);
            uiSearchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            uiSearchPanel.Location = new System.Drawing.Point(3, 16);
            uiSearchPanel.Margin = new System.Windows.Forms.Padding(0);
            uiSearchPanel.Name = "uiSearchPanel";
            uiSearchPanel.RowCount = 2;
            uiSearchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            uiSearchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            uiSearchPanel.Size = new System.Drawing.Size(403, 536);
            uiSearchPanel.TabIndex = 0;
            // 
            // uiQuery
            // 
            this.uiQuery.BackColor = System.Drawing.SystemColors.Window;
            this.uiQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.uiQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiQuery.Location = new System.Drawing.Point(3, 3);
            this.uiQuery.Name = "uiQuery";
            this.uiQuery.Size = new System.Drawing.Size(397, 20);
            this.uiQuery.TabIndex = 1;
            this.uiQuery.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnQueryKeyPress);
            // 
            // uiResultsContinueContainer
            // 
            uiResultsContinueContainer.ColumnCount = 1;
            uiResultsContinueContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiResultsContinueContainer.Controls.Add(this.uiResults, 0, 0);
            uiResultsContinueContainer.Controls.Add(this.uiLoadMore, 0, 1);
            uiResultsContinueContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            uiResultsContinueContainer.Location = new System.Drawing.Point(3, 33);
            uiResultsContinueContainer.Name = "uiResultsContinueContainer";
            uiResultsContinueContainer.RowCount = 2;
            uiResultsContinueContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiResultsContinueContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            uiResultsContinueContainer.Size = new System.Drawing.Size(397, 500);
            uiResultsContinueContainer.TabIndex = 2;
            // 
            // uiResults
            // 
            this.uiResults.AutoScroll = true;
            this.uiResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiResults.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.uiResults.Location = new System.Drawing.Point(3, 3);
            this.uiResults.Name = "uiResults";
            this.uiResults.Size = new System.Drawing.Size(391, 469);
            this.uiResults.TabIndex = 2;
            this.uiResults.WrapContents = false;
            this.uiResults.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnUiResultsScroll);
            // 
            // uiLoadMore
            // 
            this.uiLoadMore.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.uiLoadMore.AutoSize = true;
            this.uiLoadMore.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.uiLoadMore.Location = new System.Drawing.Point(170, 481);
            this.uiLoadMore.Name = "uiLoadMore";
            this.uiLoadMore.Size = new System.Drawing.Size(57, 13);
            this.uiLoadMore.TabIndex = 3;
            this.uiLoadMore.TabStop = true;
            this.uiLoadMore.Text = "Load more";
            this.uiLoadMore.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLoadMoreClicked);
            // 
            // uiPadLog
            // 
            uiPadLog.Controls.Add(this.uiLogGroup);
            uiPadLog.Dock = System.Windows.Forms.DockStyle.Fill;
            uiPadLog.Location = new System.Drawing.Point(0, 0);
            uiPadLog.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            uiPadLog.Name = "uiPadLog";
            uiPadLog.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            uiPadLog.Size = new System.Drawing.Size(366, 156);
            uiPadLog.TabIndex = 3;
            // 
            // uiLogGroup
            // 
            this.uiLogGroup.Controls.Add(uiLogContainer);
            this.uiLogGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLogGroup.Location = new System.Drawing.Point(0, 3);
            this.uiLogGroup.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.uiLogGroup.Name = "uiLogGroup";
            this.uiLogGroup.Padding = new System.Windows.Forms.Padding(5);
            this.uiLogGroup.Size = new System.Drawing.Size(366, 153);
            this.uiLogGroup.TabIndex = 2;
            this.uiLogGroup.TabStop = false;
            this.uiLogGroup.Text = "Log";
            // 
            // uiNotificationsCurrentContainer
            // 
            uiNotificationsCurrentContainer.ColumnCount = 1;
            uiNotificationsCurrentContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiNotificationsCurrentContainer.Controls.Add(this.uiSplitNotificationsLog, 0, 1);
            uiNotificationsCurrentContainer.Controls.Add(this.uiCurrentGroup, 0, 0);
            uiNotificationsCurrentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            uiNotificationsCurrentContainer.Location = new System.Drawing.Point(0, 0);
            uiNotificationsCurrentContainer.Name = "uiNotificationsCurrentContainer";
            uiNotificationsCurrentContainer.RowCount = 2;
            uiNotificationsCurrentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            uiNotificationsCurrentContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            uiNotificationsCurrentContainer.Size = new System.Drawing.Size(565, 257);
            uiNotificationsCurrentContainer.TabIndex = 0;
            // 
            // uiSplitNotificationsLog
            // 
            this.uiSplitNotificationsLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitNotificationsLog.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.uiSplitNotificationsLog.IsSplitterFixed = true;
            this.uiSplitNotificationsLog.Location = new System.Drawing.Point(3, 98);
            this.uiSplitNotificationsLog.Name = "uiSplitNotificationsLog";
            // 
            // uiSplitNotificationsLog.Panel1
            // 
            this.uiSplitNotificationsLog.Panel1.Controls.Add(uiNotificationsPanel);
            // 
            // uiSplitNotificationsLog.Panel2
            // 
            this.uiSplitNotificationsLog.Panel2.Controls.Add(uiPadLog);
            this.uiSplitNotificationsLog.Size = new System.Drawing.Size(559, 156);
            this.uiSplitNotificationsLog.SplitterDistance = 189;
            this.uiSplitNotificationsLog.TabIndex = 1;
            // 
            // uiCurrentGroup
            // 
            this.uiCurrentGroup.Controls.Add(this.uiCurrentResult);
            this.uiCurrentGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiCurrentGroup.Location = new System.Drawing.Point(6, 3);
            this.uiCurrentGroup.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
            this.uiCurrentGroup.Name = "uiCurrentGroup";
            this.uiCurrentGroup.Padding = new System.Windows.Forms.Padding(3, 3, 3, 8);
            this.uiCurrentGroup.Size = new System.Drawing.Size(556, 89);
            this.uiCurrentGroup.TabIndex = 6;
            this.uiCurrentGroup.TabStop = false;
            this.uiCurrentGroup.Text = "Current track";
            // 
            // uiSplitNotifications
            // 
            this.uiSplitNotifications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitNotifications.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.uiSplitNotifications.IsSplitterFixed = true;
            this.uiSplitNotifications.Location = new System.Drawing.Point(0, 0);
            this.uiSplitNotifications.Margin = new System.Windows.Forms.Padding(0);
            this.uiSplitNotifications.Name = "uiSplitNotifications";
            this.uiSplitNotifications.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // uiSplitNotifications.Panel1
            // 
            this.uiSplitNotifications.Panel1.Controls.Add(uiSplitPlayerToggleSearch);
            // 
            // uiSplitNotifications.Panel2
            // 
            this.uiSplitNotifications.Panel2.Controls.Add(uiNotificationsCurrentContainer);
            this.uiSplitNotifications.Size = new System.Drawing.Size(565, 561);
            this.uiSplitNotifications.SplitterDistance = 300;
            this.uiSplitNotifications.TabIndex = 0;
            // 
            // uiGroupSearch
            // 
            this.uiGroupSearch.Controls.Add(uiSearchPanel);
            this.uiGroupSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiGroupSearch.Location = new System.Drawing.Point(3, 3);
            this.uiGroupSearch.Margin = new System.Windows.Forms.Padding(0);
            this.uiGroupSearch.Name = "uiGroupSearch";
            this.uiGroupSearch.Size = new System.Drawing.Size(409, 555);
            this.uiGroupSearch.TabIndex = 0;
            this.uiGroupSearch.TabStop = false;
            this.uiGroupSearch.Text = "Search";
            // 
            // uiSplitSearch
            // 
            this.uiSplitSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplitSearch.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.uiSplitSearch.IsSplitterFixed = true;
            this.uiSplitSearch.Location = new System.Drawing.Point(0, 0);
            this.uiSplitSearch.Margin = new System.Windows.Forms.Padding(0);
            this.uiSplitSearch.Name = "uiSplitSearch";
            // 
            // uiSplitSearch.Panel1
            // 
            this.uiSplitSearch.Panel1.Controls.Add(this.uiGroupSearch);
            this.uiSplitSearch.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // uiSplitSearch.Panel2
            // 
            this.uiSplitSearch.Panel2.Controls.Add(this.uiSplitNotifications);
            this.uiSplitSearch.Size = new System.Drawing.Size(984, 561);
            this.uiSplitSearch.SplitterDistance = 415;
            this.uiSplitSearch.TabIndex = 6;
            // 
            // uiDownloadQueue
            // 
            this.uiDownloadQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiDownloadQueue.Location = new System.Drawing.Point(5, 18);
            this.uiDownloadQueue.MinimumSize = new System.Drawing.Size(372, 100);
            this.uiDownloadQueue.Name = "uiDownloadQueue";
            this.uiDownloadQueue.Size = new System.Drawing.Size(372, 130);
            this.uiDownloadQueue.TabIndex = 0;
            // 
            // uiPostProcessingQueue
            // 
            this.uiPostProcessingQueue.AutoScroll = true;
            this.uiPostProcessingQueue.AutoSize = true;
            this.uiPostProcessingQueue.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.uiPostProcessingQueue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiPostProcessingQueue.Location = new System.Drawing.Point(5, 18);
            this.uiPostProcessingQueue.MinimumSize = new System.Drawing.Size(372, 100);
            this.uiPostProcessingQueue.Name = "uiPostProcessingQueue";
            this.uiPostProcessingQueue.Size = new System.Drawing.Size(372, 130);
            this.uiPostProcessingQueue.TabIndex = 0;
            // 
            // uiCurrentResult
            // 
            this.uiCurrentResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.uiCurrentResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiCurrentResult.Location = new System.Drawing.Point(3, 16);
            this.uiCurrentResult.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.uiCurrentResult.Name = "uiCurrentResult";
            this.uiCurrentResult.Size = new System.Drawing.Size(550, 65);
            this.uiCurrentResult.TabIndex = 0;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561);
            this.Controls.Add(this.uiSplitSearch);
            this.Name = "MainWindow";
            this.Text = "XT-AutoTune";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnMainWindowClosed);
            this.Shown += new System.EventHandler(this.OnMainWindowShown);
            uiSplitPlayerToggleSearch.Panel1.ResumeLayout(false);
            uiSplitPlayerToggleSearch.Panel1.PerformLayout();
            uiSplitPlayerToggleSearch.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(uiSplitPlayerToggleSearch)).EndInit();
            uiSplitPlayerToggleSearch.ResumeLayout(false);
            uiSplitPlayerToggleNotications.Panel1.ResumeLayout(false);
            uiSplitPlayerToggleNotications.Panel2.ResumeLayout(false);
            uiSplitPlayerToggleNotications.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(uiSplitPlayerToggleNotications)).EndInit();
            uiSplitPlayerToggleNotications.ResumeLayout(false);
            uiNotificationsPanel.ResumeLayout(false);
            uiNotificationsPanel.PerformLayout();
            this.uiDownloadGroup.ResumeLayout(false);
            this.uiPostProcessingGroup.ResumeLayout(false);
            this.uiPostProcessingGroup.PerformLayout();
            uiLogContainer.ResumeLayout(false);
            uiLogContainer.PerformLayout();
            uiLogLevelContainer.ResumeLayout(false);
            uiLogLevelContainer.PerformLayout();
            uiSearchPanel.ResumeLayout(false);
            uiSearchPanel.PerformLayout();
            uiResultsContinueContainer.ResumeLayout(false);
            uiResultsContinueContainer.PerformLayout();
            uiPadLog.ResumeLayout(false);
            this.uiLogGroup.ResumeLayout(false);
            uiNotificationsCurrentContainer.ResumeLayout(false);
            this.uiSplitNotificationsLog.Panel1.ResumeLayout(false);
            this.uiSplitNotificationsLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitNotificationsLog)).EndInit();
            this.uiSplitNotificationsLog.ResumeLayout(false);
            this.uiCurrentGroup.ResumeLayout(false);
            this.uiSplitNotifications.Panel1.ResumeLayout(false);
            this.uiSplitNotifications.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitNotifications)).EndInit();
            this.uiSplitNotifications.ResumeLayout(false);
            this.uiGroupSearch.ResumeLayout(false);
            this.uiSplitSearch.Panel1.ResumeLayout(false);
            this.uiSplitSearch.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplitSearch)).EndInit();
            this.uiSplitSearch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox uiQuery;
        private System.Windows.Forms.Panel uiBrowserContainer;
        private System.Windows.Forms.FlowLayoutPanel uiResults;
        private System.Windows.Forms.SplitContainer uiSplitSearch;
        private System.Windows.Forms.LinkLabel uiToggleSearch;
        private System.Windows.Forms.LinkLabel uiToggleNotifications;
        private System.Windows.Forms.SplitContainer uiSplitNotifications;
        private System.Windows.Forms.GroupBox uiDownloadGroup;
        private System.Windows.Forms.GroupBox uiPostProcessingGroup;
        private System.Windows.Forms.GroupBox uiLogGroup;
        private System.Windows.Forms.GroupBox uiGroupSearch;
        private System.Windows.Forms.TextBox uiLog;
        private QueueView uiDownloadQueue;
        private System.Windows.Forms.Label uiLogLevelLabel;
        private System.Windows.Forms.ComboBox uiLogLevel;
        private System.Windows.Forms.SplitContainer uiSplitNotificationsLog;
        private System.Windows.Forms.LinkLabel uiToggleLog;
        private QueueView uiPostProcessingQueue;
        private System.Windows.Forms.LinkLabel uiLoadMore;
        private System.Windows.Forms.GroupBox uiCurrentGroup;
        private ResultView uiCurrentResult;
    }
}

