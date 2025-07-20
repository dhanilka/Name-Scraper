using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NameScraper
{
    public class NameSearchService
    {
        private readonly HttpClient _httpClient;
        private readonly List<string> _searchEngines;
        private readonly Random _random;

        public event EventHandler<SearchProgress>? ProgressChanged;

        public NameSearchService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
            
            _searchEngines = new List<string>
            {
                "Google",
                "Bing",
                "DuckDuckGo",
                "Yahoo",
                "Custom Scraper"
            };
            
            _random = new Random();
        }

        public async Task<List<SearchResult>> SearchNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var allResults = new List<SearchResult>();
            var progress = new SearchProgress();

            try
            {
                // Search multiple sources
                var tasks = new List<Task<List<SearchResult>>>
                {
                    SearchGoogleAsync(name, cancellationToken),
                    SearchBingAsync(name, cancellationToken),
                    SearchDuckDuckGoAsync(name, cancellationToken),
                    SearchYahooAsync(name, cancellationToken),
                    SearchSocialMediaAsync(name, cancellationToken),
                    SearchNewsAsync(name, cancellationToken),
                    SearchForumsAsync(name, cancellationToken)
                };

                for (int i = 0; i < tasks.Count; i++)
                {
                    progress.PercentComplete = (i * 100) / tasks.Count;
                    progress.CurrentAction = $"Searching {_searchEngines[Math.Min(i, _searchEngines.Count - 1)]}...";
                    progress.SearchEngine = _searchEngines[Math.Min(i, _searchEngines.Count - 1)];
                    ProgressChanged?.Invoke(this, progress);

                    try
                    {
                        var results = await tasks[i];
                        allResults.AddRange(results);
                        progress.ResultsFound = allResults.Count;
                        ProgressChanged?.Invoke(this, progress);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other search engines
                        Console.WriteLine($"Error searching: {ex.Message}");
                    }

                    // Small delay to avoid overwhelming servers
                    await Task.Delay(500, cancellationToken);
                }

                progress.PercentComplete = 100;
                progress.CurrentAction = "Search completed!";
                ProgressChanged?.Invoke(this, progress);
            }
            catch (Exception ex)
            {
                throw new Exception($"Search failed: {ex.Message}");
            }

            // Remove duplicates and sort by relevance
            return allResults
                .GroupBy(r => r.Url)
                .Select(g => g.First())
                .OrderByDescending(r => r.Relevance)
                .Take(100)
                .ToList();
        }

        private async Task<List<SearchResult>> SearchGoogleAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            try
            {
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://www.google.com/search?q={query}&num=20";
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var searchResults = doc.DocumentNode.SelectNodes("//div[@class='g']");
                if (searchResults != null)
                {
                    foreach (var result in searchResults.Take(10))
                    {
                        var titleNode = result.SelectSingleNode(".//h3");
                        var linkNode = result.SelectSingleNode(".//a[@href]");
                        var descNode = result.SelectSingleNode(".//span[@class='aCOpRe']");

                        if (titleNode != null && linkNode != null)
                        {
                            var href = linkNode.GetAttributeValue("href", "");
                            if (href.StartsWith("/url?q="))
                            {
                                href = HttpUtility.ParseQueryString(href.Substring(7))[""] ?? "";
                            }

                            results.Add(new SearchResult
                            {
                                Title = titleNode.InnerText.Trim(),
                                Url = href,
                                Description = descNode?.InnerText?.Trim() ?? "",
                                SearchEngine = "Google",
                                Relevance = CalculateRelevance(titleNode.InnerText, name),
                                Domain = ExtractDomain(href)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google search error: {ex.Message}");
            }
            return results;
        }

        private async Task<List<SearchResult>> SearchBingAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            try
            {
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://www.bing.com/search?q={query}&count=20";
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var searchResults = doc.DocumentNode.SelectNodes("//li[@class='b_algo']");
                if (searchResults != null)
                {
                    foreach (var result in searchResults.Take(10))
                    {
                        var titleNode = result.SelectSingleNode(".//h2/a");
                        var descNode = result.SelectSingleNode(".//p[@class='b_lineclamp4 b_algoSlug']");

                        if (titleNode != null)
                        {
                            results.Add(new SearchResult
                            {
                                Title = titleNode.InnerText.Trim(),
                                Url = titleNode.GetAttributeValue("href", ""),
                                Description = descNode?.InnerText?.Trim() ?? "",
                                SearchEngine = "Bing",
                                Relevance = CalculateRelevance(titleNode.InnerText, name),
                                Domain = ExtractDomain(titleNode.GetAttributeValue("href", ""))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Bing search error: {ex.Message}");
            }
            return results;
        }

        private async Task<List<SearchResult>> SearchDuckDuckGoAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            try
            {
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://duckduckgo.com/html/?q={query}";
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var searchResults = doc.DocumentNode.SelectNodes("//div[@class='result']");
                if (searchResults != null)
                {
                    foreach (var result in searchResults.Take(10))
                    {
                        var titleNode = result.SelectSingleNode(".//a[@class='result__a']");
                        var descNode = result.SelectSingleNode(".//a[@class='result__snippet']");

                        if (titleNode != null)
                        {
                            results.Add(new SearchResult
                            {
                                Title = titleNode.InnerText.Trim(),
                                Url = titleNode.GetAttributeValue("href", ""),
                                Description = descNode?.InnerText?.Trim() ?? "",
                                SearchEngine = "DuckDuckGo",
                                Relevance = CalculateRelevance(titleNode.InnerText, name),
                                Domain = ExtractDomain(titleNode.GetAttributeValue("href", ""))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DuckDuckGo search error: {ex.Message}");
            }
            return results;
        }

        private async Task<List<SearchResult>> SearchYahooAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            try
            {
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://search.yahoo.com/search?p={query}&n=20";
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var searchResults = doc.DocumentNode.SelectNodes("//div[@class='dd algo algo-sr Sr']");
                if (searchResults != null)
                {
                    foreach (var result in searchResults.Take(10))
                    {
                        var titleNode = result.SelectSingleNode(".//h3/a");
                        var descNode = result.SelectSingleNode(".//p[@class='fz-ms lh-1_43x']");

                        if (titleNode != null)
                        {
                            results.Add(new SearchResult
                            {
                                Title = titleNode.InnerText.Trim(),
                                Url = titleNode.GetAttributeValue("href", ""),
                                Description = descNode?.InnerText?.Trim() ?? "",
                                SearchEngine = "Yahoo",
                                Relevance = CalculateRelevance(titleNode.InnerText, name),
                                Domain = ExtractDomain(titleNode.GetAttributeValue("href", ""))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yahoo search error: {ex.Message}");
            }
            return results;
        }

        private async Task<List<SearchResult>> SearchSocialMediaAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            var socialSites = new[]
            {
                "facebook.com",
                "twitter.com",
                "linkedin.com",
                "instagram.com",
                "youtube.com",
                "tiktok.com",
                "pinterest.com",
                "reddit.com"
            };

            foreach (var site in socialSites)
            {
                try
                {
                    var query = HttpUtility.UrlEncode($"site:{site} \"{name}\"");
                    var url = $"https://www.google.com/search?q={query}&num=5";
                    
                    var response = await _httpClient.GetStringAsync(url, cancellationToken);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(response);

                    var searchResults = doc.DocumentNode.SelectNodes("//div[@class='g']");
                    if (searchResults != null)
                    {
                        foreach (var result in searchResults.Take(3))
                        {
                            var titleNode = result.SelectSingleNode(".//h3");
                            var linkNode = result.SelectSingleNode(".//a[@href]");

                            if (titleNode != null && linkNode != null)
                            {
                                var href = linkNode.GetAttributeValue("href", "");
                                if (href.StartsWith("/url?q="))
                                {
                                    href = HttpUtility.ParseQueryString(href.Substring(7))[""] ?? "";
                                }

                                results.Add(new SearchResult
                                {
                                    Title = titleNode.InnerText.Trim(),
                                    Url = href,
                                    Description = $"Social media profile on {site}",
                                    SearchEngine = "Social Media",
                                    Relevance = CalculateRelevance(titleNode.InnerText, name) + 10, // Boost social media results
                                    Domain = site
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Social media search error for {site}: {ex.Message}");
                }

                await Task.Delay(200, cancellationToken); // Rate limiting
            }

            return results;
        }

        private async Task<List<SearchResult>> SearchNewsAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            try
            {
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://www.google.com/search?q={query}&tbm=nws&num=10";
                
                var response = await _httpClient.GetStringAsync(url, cancellationToken);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(response);

                var searchResults = doc.DocumentNode.SelectNodes("//div[@class='SoaBEf']");
                if (searchResults != null)
                {
                    foreach (var result in searchResults.Take(10))
                    {
                        var titleNode = result.SelectSingleNode(".//div[@class='MBeuO']");
                        var linkNode = result.SelectSingleNode(".//a[@href]");
                        var descNode = result.SelectSingleNode(".//div[@class='GI74Re nDgy9d']");

                        if (titleNode != null && linkNode != null)
                        {
                            results.Add(new SearchResult
                            {
                                Title = titleNode.InnerText.Trim(),
                                Url = linkNode.GetAttributeValue("href", ""),
                                Description = descNode?.InnerText?.Trim() ?? "",
                                SearchEngine = "News",
                                Relevance = CalculateRelevance(titleNode.InnerText, name) + 5,
                                Domain = ExtractDomain(linkNode.GetAttributeValue("href", ""))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"News search error: {ex.Message}");
            }
            return results;
        }

        private async Task<List<SearchResult>> SearchForumsAsync(string name, CancellationToken cancellationToken)
        {
            var results = new List<SearchResult>();
            var forumSites = new[]
            {
                "reddit.com",
                "stackoverflow.com",
                "quora.com",
                "forum",
                "community"
            };

            foreach (var site in forumSites)
            {
                try
                {
                    var query = HttpUtility.UrlEncode($"site:{site} \"{name}\"");
                    var url = $"https://www.google.com/search?q={query}&num=5";
                    
                    var response = await _httpClient.GetStringAsync(url, cancellationToken);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(response);

                    var searchResults = doc.DocumentNode.SelectNodes("//div[@class='g']");
                    if (searchResults != null)
                    {
                        foreach (var result in searchResults.Take(2))
                        {
                            var titleNode = result.SelectSingleNode(".//h3");
                            var linkNode = result.SelectSingleNode(".//a[@href]");

                            if (titleNode != null && linkNode != null)
                            {
                                var href = linkNode.GetAttributeValue("href", "");
                                if (href.StartsWith("/url?q="))
                                {
                                    href = HttpUtility.ParseQueryString(href.Substring(7))[""] ?? "";
                                }

                                results.Add(new SearchResult
                                {
                                    Title = titleNode.InnerText.Trim(),
                                    Url = href,
                                    Description = $"Forum discussion mentioning {name}",
                                    SearchEngine = "Forums",
                                    Relevance = CalculateRelevance(titleNode.InnerText, name),
                                    Domain = ExtractDomain(href)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Forum search error for {site}: {ex.Message}");
                }

                await Task.Delay(200, cancellationToken);
            }

            return results;
        }

        private int CalculateRelevance(string text, string searchName)
        {
            var score = 0;
            var lowerText = text.ToLower();
            var lowerName = searchName.ToLower();

            // Exact match in title
            if (lowerText.Contains(lowerName))
                score += 50;

            // Partial match
            var nameWords = lowerName.Split(' ');
            foreach (var word in nameWords)
            {
                if (lowerText.Contains(word))
                    score += 10;
            }

            // Boost for certain domains
            if (text.Contains("linkedin") || text.Contains("facebook") || text.Contains("twitter"))
                score += 15;

            return score;
        }

        private string ExtractDomain(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return "";
                var uri = new Uri(url);
                return uri.Host;
            }
            catch
            {
                return "";
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}