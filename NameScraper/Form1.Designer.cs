namespace NameScraper
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.newSearchToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.exportResultsToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator2 = new ToolStripSeparator();
            this.exitToolStripMenuItem = new ToolStripMenuItem();
            this.editToolStripMenuItem = new ToolStripMenuItem();
            this.clearResultsToolStripMenuItem = new ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new ToolStripMenuItem();
            this.viewToolStripMenuItem = new ToolStripMenuItem();
            this.statusBarToolStripMenuItem = new ToolStripMenuItem();
            this.toolsToolStripMenuItem = new ToolStripMenuItem();
            this.settingsToolStripMenuItem = new ToolStripMenuItem();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.userGuideToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripSeparator3 = new ToolStripSeparator();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            this.txtSearchName = new TextBox();
            this.btnSearch = new Button();
            this.btnClear = new Button();
            this.btnExport = new Button();
            this.progressBar = new ProgressBar();
            this.lblProgress = new Label();
            this.lblResultCount = new Label();
            this.listViewResults = new ListView();
            this.columnTitle = new ColumnHeader();
            this.columnUrl = new ColumnHeader();
            this.columnDescription = new ColumnHeader();
            this.columnSource = new ColumnHeader();
            this.columnDomain = new ColumnHeader();
            this.txtFilter = new TextBox();
            this.lblFilter = new Label();
            this.comboSearchEngine = new ComboBox();
            this.lblSearchEngine = new Label();
            this.panelHeader = new Panel();
            this.lblTitle = new Label();
            this.panelControls = new Panel();
            this.panelResults = new Panel();
            this.contextMenuResults = new ContextMenuStrip(this.components);
            this.openUrlMenuItem = new ToolStripMenuItem();
            this.copyUrlMenuItem = new ToolStripMenuItem();
            this.copyTitleMenuItem = new ToolStripMenuItem();
            this.saveResultMenuItem = new ToolStripMenuItem();
            this.statusStrip = new StatusStrip();
            this.statusLabel = new ToolStripStatusLabel();
            this.toolTip = new ToolTip(this.components);
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            
            this.menuStrip.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelResults.SuspendLayout();
            this.contextMenuResults.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
                this.fileToolStripMenuItem,
                this.editToolStripMenuItem,
                this.viewToolStripMenuItem,
                this.toolsToolStripMenuItem,
                this.helpToolStripMenuItem
            });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new Size(1200, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";

            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.newSearchToolStripMenuItem,
                this.toolStripSeparator1,
                this.exportResultsToolStripMenuItem,
                this.toolStripSeparator2,
                this.exitToolStripMenuItem
            });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";

            // 
            // newSearchToolStripMenuItem
            // 
            this.newSearchToolStripMenuItem.Name = "newSearchToolStripMenuItem";
            this.newSearchToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            this.newSearchToolStripMenuItem.Size = new Size(180, 22);
            this.newSearchToolStripMenuItem.Text = "&New Search";
            this.newSearchToolStripMenuItem.Click += new EventHandler(this.NewSearchToolStripMenuItem_Click);

            // 
            // exportResultsToolStripMenuItem
            // 
            this.exportResultsToolStripMenuItem.Name = "exportResultsToolStripMenuItem";
            this.exportResultsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
            this.exportResultsToolStripMenuItem.Size = new Size(180, 22);
            this.exportResultsToolStripMenuItem.Text = "&Export Results";
            this.exportResultsToolStripMenuItem.Click += new EventHandler(this.BtnExport_Click);

            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
            this.exitToolStripMenuItem.Size = new Size(180, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.ExitToolStripMenuItem_Click);

            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.clearResultsToolStripMenuItem,
                this.selectAllToolStripMenuItem
            });
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new Size(39, 20);
            this.editToolStripMenuItem.Text = "&Edit";

            // 
            // clearResultsToolStripMenuItem
            // 
            this.clearResultsToolStripMenuItem.Name = "clearResultsToolStripMenuItem";
            this.clearResultsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Delete;
            this.clearResultsToolStripMenuItem.Size = new Size(180, 22);
            this.clearResultsToolStripMenuItem.Text = "&Clear Results";
            this.clearResultsToolStripMenuItem.Click += new EventHandler(this.BtnClear_Click);

            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            this.selectAllToolStripMenuItem.Size = new Size(180, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new EventHandler(this.SelectAllToolStripMenuItem_Click);

            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.statusBarToolStripMenuItem
            });
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";

            // 
            // statusBarToolStripMenuItem
            // 
            this.statusBarToolStripMenuItem.Checked = true;
            this.statusBarToolStripMenuItem.CheckState = CheckState.Checked;
            this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
            this.statusBarToolStripMenuItem.Size = new Size(180, 22);
            this.statusBarToolStripMenuItem.Text = "&Status Bar";
            this.statusBarToolStripMenuItem.Click += new EventHandler(this.StatusBarToolStripMenuItem_Click);

            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.settingsToolStripMenuItem
            });
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new Size(46, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";

            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new Size(180, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new EventHandler(this.SettingsToolStripMenuItem_Click);

            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                this.userGuideToolStripMenuItem,
                this.toolStripSeparator3,
                this.aboutToolStripMenuItem
            });
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";

            // 
            // userGuideToolStripMenuItem
            // 
            this.userGuideToolStripMenuItem.Name = "userGuideToolStripMenuItem";
            this.userGuideToolStripMenuItem.ShortcutKeys = Keys.F1;
            this.userGuideToolStripMenuItem.Size = new Size(180, 22);
            this.userGuideToolStripMenuItem.Text = "&User Guide";
            this.userGuideToolStripMenuItem.Click += new EventHandler(this.UserGuideToolStripMenuItem_Click);

            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new Size(180, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new EventHandler(this.AboutToolStripMenuItem_Click);

            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(177, 6);

            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new Size(177, 6);

            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new Size(177, 6);

            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] {
                this.statusLabel
            });
            this.statusStrip.Location = new Point(0, 651);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(1200, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip";

            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new Size(39, 17);
            this.statusLabel.Text = "Ready";

            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = Color.FromArgb(51, 122, 183);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Location = new Point(0, 24);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new Size(1200, 80);
            this.panelHeader.TabIndex = 1;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(400, 45);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🔍 Ultimate Name Scraper";

            // 
            // panelControls
            // 
            this.panelControls.BackColor = Color.FromArgb(248, 249, 250);
            this.panelControls.Controls.Add(this.txtSearchName);
            this.panelControls.Controls.Add(this.btnSearch);
            this.panelControls.Controls.Add(this.btnClear);
            this.panelControls.Controls.Add(this.btnExport);
            this.panelControls.Controls.Add(this.progressBar);
            this.panelControls.Controls.Add(this.lblProgress);
            this.panelControls.Controls.Add(this.lblResultCount);
            this.panelControls.Controls.Add(this.txtFilter);
            this.panelControls.Controls.Add(this.lblFilter);
            this.panelControls.Controls.Add(this.comboSearchEngine);
            this.panelControls.Controls.Add(this.lblSearchEngine);
            this.panelControls.Dock = DockStyle.Top;
            this.panelControls.Location = new Point(0, 104);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new Padding(20);
            this.panelControls.Size = new Size(1200, 150);
            this.panelControls.TabIndex = 2;

            // 
            // txtSearchName
            // 
            this.txtSearchName.Font = new Font("Segoe UI", 14F);
            this.txtSearchName.Location = new Point(20, 20);
            this.txtSearchName.Name = "txtSearchName";
            this.txtSearchName.PlaceholderText = "Enter name to search...";
            this.txtSearchName.Size = new Size(300, 32);
            this.txtSearchName.TabIndex = 0;
            this.toolTip.SetToolTip(this.txtSearchName, "Enter the name you want to search across the internet");

            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = Color.FromArgb(40, 167, 69);
            this.btnSearch.FlatStyle = FlatStyle.Flat;
            this.btnSearch.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.btnSearch.ForeColor = Color.White;
            this.btnSearch.Location = new Point(340, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(100, 32);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "🚀 Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new EventHandler(this.BtnSearch_Click);

            // 
            // btnClear
            // 
            this.btnClear.BackColor = Color.FromArgb(220, 53, 69);
            this.btnClear.FlatStyle = FlatStyle.Flat;
            this.btnClear.Font = new Font("Segoe UI", 10F);
            this.btnClear.ForeColor = Color.White;
            this.btnClear.Location = new Point(460, 20);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new Size(80, 32);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "🗑️ Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new EventHandler(this.BtnClear_Click);

            // 
            // btnExport
            // 
            this.btnExport.BackColor = Color.FromArgb(23, 162, 184);
            this.btnExport.FlatStyle = FlatStyle.Flat;
            this.btnExport.Font = new Font("Segoe UI", 10F);
            this.btnExport.ForeColor = Color.White;
            this.btnExport.Location = new Point(560, 20);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new Size(80, 32);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "📄 Export";
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new EventHandler(this.BtnExport_Click);

            // 
            // comboSearchEngine
            // 
            this.comboSearchEngine.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboSearchEngine.Font = new Font("Segoe UI", 10F);
            this.comboSearchEngine.FormattingEnabled = true;
            this.comboSearchEngine.Items.AddRange(new object[] {
                "All Search Engines",
                "Google Only",
                "Bing Only",
                "DuckDuckGo Only",
                "Social Media Focus",
                "News Focus"
            });
            this.comboSearchEngine.Location = new Point(700, 25);
            this.comboSearchEngine.Name = "comboSearchEngine";
            this.comboSearchEngine.SelectedIndex = 0;
            this.comboSearchEngine.Size = new Size(150, 25);
            this.comboSearchEngine.TabIndex = 4;

            // 
            // lblSearchEngine
            // 
            this.lblSearchEngine.AutoSize = true;
            this.lblSearchEngine.Font = new Font("Segoe UI", 9F);
            this.lblSearchEngine.Location = new Point(700, 8);
            this.lblSearchEngine.Name = "lblSearchEngine";
            this.lblSearchEngine.Size = new Size(82, 15);
            this.lblSearchEngine.TabIndex = 5;
            this.lblSearchEngine.Text = "Search Engine:";

            // 
            // txtFilter
            // 
            this.txtFilter.Font = new Font("Segoe UI", 10F);
            this.txtFilter.Location = new Point(20, 80);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.PlaceholderText = "Filter results...";
            this.txtFilter.Size = new Size(200, 25);
            this.txtFilter.TabIndex = 6;
            this.txtFilter.TextChanged += new EventHandler(this.TxtFilter_TextChanged);

            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Font = new Font("Segoe UI", 9F);
            this.lblFilter.Location = new Point(20, 65);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new Size(74, 15);
            this.lblFilter.TabIndex = 7;
            this.lblFilter.Text = "Filter Results:";

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(240, 80);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new Size(400, 25);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 8;

            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Font = new Font("Segoe UI", 9F);
            this.lblProgress.Location = new Point(240, 115);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new Size(39, 15);
            this.lblProgress.TabIndex = 9;
            this.lblProgress.Text = "Ready";

            // 
            // lblResultCount
            // 
            this.lblResultCount.AutoSize = true;
            this.lblResultCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblResultCount.Location = new Point(660, 85);
            this.lblResultCount.Name = "lblResultCount";
            this.lblResultCount.Size = new Size(73, 19);
            this.lblResultCount.TabIndex = 10;
            this.lblResultCount.Text = "Results: 0";

            // 
            // panelResults
            // 
            this.panelResults.Controls.Add(this.listViewResults);
            this.panelResults.Dock = DockStyle.Fill;
            this.panelResults.Location = new Point(0, 254);
            this.panelResults.Name = "panelResults";
            this.panelResults.Padding = new Padding(20, 10, 20, 20);
            this.panelResults.Size = new Size(1200, 397);
            this.panelResults.TabIndex = 3;

            // 
            // listViewResults
            // 
            this.listViewResults.Columns.AddRange(new ColumnHeader[] {
                this.columnTitle,
                this.columnUrl,
                this.columnDescription,
                this.columnSource,
                this.columnDomain
            });
            this.listViewResults.ContextMenuStrip = this.contextMenuResults;
            this.listViewResults.Dock = DockStyle.Fill;
            this.listViewResults.Font = new Font("Segoe UI", 9F);
            this.listViewResults.FullRowSelect = true;
            this.listViewResults.GridLines = true;
            this.listViewResults.Location = new Point(20, 10);
            this.listViewResults.MultiSelect = false;
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.Size = new Size(1160, 367);
            this.listViewResults.TabIndex = 0;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = View.Details;
            this.listViewResults.DoubleClick += new EventHandler(this.ListViewResults_DoubleClick);

            // 
            // columnTitle
            // 
            this.columnTitle.Text = "Title";
            this.columnTitle.Width = 300;

            // 
            // columnUrl
            // 
            this.columnUrl.Text = "URL";
            this.columnUrl.Width = 350;

            // 
            // columnDescription
            // 
            this.columnDescription.Text = "Description";
            this.columnDescription.Width = 300;

            // 
            // columnSource
            // 
            this.columnSource.Text = "Source";
            this.columnSource.Width = 100;

            // 
            // columnDomain
            // 
            this.columnDomain.Text = "Domain";
            this.columnDomain.Width = 110;

            // 
            // contextMenuResults
            // 
            this.contextMenuResults.Items.AddRange(new ToolStripItem[] {
                this.openUrlMenuItem,
                this.copyUrlMenuItem,
                this.copyTitleMenuItem,
                this.saveResultMenuItem
            });
            this.contextMenuResults.Name = "contextMenuResults";
            this.contextMenuResults.Size = new Size(153, 92);

            // 
            // openUrlMenuItem
            // 
            this.openUrlMenuItem.Name = "openUrlMenuItem";
            this.openUrlMenuItem.Size = new Size(152, 22);
            this.openUrlMenuItem.Text = "🌐 Open URL";
            this.openUrlMenuItem.Click += new EventHandler(this.OpenUrlMenuItem_Click);

            // 
            // copyUrlMenuItem
            // 
            this.copyUrlMenuItem.Name = "copyUrlMenuItem";
            this.copyUrlMenuItem.Size = new Size(152, 22);
            this.copyUrlMenuItem.Text = "📋 Copy URL";
            this.copyUrlMenuItem.Click += new EventHandler(this.CopyUrlMenuItem_Click);

            // 
            // copyTitleMenuItem
            // 
            this.copyTitleMenuItem.Name = "copyTitleMenuItem";
            this.copyTitleMenuItem.Size = new Size(152, 22);
            this.copyTitleMenuItem.Text = "📝 Copy Title";
            this.copyTitleMenuItem.Click += new EventHandler(this.CopyTitleMenuItem_Click);

            // 
            // saveResultMenuItem
            // 
            this.saveResultMenuItem.Name = "saveResultMenuItem";
            this.saveResultMenuItem.Size = new Size(152, 22);
            this.saveResultMenuItem.Text = "💾 Save Result";
            this.saveResultMenuItem.Click += new EventHandler(this.SaveResultMenuItem_Click);

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(1200, 673);
            this.Controls.Add(this.panelResults);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = SystemIcons.Application;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new Size(800, 600);
            this.Name = "Form1";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Ultimate Name Scraper - Find Names Across the Internet";
            this.Load += new EventHandler(this.Form1_Load);

            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelResults.ResumeLayout(false);
            this.contextMenuResults.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newSearchToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exportResultsToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem clearResultsToolStripMenuItem;
        private ToolStripMenuItem selectAllToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem statusBarToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem userGuideToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private TextBox txtSearchName;
        private Button btnSearch;
        private Button btnClear;
        private Button btnExport;
        private ProgressBar progressBar;
        private Label lblProgress;
        private Label lblResultCount;
        private ListView listViewResults;
        private ColumnHeader columnTitle;
        private ColumnHeader columnUrl;
        private ColumnHeader columnDescription;
        private ColumnHeader columnSource;
        private ColumnHeader columnDomain;
        private TextBox txtFilter;
        private Label lblFilter;
        private ComboBox comboSearchEngine;
        private Label lblSearchEngine;
        private Panel panelHeader;
        private Label lblTitle;
        private Panel panelControls;
        private Panel panelResults;
        private ContextMenuStrip contextMenuResults;
        private ToolStripMenuItem openUrlMenuItem;
        private ToolStripMenuItem copyUrlMenuItem;
        private ToolStripMenuItem copyTitleMenuItem;
        private ToolStripMenuItem saveResultMenuItem;
        private ToolTip toolTip;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}
