﻿using AutoTune.Processing;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class QueueItemView : UserControl {

        internal const string Done = "Done";
        internal const string Error = "Error";
        internal const string Queued = "Queued";
        internal const string Missing = "Missing";
        internal const string Started = "Started";

        internal QueueItem item;
        internal string State { get { return uiState.Text; } }

        public QueueItemView() {
            InitializeComponent();
            if (DesignMode)
                return;
            InitializeColors();
            uiState.Text = Queued;
        }

        internal void SetTitleWidth(int width) {
            uiTitle.Width = width;
        }

        internal void SetState(string state) {
            BeginInvoke(new Action(() => uiState.Text = state));
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            BackColor = ColorTranslator.FromHtml(theme.BackColor2);
            uiTitle.ForeColor = ColorTranslator.FromHtml(theme.ForeColor1);
            uiState.ForeColor = ColorTranslator.FromHtml(theme.ForeColor1);
        }

        internal void Initialize(QueueItem item) {
            this.item = item;
            uiTitle.Text = string.Format("({0}) {1}", item.Search.TypeId, item.Search.Title);
            uiImage.Image = UiUtility.ImageFromBase64(item?.Search?.ThumbnailBase64 ?? AppSettings.NoImageAvailableBase64);
        }
    }
}
