using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace NameScraper
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            SetupAboutForm();
        }

        private void SetupAboutForm()
        {
            // Form properties
            this.Size = new Size(500, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "About Ultimate Name Scraper";
            this.BackColor = Color.White;

            // Header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(51, 122, 183)
            };

            var iconLabel = new Label
            {
                Text = "??",
                Font = new Font("Segoe UI", 32F),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var titleLabel = new Label
            {
                Text = "Ultimate Name Scraper",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(90, 25),
                Size = new Size(350, 30)
            };

            headerPanel.Controls.AddRange(new Control[] { iconLabel, titleLabel });

            // Content panel
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            var descriptionLabel = new Label
            {
                Text = "A powerful application to search for names across multiple search engines and platforms on the internet.",
                Font = new Font("Segoe UI", 11F),
                Location = new Point(20, 20),
                Size = new Size(440, 40),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var featuresLabel = new Label
            {
                Text = "Features:\n" +
                       "• Multi-engine search (Google, Bing, DuckDuckGo, Yahoo)\n" +
                       "• Social media platform scanning\n" +
                       "• News and forum search capabilities\n" +
                       "• Advanced filtering and sorting\n" +
                       "• Export results to CSV, JSON, or TXT\n" +
                       "• Real-time search progress tracking\n" +
                       "• Professional user interface",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 80),
                Size = new Size(440, 140),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            var versionLabel = new Label
            {
                Text = "Version: 1.0.0 Professional Edition\n" +
                       "Built with: .NET 8.0 & Windows Forms\n" +
                       "Created: " + DateTime.Now.Year,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(20, 240),
                Size = new Size(440, 60),
                ForeColor = Color.FromArgb(128, 128, 128)
            };

            var closeButton = new Button
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10F),
                Size = new Size(80, 32),
                Location = new Point(380, 260),
                BackColor = Color.FromArgb(51, 122, 183),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false
            };

            closeButton.Click += (s, e) => this.Close();

            contentPanel.Controls.AddRange(new Control[] {
                descriptionLabel, featuresLabel, versionLabel, closeButton
            });

            this.Controls.AddRange(new Control[] { headerPanel, contentPanel });
        }
    }

    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Name = "AboutForm";
            this.ResumeLayout(false);
        }
    }
}