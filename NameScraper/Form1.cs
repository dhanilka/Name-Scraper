using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace NameScraper
{
    public partial class Form1 : Form
    {
        private EnhancedNameSearchService _enhancedSearchService;
        private NameSearchService _basicSearchService;
        private List<SearchResult> _allResults;
        private List<SearchResult> _filteredResults;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isSearching;
        private bool _useEnhancedSearch = true;

        public Form1()
        {
            InitializeComponent();
            _enhancedSearchService = new EnhancedNameSearchService();
            _basicSearchService = new NameSearchService();
            _allResults = new List<SearchResult>();
            _filteredResults = new List<SearchResult>();
            
            // Subscribe to search progress events
            _enhancedSearchService.ProgressChanged += OnSearchProgressChanged;
            _basicSearchService.ProgressChanged += OnSearchProgressChanged;
            
            // Set up form
            SetupForm();
        }

        private void SetupForm()
        {
            // Enable double buffering for smooth UI
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            
            // Set up tooltips
            toolTip.SetToolTip(btnSearch, "Start searching for the entered name across multiple search engines and platforms");
            toolTip.SetToolTip(btnClear, "Clear all search results");
            toolTip.SetToolTip(btnExport, "Export search results to CSV, JSON, or TXT file");
            toolTip.SetToolTip(txtFilter, "Filter results by typing keywords");
            toolTip.SetToolTip(comboSearchEngine, "Choose search mode: Enhanced (recommended) uses advanced techniques and more sources");
            
            // Update combo box options
            comboSearchEngine.Items.Clear();
            comboSearchEngine.Items.AddRange(new object[] {
                "Enhanced Search (Recommended)",
                "Basic Search",
                "Social Media Focus",
                "Professional Focus", 
                "News Focus",
                "Academic Focus"
            });
            comboSearchEngine.SelectedIndex = 0;
            
            // Set up enter key handling for search
            txtSearchName.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter && !_isSearching)
                {
                    BtnSearch_Click(s, e);
                }
            };
            
            // Set initial states
            btnExport.Enabled = false;
            progressBar.Visible = false;
            lblProgress.Text = "Ready to search";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Focus on the search textbox
            txtSearchName.Focus();
            
            // Show welcome message
            lblProgress.Text = "?? Enter a name and click Search to find mentions across the internet";
            statusLabel.Text = "Ready - Enhanced search engine ready with 25+ sources";
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchName.Text))
            {
                MessageBox.Show("Please enter a name to search for.", "No Name Entered", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchName.Focus();
                return;
            }

            if (_isSearching)
            {
                // Cancel current search
                _cancellationTokenSource?.Cancel();
                return;
            }

            await StartSearchAsync();
        }

        private async Task StartSearchAsync()
        {
            try
            {
                _isSearching = true;
                _cancellationTokenSource = new CancellationTokenSource();
                
                // Determine search mode
                var searchMode = comboSearchEngine.SelectedIndex;
                _useEnhancedSearch = searchMode == 0 || searchMode >= 2; // Enhanced for most modes
                
                // Update UI for search state
                btnSearch.Text = "?? Cancel";
                btnSearch.BackColor = Color.FromArgb(220, 53, 69);
                btnClear.Enabled = false;
                btnExport.Enabled = false;
                progressBar.Visible = true;
                progressBar.Value = 0;
                
                // Clear previous results
                _allResults.Clear();
                _filteredResults.Clear();
                listViewResults.Items.Clear();
                UpdateResultCount();
                
                var searchName = txtSearchName.Text.Trim();
                var searchService = _useEnhancedSearch ? "Enhanced Search Engine" : "Basic Search Engine";
                lblProgress.Text = $"?? Searching for '{searchName}' using {searchService}...";
                statusLabel.Text = $"Searching for '{searchName}' across multiple platforms...";
                
                // Perform search based on selected mode
                List<SearchResult> results;
                
                if (_useEnhancedSearch)
                {
                    results = await _enhancedSearchService.SearchNameAsync(searchName, _cancellationTokenSource.Token);
                }
                else
                {
                    results = await _basicSearchService.SearchNameAsync(searchName, _cancellationTokenSource.Token);
                }
                
                // Apply search mode filtering if needed
                results = ApplySearchModeFilter(results, searchMode);
                
                // Update results
                _allResults = results;
                ApplyFilter();
                
                var searchEngineUsed = _useEnhancedSearch ? "Enhanced" : "Basic";
                lblProgress.Text = $"? {searchEngineUsed} search completed! Found {_allResults.Count} results from {GetUniqueDomainsCount()} different sources.";
                statusLabel.Text = $"Search completed - {_allResults.Count} results from {GetUniqueDomainsCount()} domains";
                
                if (_allResults.Count == 0)
                {
                    MessageBox.Show($"No results found for '{searchName}'. Try:\n\n" +
                        "• Different spelling or name variations\n" +
                        "• Just first name or last name\n" +
                        "• Adding middle initial\n" +
                        "• Using Enhanced Search mode\n" +
                        "• Checking for typos", 
                        "No Results Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (_allResults.Count > 100)
                {
                    MessageBox.Show($"Found {_allResults.Count} results! This is a comprehensive search.\n\n" +
                        "Tips:\n" +
                        "• Use the filter box to narrow results\n" +
                        "• Sort by clicking column headers\n" +
                        "• Export results for detailed analysis\n" +
                        "• Right-click results for more options", 
                        "Extensive Results Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (OperationCanceledException)
            {
                lblProgress.Text = "?? Search cancelled by user";
                statusLabel.Text = "Search cancelled";
                progressBar.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during search:\n\n{ex.Message}\n\n" +
                    "This might be due to:\n" +
                    "• Network connectivity issues\n" +
                    "• Rate limiting by search engines\n" +
                    "• Firewall or antivirus blocking\n\n" +
                    "Try again in a few moments or use Basic Search mode.", 
                    "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblProgress.Text = "? Search failed - Try again or use Basic mode";
                statusLabel.Text = "Search failed";
                progressBar.Value = 0;
            }
            finally
            {
                // Reset UI state
                _isSearching = false;
                btnSearch.Text = "?? Search";
                btnSearch.BackColor = Color.FromArgb(40, 167, 69);
                btnClear.Enabled = true;
                btnExport.Enabled = _allResults.Count > 0;
                progressBar.Visible = false;
            }
        }

        private List<SearchResult> ApplySearchModeFilter(List<SearchResult> results, int searchMode)
        {
            switch (searchMode)
            {
                case 2: // Social Media Focus
                    return results.Where(r => 
                        r.Domain.Contains("facebook") || r.Domain.Contains("twitter") || r.Domain.Contains("x.com") ||
                        r.Domain.Contains("instagram") || r.Domain.Contains("linkedin") || r.Domain.Contains("youtube") ||
                        r.Domain.Contains("tiktok") || r.Domain.Contains("pinterest") || r.Domain.Contains("snapchat") ||
                        r.SearchEngine.Contains("Social") || r.SearchEngine.Contains("LinkedIn") || 
                        r.SearchEngine.Contains("Twitter") || r.SearchEngine.Contains("YouTube"))
                        .OrderByDescending(r => r.Relevance)
                        .ToList();
                
                case 3: // Professional Focus
                    return results.Where(r => 
                        r.Domain.Contains("linkedin") || r.Domain.Contains("crunchbase") || r.Domain.Contains("bloomberg") ||
                        r.Domain.Contains("github") || r.Domain.Contains("angel.co") || r.Domain.Contains("medium") ||
                        r.Domain.Contains("behance") || r.Domain.Contains("dribbble") || r.Domain.Contains("indeed") ||
                        r.SearchEngine.Contains("Professional") || r.SearchEngine.Contains("LinkedIn") ||
                        r.SearchEngine.Contains("Job"))
                        .OrderByDescending(r => r.Relevance)
                        .ToList();
                
                case 4: // News Focus
                    return results.Where(r => 
                        r.SearchEngine.Contains("News") || r.Domain.Contains("news") || 
                        r.Domain.Contains("reuters") || r.Domain.Contains("bloomberg") || r.Domain.Contains("cnn") ||
                        r.Domain.Contains("bbc") || r.Domain.Contains("forbes") || r.Domain.Contains("techcrunch"))
                        .OrderByDescending(r => r.Relevance)
                        .ToList();
                
                case 5: // Academic Focus
                    return results.Where(r => 
                        r.Domain.Contains("scholar.google") || r.Domain.Contains("researchgate") || 
                        r.Domain.Contains("orcid") || r.Domain.Contains("academia.edu") || r.Domain.Contains(".edu") ||
                        r.SearchEngine.Contains("Academic") || r.SearchEngine.Contains("Professional"))
                        .OrderByDescending(r => r.Relevance)
                        .ToList();
                
                default:
                    return results;
            }
        }

        private int GetUniqueDomainsCount()
        {
            return _allResults.Select(r => r.Domain).Where(d => !string.IsNullOrEmpty(d)).Distinct().Count();
        }

        private void OnSearchProgressChanged(object sender, SearchProgress progress)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnSearchProgressChanged(sender, progress)));
                return;
            }

            progressBar.Value = Math.Min(progress.PercentComplete, 100);
            lblProgress.Text = $"{progress.CurrentAction} ({progress.ResultsFound} results found)";
            statusLabel.Text = $"{progress.CurrentAction} - {progress.ResultsFound} results found";
            
            if (!string.IsNullOrEmpty(progress.SearchEngine))
            {
                lblProgress.Text += $" - {progress.SearchEngine}";
                statusLabel.Text += $" - {progress.SearchEngine}";
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            // Clear all data
            _allResults.Clear();
            _filteredResults.Clear();
            listViewResults.Items.Clear();
            txtSearchName.Clear();
            txtFilter.Clear();
            
            // Reset UI
            UpdateResultCount();
            lblProgress.Text = "Ready to search with Enhanced Search Engine";
            statusLabel.Text = "Ready - Enhanced search engine with 25+ sources available";
            btnExport.Enabled = false;
            
            // Focus back to search box
            txtSearchName.Focus();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (_filteredResults.Count == 0)
            {
                MessageBox.Show("No results to export.", "No Data", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV Files (*.csv)|*.csv|JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt";
                saveDialog.FileName = $"NameScraper_Enhanced_Results_{DateTime.Now:yyyyMMdd_HHmmss}";
                
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportResults(saveDialog.FileName, saveDialog.FilterIndex);
                        MessageBox.Show($"Enhanced search results exported successfully!\n\n" +
                            $"File: {saveDialog.FileName}\n" +
                            $"Results: {_filteredResults.Count}\n" +
                            $"Sources: {GetUniqueDomainsCount()}", 
                            "Export Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to export results:\n\n{ex.Message}", 
                            "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportResults(string fileName, int filterIndex)
        {
            switch (filterIndex)
            {
                case 1: // CSV
                    ExportToCsv(fileName);
                    break;
                case 2: // JSON
                    ExportToJson(fileName);
                    break;
                case 3: // Text
                    ExportToText(fileName);
                    break;
                default:
                    ExportToCsv(fileName);
                    break;
            }
        }

        private void ExportToCsv(string fileName)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Title,URL,Description,Source,Domain,Relevance,Timestamp");
            
            foreach (var result in _filteredResults)
            {
                csv.AppendLine($"\"{EscapeCsv(result.Title)}\",\"{EscapeCsv(result.Url)}\",\"{EscapeCsv(result.Description)}\",\"{EscapeCsv(result.SearchEngine)}\",\"{EscapeCsv(result.Domain)}\",\"{result.Relevance}\",\"{result.Timestamp:yyyy-MM-dd HH:mm:ss}\"");
            }
            
            File.WriteAllText(fileName, csv.ToString(), Encoding.UTF8);
        }

        private void ExportToJson(string fileName)
        {
            var exportData = new
            {
                SearchInfo = new
                {
                    SearchTerm = txtSearchName.Text,
                    SearchMode = comboSearchEngine.Text,
                    TotalResults = _filteredResults.Count,
                    UniqueDomains = GetUniqueDomainsCount(),
                    ExportTimestamp = DateTime.Now,
                    SearchEngine = _useEnhancedSearch ? "Enhanced" : "Basic"
                },
                Results = _filteredResults
            };
            
            var json = JsonConvert.SerializeObject(exportData, Formatting.Indented);
            File.WriteAllText(fileName, json, Encoding.UTF8);
        }

        private void ExportToText(string fileName)
        {
            var text = new StringBuilder();
            text.AppendLine($"Enhanced Name Scraper Results - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            text.AppendLine($"Search Term: {txtSearchName.Text}");
            text.AppendLine($"Search Mode: {comboSearchEngine.Text}");
            text.AppendLine($"Search Engine: {(_useEnhancedSearch ? "Enhanced (25+ sources)" : "Basic (4 sources)")}");
            text.AppendLine($"Total Results: {_filteredResults.Count}");
            text.AppendLine($"Unique Domains: {GetUniqueDomainsCount()}");
            text.AppendLine(new string('=', 80));
            text.AppendLine();
            
            var groupedBySource = _filteredResults.GroupBy(r => r.SearchEngine).OrderByDescending(g => g.Count());
            foreach (var group in groupedBySource)
            {
                text.AppendLine($"=== {group.Key} ({group.Count()} results) ===");
                text.AppendLine();
                
                foreach (var result in group.OrderByDescending(r => r.Relevance))
                {
                    text.AppendLine($"Title: {result.Title}");
                    text.AppendLine($"URL: {result.Url}");
                    text.AppendLine($"Description: {result.Description}");
                    text.AppendLine($"Domain: {result.Domain}");
                    text.AppendLine($"Relevance Score: {result.Relevance}");
                    text.AppendLine($"Timestamp: {result.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    text.AppendLine(new string('-', 40));
                    text.AppendLine();
                }
                text.AppendLine();
            }
            
            File.WriteAllText(fileName, text.ToString(), Encoding.UTF8);
        }

        private string EscapeCsv(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("\"", "\"\"").Replace("\n", " ").Replace("\r", " ");
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filterText = txtFilter.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(filterText))
            {
                _filteredResults = new List<SearchResult>(_allResults);
            }
            else
            {
                _filteredResults = _allResults.Where(result =>
                    result.Title.ToLower().Contains(filterText) ||
                    result.Description.ToLower().Contains(filterText) ||
                    result.SearchEngine.ToLower().Contains(filterText) ||
                    result.Domain.ToLower().Contains(filterText) ||
                    result.Url.ToLower().Contains(filterText)
                ).ToList();
            }
            
            UpdateResultsDisplay();
            UpdateResultCount();
        }

        private void UpdateResultsDisplay()
        {
            listViewResults.BeginUpdate();
            listViewResults.Items.Clear();
            
            foreach (var result in _filteredResults)
            {
                var item = new ListViewItem(result.Title);
                item.SubItems.Add(result.Url);
                item.SubItems.Add(result.Description);
                item.SubItems.Add(result.SearchEngine);
                item.SubItems.Add(result.Domain);
                item.Tag = result;
                
                // Enhanced color coding by search engine type
                switch (result.SearchEngine.ToLower())
                {
                    case var engine when engine.Contains("google"):
                        item.BackColor = Color.FromArgb(255, 248, 248);
                        break;
                    case var engine when engine.Contains("bing"):
                        item.BackColor = Color.FromArgb(248, 248, 255);
                        break;
                    case var engine when engine.Contains("linkedin"):
                        item.BackColor = Color.FromArgb(240, 255, 255);
                        break;
                    case var engine when engine.Contains("social"):
                        item.BackColor = Color.FromArgb(248, 255, 248);
                        break;
                    case var engine when engine.Contains("twitter") || engine.Contains("x"):
                        item.BackColor = Color.FromArgb(240, 248, 255);
                        break;
                    case var engine when engine.Contains("news"):
                        item.BackColor = Color.FromArgb(255, 255, 248);
                        break;
                    case var engine when engine.Contains("professional"):
                        item.BackColor = Color.FromArgb(255, 248, 255);
                        break;
                    case var engine when engine.Contains("enhanced"):
                        item.BackColor = Color.FromArgb(248, 255, 240);
                        break;
                    default:
                        item.BackColor = Color.White;
                        break;
                }
                
                // Highlight high relevance results
                if (result.Relevance > 80)
                {
                    item.Font = new Font(item.Font, FontStyle.Bold);
                }
                
                listViewResults.Items.Add(item);
            }
            
            listViewResults.EndUpdate();
        }

        private void UpdateResultCount()
        {
            lblResultCount.Text = $"Results: {_filteredResults.Count}";
            if (_filteredResults.Count != _allResults.Count)
            {
                lblResultCount.Text += $" (filtered from {_allResults.Count})";
            }
            
            if (_allResults.Count > 0)
            {
                lblResultCount.Text += $" | Domains: {GetUniqueDomainsCount()}";
            }
            
            btnExport.Enabled = _filteredResults.Count > 0;
        }

        private void ListViewResults_DoubleClick(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var result = (SearchResult)listViewResults.SelectedItems[0].Tag;
                OpenUrl(result.Url);
            }
        }

        private void OpenUrlMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var result = (SearchResult)listViewResults.SelectedItems[0].Tag;
                OpenUrl(result.Url);
            }
        }

        private void CopyUrlMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var result = (SearchResult)listViewResults.SelectedItems[0].Tag;
                Clipboard.SetText(result.Url);
                lblProgress.Text = "?? URL copied to clipboard";
                statusLabel.Text = "URL copied to clipboard";
            }
        }

        private void CopyTitleMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var result = (SearchResult)listViewResults.SelectedItems[0].Tag;
                Clipboard.SetText(result.Title);
                lblProgress.Text = "?? Title copied to clipboard";
                statusLabel.Text = "Title copied to clipboard";
            }
        }

        private void SaveResultMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewResults.SelectedItems.Count > 0)
            {
                var result = (SearchResult)listViewResults.SelectedItems[0].Tag;
                
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt";
                    saveDialog.FileName = $"SingleResult_{DateTime.Now:yyyyMMdd_HHmmss}";
                    
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            if (saveDialog.FilterIndex == 1)
                            {
                                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                                File.WriteAllText(saveDialog.FileName, json, Encoding.UTF8);
                            }
                            else
                            {
                                var text = $"Enhanced Name Scraper - Single Result\n" +
                                    $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n" +
                                    $"Title: {result.Title}\n" +
                                    $"URL: {result.Url}\n" +
                                    $"Description: {result.Description}\n" +
                                    $"Source: {result.SearchEngine}\n" +
                                    $"Domain: {result.Domain}\n" +
                                    $"Relevance Score: {result.Relevance}\n" +
                                    $"Timestamp: {result.Timestamp:yyyy-MM-dd HH:mm:ss}";
                                File.WriteAllText(saveDialog.FileName, text, Encoding.UTF8);
                            }
                            
                            lblProgress.Text = "?? Result saved successfully";
                            statusLabel.Text = "Individual result saved successfully";
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to save result:\n\n{ex.Message}", 
                                "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    statusLabel.Text = $"Opened: {ExtractDomain(url)}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open URL:\n\n{ex.Message}", 
                    "URL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ExtractDomain(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return "";
                var uri = new Uri(url);
                return uri.Host.ToLower().Replace("www.", "");
            }
            catch
            {
                return "";
            }
        }

        #region Menu Event Handlers

        private void NewSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnClear_Click(sender, e);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewResults.Items)
            {
                item.Selected = true;
            }
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = !statusStrip.Visible;
            statusBarToolStripMenuItem.Checked = statusStrip.Visible;
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settingsText = $"Enhanced Name Scraper Settings\n\n" +
                $"Current Configuration:\n" +
                $"• Search Mode: {comboSearchEngine.Text}\n" +
                $"• Results Limit: 500 per search\n" +
                $"• Concurrent Requests: 10\n" +
                $"• Search Sources: 25+ platforms\n" +
                $"• Rate Limiting: Enabled\n" +
                $"• User Agents: 5 rotating agents\n\n" +
                $"Advanced Features:\n" +
                $"• Multi-engine parallel search\n" +
                $"• Social media deep scanning\n" +
                $"• Professional platform integration\n" +
                $"• News and blog coverage\n" +
                $"• Academic database search\n" +
                $"• Intelligent relevance scoring\n" +
                $"• Duplicate detection and removal\n\n" +
                $"For custom configurations or enterprise features,\n" +
                $"contact support or check documentation.";

            MessageBox.Show(settingsText, "Enhanced Search Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UserGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var helpText = @"Enhanced Ultimate Name Scraper - User Guide

HOW TO USE:
1. Enter a name in the search box
2. Choose search mode:
   - Enhanced Search: Uses all 25+ sources (recommended)
   - Basic Search: Uses 4 main search engines
   - Focus modes: Target specific platform types
3. Click 'Search' or press Enter
4. Wait for results from multiple sources
5. Use filter box to narrow down results
6. Right-click results for more options
7. Export results using Export button

ENHANCED FEATURES:
• 25+ Search Sources: Google, Bing, Yahoo, DuckDuckGo, Yandex, Baidu
• Social Media: LinkedIn, Twitter/X, Facebook, Instagram, YouTube, TikTok, Pinterest
• Professional: Crunchbase, Bloomberg, AngelList, GitHub, Behance, Dribbble
• Job Platforms: Indeed, Glassdoor, Monster, Dice
• Academic: Google Scholar, ResearchGate, ORCID
• News & Media: Enhanced news search, blog discovery
• Forums: Reddit, StackOverflow, Quora, specialized forums

SEARCH MODES:
• Enhanced Search: Maximum coverage across all platforms
• Basic Search: Traditional search engines only
• Social Media Focus: Prioritizes social platforms
• Professional Focus: Business and career platforms
• News Focus: Media and news sources
• Academic Focus: Scholarly and research platforms

ADVANCED FEATURES:
• Parallel Processing: Multiple simultaneous searches
• Rate Limiting: Respectful server interaction
• Smart Relevance: AI-powered result ranking
• Duplicate Detection: Automatic result deduplication
• Multiple User Agents: Rotating browser identities
• Error Recovery: Continues on individual source failures

KEYBOARD SHORTCUTS:
• Ctrl+N: New Search
• Ctrl+E: Export Results
• Ctrl+Delete: Clear Results
• Ctrl+A: Select All Results
• F1: Show this help
• Alt+F4: Exit application
• Enter: Start Search

SEARCH TIPS:
• Try different name variations and spellings
• Use Enhanced Search for maximum coverage
• Apply focus modes for targeted results
• Use filters to narrow large result sets
• Check multiple pages of results
• Verify information through multiple sources

EXPORT OPTIONS:
• CSV: Spreadsheet-compatible format
• JSON: Structured data for developers
• TXT: Human-readable text format
• Individual Results: Save specific findings

TROUBLESHOOTING:
• No results: Try name variations, check spelling
• Search errors: Use Basic mode, check internet connection
• Slow performance: Close other applications
• Rate limiting: Wait and retry, use different search mode

The Enhanced Search Engine provides comprehensive coverage
across the internet while respecting platform policies and
implementing proper rate limiting for sustainable usage.

For technical support or feature requests,
refer to the application documentation.";

            var helpForm = new Form
            {
                Text = "Enhanced User Guide - Ultimate Name Scraper",
                Size = new Size(700, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Text = helpText,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(10)
            };

            helpForm.Controls.Add(textBox);
            helpForm.ShowDialog(this);
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog(this);
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Cancel any ongoing search
            _cancellationTokenSource?.Cancel();
            _enhancedSearchService?.Dispose();
            _basicSearchService?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
