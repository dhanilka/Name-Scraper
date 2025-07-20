using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NameScraper
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer fadeTimer;
        private int fadeStep = 0;
        private const int FADE_STEPS = 20;

        public SplashForm()
        {
            InitializeComponent();
            SetupSplash();
        }

        private void SetupSplash()
        {
            // Form properties
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(600, 400);
            this.BackColor = Color.FromArgb(51, 122, 183);
            this.ShowInTaskbar = false;
            this.TopMost = true;

            // Create main panel
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(51, 122, 183)
            };

            // App icon/logo
            var iconLabel = new Label
            {
                Text = "??",
                Font = new Font("Segoe UI", 72F),
                ForeColor = Color.White,
                Size = new Size(150, 150),
                Location = new Point(225, 80),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // App title
            var titleLabel = new Label
            {
                Text = "Ultimate Name Scraper",
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(500, 40),
                Location = new Point(50, 200),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Subtitle
            var subtitleLabel = new Label
            {
                Text = "Find names across the entire internet",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.FromArgb(200, 220, 255),
                Size = new Size(500, 30),
                Location = new Point(50, 240),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Version info
            var versionLabel = new Label
            {
                Text = "Version 1.0 - Professional Edition",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(180, 200, 255),
                Size = new Size(300, 20),
                Location = new Point(150, 280),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Loading indicator
            var loadingLabel = new Label
            {
                Text = "Loading...",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.White,
                Size = new Size(200, 25),
                Location = new Point(200, 320),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Progress bar
            var progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 50,
                Size = new Size(300, 10),
                Location = new Point(150, 350),
                ForeColor = Color.White
            };

            // Add controls
            mainPanel.Controls.AddRange(new Control[] {
                iconLabel, titleLabel, subtitleLabel, versionLabel, loadingLabel, progressBar
            });

            this.Controls.Add(mainPanel);

            // Setup fade in effect
            this.Opacity = 0;
            fadeTimer = new System.Windows.Forms.Timer { Interval = 50 };
            fadeTimer.Tick += FadeTimer_Tick;
            fadeTimer.Start();

            // Auto close after 3 seconds
            Task.Delay(3000).ContinueWith(t => {
                if (!this.IsDisposed)
                {
                    this.Invoke(new Action(() => {
                        this.Hide();
                        var mainForm = new Form1();
                        mainForm.Show();
                    }));
                }
            });
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            fadeStep++;
            this.Opacity = (double)fadeStep / FADE_STEPS;
            
            if (fadeStep >= FADE_STEPS)
            {
                fadeTimer.Stop();
                fadeTimer.Dispose();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Allow user to skip splash with any key
            if (e.KeyCode != Keys.None)
            {
                this.Hide();
                var mainForm = new Form1();
                mainForm.Show();
            }
            base.OnKeyDown(e);
        }

        protected override void OnClick(EventArgs e)
        {
            // Allow user to skip splash with click
            this.Hide();
            var mainForm = new Form1();
            mainForm.Show();
            base.OnClick(e);
        }
    }

    partial class SplashForm
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
            this.Name = "SplashForm";
            this.ResumeLayout(false);
        }
    }
}