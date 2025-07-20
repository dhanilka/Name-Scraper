using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NameScraper
{
    public class EnhancedNameSearchService
    {
        private readonly HttpClient[] _httpClients;
        private readonly Random _random;
        private readonly List<string> _userAgents;
        private readonly SemaphoreSlim _rateLimitSemaphore;
        private readonly IBrowsingContext _browsingContext;

        public event EventHandler<SearchProgress>? ProgressChanged;

        public EnhancedNameSearchService()
        {
            // Initialize multiple HTTP clients with different configurations
            _httpClients = new HttpClient[5];
            for (int i = 0; i < _httpClients.Length; i++)
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                    UseCookies = true
                };

                _httpClients[i] = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
            }

            _random = new Random();
            _rateLimitSemaphore = new SemaphoreSlim(10, 10); // Allow 10 concurrent requests

            // Multiple user agents for better success rate
            _userAgents = new List<string>
            {
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36 Edg/119.0.0.0",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:120.0) Gecko/20100101 Firefox/120.0",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36"
            };

            // Initialize AngleSharp for advanced HTML parsing
            var config = Configuration.Default.WithDefaultLoader();
            _browsingContext = BrowsingContext.New(config);
        }

        public async Task<List<SearchResult>> SearchNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var allResults = new ConcurrentBag<SearchResult>();
            var progress = new SearchProgress();

            var searchTasks = new List<Task>
            {
                // Primary search engines with enhanced techniques
                SearchGoogleEnhancedAsync(name, allResults, cancellationToken),
                SearchBingEnhancedAsync(name, allResults, cancellationToken),
                SearchDuckDuckGoEnhancedAsync(name, allResults, cancellationToken),
                SearchYahooEnhancedAsync(name, allResults, cancellationToken),
                SearchYandexAsync(name, allResults, cancellationToken),
                SearchBaiduAsync(name, allResults, cancellationToken),

                // Social media platforms (enhanced)
                SearchSocialMediaEnhancedAsync(name, allResults, cancellationToken),
                SearchLinkedInAsync(name, allResults, cancellationToken),
                SearchTwitterAsync(name, allResults, cancellationToken),
                SearchInstagramAsync(name, allResults, cancellationToken),
                SearchFacebookAsync(name, allResults, cancellationToken),
                SearchYouTubeAsync(name, allResults, cancellationToken),
                SearchTikTokAsync(name, allResults, cancellationToken),
                SearchPinterestAsync(name, allResults, cancellationToken),

                // Professional and business platforms
                SearchProfessionalPlatformsAsync(name, allResults, cancellationToken),
                SearchBusinessDirectoriesAsync(name, allResults, cancellationToken),
                SearchAcademicPlatformsAsync(name, allResults, cancellationToken),

                // News and media
                SearchNewsEnhancedAsync(name, allResults, cancellationToken),
                SearchBlogsAsync(name, allResults, cancellationToken),
                SearchPodcastsAsync(name, allResults, cancellationToken),

                // Forums and communities
                SearchForumsEnhancedAsync(name, allResults, cancellationToken),
                SearchRedditEnhancedAsync(name, allResults, cancellationToken),
                SearchStackOverflowAsync(name, allResults, cancellationToken),
                SearchQuoraAsync(name, allResults, cancellationToken),

                // Specialized searches
                SearchImagePlatformsAsync(name, allResults, cancellationToken),
                SearchJobPlatformsAsync(name, allResults, cancellationToken),
                SearchDirectoriesAsync(name, allResults, cancellationToken),
                SearchPublicRecordsAsync(name, allResults, cancellationToken)
            };

            // Execute searches with progress tracking
            var totalTasks = searchTasks.Count;
            var completedTasks = 0;

            foreach (var task in searchTasks)
            {
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Search task error: {ex.Message}");
                }

                completedTasks++;
                progress.PercentComplete = (completedTasks * 100) / totalTasks;
                progress.CurrentAction = $"Completed {completedTasks}/{totalTasks} search sources";
                progress.ResultsFound = allResults.Count;
                ProgressChanged?.Invoke(this, progress);
            }

            // Process and deduplicate results
            var results = allResults.ToList()
                .GroupBy(r => NormalizeUrl(r.Url))
                .Select(g => g.OrderByDescending(x => x.Relevance).First())
                .Where(r => !string.IsNullOrEmpty(r.Url) && !string.IsNullOrEmpty(r.Title))
                .OrderByDescending(r => r.Relevance)
                .Take(500) // Increased limit
                .ToList();

            progress.PercentComplete = 100;
            progress.CurrentAction = "Search completed!";
            progress.ResultsFound = results.Count;
            ProgressChanged?.Invoke(this, progress);

            return results;
        }

        private async Task SearchGoogleEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var queries = GenerateSearchQueries(name);

                foreach (var query in queries.Take(3))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&num=20&hl=en";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        // Multiple selectors for better coverage
                        var selectors = new[]
                        {
                            "div.g",
                            "div[data-ved]",
                            ".search-result",
                            ".rc"
                        };

                        foreach (var selector in selectors)
                        {
                            var searchResults = document.QuerySelectorAll(selector);
                            foreach (var result in searchResults.Take(15))
                            {
                                var titleElement = result.QuerySelector("h3") ?? result.QuerySelector("a h3") ?? result.QuerySelector(".LC20lb");
                                var linkElement = result.QuerySelector("a[href]");
                                var descElement = result.QuerySelector(".VwiC3b") ?? result.QuerySelector(".aCOpRe") ?? result.QuerySelector(".s3v9rd");

                                if (titleElement != null && linkElement != null)
                                {
                                    var href = linkElement.GetAttribute("href");
                                    if (!string.IsNullOrEmpty(href))
                                    {
                                        href = CleanGoogleUrl(href);
                                        if (IsValidUrl(href))
                                        {
                                            results.Add(new SearchResult
                                            {
                                                Title = CleanText(titleElement.TextContent),
                                                Url = href,
                                                Description = CleanText(descElement?.TextContent ?? ""),
                                                SearchEngine = "Google Enhanced",
                                                Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href),
                                                Domain = ExtractDomain(href)
                                            });
                                        }
                                    }
                                }
                            }
                        }

                        await Task.Delay(_random.Next(1000, 2000), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Google enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchBingEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var queries = GenerateSearchQueries(name);

                foreach (var query in queries.Take(2))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.bing.com/search?q={encodedQuery}&count=20";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("li.b_algo, .b_algo");
                        foreach (var result in searchResults.Take(15))
                        {
                            var titleElement = result.QuerySelector("h2 a") ?? result.QuerySelector("a h2");
                            var descElement = result.QuerySelector("p") ?? result.QuerySelector(".b_caption p");

                            if (titleElement != null)
                            {
                                var href = titleElement.GetAttribute("href");
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? ""),
                                        SearchEngine = "Bing Enhanced",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href),
                                        Domain = ExtractDomain(href)
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(800, 1500), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Bing enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchDuckDuckGoEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var queries = GenerateSearchQueries(name);

                foreach (var query in queries.Take(2))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://duckduckgo.com/html/?q={encodedQuery}&s=0";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.result, .web-result, .results_links");
                        foreach (var result in searchResults.Take(12))
                        {
                            var titleElement = result.QuerySelector("a.result__a, h2 a, .result_title a");
                            var descElement = result.QuerySelector(".result__snippet, .result_snippet");

                            if (titleElement != null)
                            {
                                var href = titleElement.GetAttribute("href");
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? ""),
                                        SearchEngine = "DuckDuckGo Enhanced",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href),
                                        Domain = ExtractDomain(href)
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(1000, 2000), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"DuckDuckGo enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchYahooEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var queries = GenerateSearchQueries(name);

                foreach (var query in queries.Take(2))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://search.yahoo.com/search?p={encodedQuery}&n=20";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.algo, .Sr, .dd.algo");
                        foreach (var result in searchResults.Take(12))
                        {
                            var titleElement = result.QuerySelector("h3 a, .ac-algo h3 a");
                            var descElement = result.QuerySelector("p, .compText");

                            if (titleElement != null)
                            {
                                var href = titleElement.GetAttribute("href");
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? ""),
                                        SearchEngine = "Yahoo Enhanced",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href),
                                        Domain = ExtractDomain(href)
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(800, 1500), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Yahoo enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchSocialMediaEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            var socialPlatforms = new Dictionary<string, string[]>
            {
                ["twitter.com"] = new[] { $"site:twitter.com \"{name}\"", $"site:x.com \"{name}\"" },
                ["instagram.com"] = new[] { $"site:instagram.com \"{name}\"", $"site:instagram.com/p \"{name}\"" },
                ["facebook.com"] = new[] { $"site:facebook.com \"{name}\"", $"site:facebook.com/people \"{name}\"" },
                ["tiktok.com"] = new[] { $"site:tiktok.com \"{name}\"", $"site:tiktok.com/@{name.Replace(" ", "")}" },
                ["snapchat.com"] = new[] { $"site:snapchat.com \"{name}\"" },
                ["pinterest.com"] = new[] { $"site:pinterest.com \"{name}\"" },
                ["tumblr.com"] = new[] { $"site:tumblr.com \"{name}\"" },
                ["discord.gg"] = new[] { $"site:discord.gg \"{name}\"" }
            };

            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();

                foreach (var platform in socialPlatforms.Take(6))
                {
                    foreach (var query in platform.Value.Take(1))
                    {
                        try
                        {
                            var encodedQuery = HttpUtility.UrlEncode(query);
                            var url = $"https://www.google.com/search?q={encodedQuery}&num=8";

                            var response = await client.GetStringAsync(url, cancellationToken);
                            var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                            var searchResults = document.QuerySelectorAll("div.g");
                            foreach (var result in searchResults.Take(5))
                            {
                                var titleElement = result.QuerySelector("h3");
                                var linkElement = result.QuerySelector("a[href]");

                                if (titleElement != null && linkElement != null)
                                {
                                    var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                    if (href.Contains(platform.Key) && IsValidUrl(href))
                                    {
                                        results.Add(new SearchResult
                                        {
                                            Title = CleanText(titleElement.TextContent),
                                            Url = href,
                                            Description = $"Social media profile on {platform.Key}",
                                            SearchEngine = "Social Media Enhanced",
                                            Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 25,
                                            Domain = platform.Key
                                        });
                                    }
                                }
                            }

                            await Task.Delay(_random.Next(400, 800), cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Social media enhanced search error for {platform.Key}: {ex.Message}");
                        }
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchTwitterAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var twitterQueries = new[]
                {
                    $"site:twitter.com \"{name}\"",
                    $"site:x.com \"{name}\"",
                    $"site:twitter.com/@{name.Replace(" ", "")}",
                    $"site:x.com/@{name.Replace(" ", "")}",
                    $"\"{name}\" site:twitter.com profile",
                    $"\"{name}\" site:x.com profile"
                };

                foreach (var query in twitterQueries.Take(3))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&num=10";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(8))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");
                            var descElement = result.QuerySelector(".VwiC3b, .aCOpRe");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if ((href.Contains("twitter.com") || href.Contains("x.com")) && IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? $"Twitter profile for {name}"),
                                        SearchEngine = "Twitter/X",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 30,
                                        Domain = href.Contains("x.com") ? "x.com" : "twitter.com"
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(600, 1200), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Twitter search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchRedditEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var redditQueries = new[]
                {
                    $"site:reddit.com \"{name}\"",
                    $"site:reddit.com/user \"{name}\"",
                    $"site:reddit.com/u \"{name}\"",
                    $"\"{name}\" reddit profile",
                    $"\"{name}\" reddit user"
                };

                foreach (var query in redditQueries.Take(3))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&num=10";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(8))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");
                            var descElement = result.QuerySelector(".VwiC3b, .aCOpRe");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if (href.Contains("reddit.com") && IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? $"Reddit discussion mentioning {name}"),
                                        SearchEngine = "Reddit Enhanced",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 15,
                                        Domain = "reddit.com"
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(500, 1000), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Reddit enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchYouTubeAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var youtubeQueries = new[]
                {
                    $"site:youtube.com \"{name}\"",
                    $"site:youtube.com/channel \"{name}\"",
                    $"site:youtube.com/user \"{name}\"",
                    $"site:youtube.com/c \"{name}\"",
                    $"site:youtube.com/@{name.Replace(" ", "")}",
                    $"\"{name}\" youtube channel"
                };

                foreach (var query in youtubeQueries.Take(4))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&num=8";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(6))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");
                            var descElement = result.QuerySelector(".VwiC3b, .aCOpRe");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if (href.Contains("youtube.com") && IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? $"YouTube channel or video featuring {name}"),
                                        SearchEngine = "YouTube",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 20,
                                        Domain = "youtube.com"
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(500, 1000), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"YouTube search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchJobPlatformsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            var jobPlatforms = new[]
            {
                "indeed.com",
                "glassdoor.com",
                "monster.com",
                "ziprecruiter.com",
                "careerbuilder.com",
                "dice.com",
                "hired.com",
                "angel.co"
            };

            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();

                foreach (var platform in jobPlatforms.Take(6))
                {
                    try
                    {
                        var query = HttpUtility.UrlEncode($"site:{platform} \"{name}\"");
                        var url = $"https://www.google.com/search?q={query}&num=5";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(3))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = $"Professional profile or job listing on {platform}",
                                        SearchEngine = "Job Platforms",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 10,
                                        Domain = platform
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(300, 700), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Job platform search error for {platform}: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchYandexAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var query = HttpUtility.UrlEncode($"\"{name}\"");
                var url = $"https://yandex.com/search/?text={query}&lr=213";

                var response = await client.GetStringAsync(url, cancellationToken);
                var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                var searchResults = document.QuerySelectorAll(".organic, .serp-item");
                foreach (var result in searchResults.Take(10))
                {
                    var titleElement = result.QuerySelector("h2 a") ?? result.QuerySelector(".organic__title a");
                    var descElement = result.QuerySelector(".organic__text") ?? result.QuerySelector(".text-container");

                    if (titleElement != null)
                    {
                        var href = titleElement.GetAttribute("href");
                        if (IsValidUrl(href))
                        {
                            results.Add(new SearchResult
                            {
                                Title = CleanText(titleElement.TextContent),
                                Url = href,
                                Description = CleanText(descElement?.TextContent ?? ""),
                                SearchEngine = "Yandex",
                                Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href),
                                Domain = ExtractDomain(href)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Yandex search error: {ex.Message}");
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchLinkedInAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var queries = new[]
                {
                    $"site:linkedin.com/in \"{name}\"",
                    $"site:linkedin.com \"{name}\" profile",
                    $"linkedin.com/in/{name.Replace(" ", "-").ToLower()}",
                    $"linkedin.com/in/{name.Replace(" ", "").ToLower()}"
                };

                foreach (var query in queries)
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&num=10";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(5))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if (href.Contains("linkedin.com") && IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = $"LinkedIn profile for {name}",
                                        SearchEngine = "LinkedIn",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 20,
                                        Domain = "linkedin.com"
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(500, 1000), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"LinkedIn search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchProfessionalPlatformsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            var platforms = new[]
            {
                "crunchbase.com",
                "bloomberg.com",
                "forbes.com",
                "angel.co",
                "behance.net",
                "dribbble.com",
                "github.com",
                "medium.com",
                "researchgate.net",
                "orcid.org",
                "scholar.google.com"
            };

            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();

                foreach (var platform in platforms.Take(8))
                {
                    try
                    {
                        var query = HttpUtility.UrlEncode($"site:{platform} \"{name}\"");
                        var url = $"https://www.google.com/search?q={query}&num=5";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.g");
                        foreach (var result in searchResults.Take(3))
                        {
                            var titleElement = result.QuerySelector("h3");
                            var linkElement = result.QuerySelector("a[href]");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = CleanGoogleUrl(linkElement.GetAttribute("href"));
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = $"Professional profile on {platform}",
                                        SearchEngine = "Professional Platforms",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 15,
                                        Domain = platform
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(300, 800), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Professional platform search error for {platform}: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private async Task SearchNewsEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            await _rateLimitSemaphore.WaitAsync(cancellationToken);
            try
            {
                var client = GetRandomHttpClient();
                var newsQueries = new[]
                {
                    $"\"{name}\" news",
                    $"\"{name}\" article",
                    $"\"{name}\" interview",
                    $"\"{name}\" press release"
                };

                foreach (var query in newsQueries.Take(2))
                {
                    try
                    {
                        var encodedQuery = HttpUtility.UrlEncode(query);
                        var url = $"https://www.google.com/search?q={encodedQuery}&tbm=nws&num=15";

                        var response = await client.GetStringAsync(url, cancellationToken);
                        var document = await _browsingContext.OpenAsync(req => req.Content(response), cancellationToken);

                        var searchResults = document.QuerySelectorAll("div.SoaBEf, .ftSUBd, .WlydOe");
                        foreach (var result in searchResults.Take(10))
                        {
                            var titleElement = result.QuerySelector("div.MBeuO, h3, .n0jPhd") ?? result.QuerySelector("a");
                            var linkElement = result.QuerySelector("a[href]");
                            var descElement = result.QuerySelector(".GI74Re, .Y3v8qd");

                            if (titleElement != null && linkElement != null)
                            {
                                var href = linkElement.GetAttribute("href");
                                if (IsValidUrl(href))
                                {
                                    results.Add(new SearchResult
                                    {
                                        Title = CleanText(titleElement.TextContent),
                                        Url = href,
                                        Description = CleanText(descElement?.TextContent ?? ""),
                                        SearchEngine = "News Enhanced",
                                        Relevance = CalculateEnhancedRelevance(titleElement.TextContent, name, href) + 10,
                                        Domain = ExtractDomain(href)
                                    });
                                }
                            }
                        }

                        await Task.Delay(_random.Next(800, 1500), cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"News enhanced search error: {ex.Message}");
                    }
                }
            }
            finally
            {
                _rateLimitSemaphore.Release();
            }
        }

        private List<string> GenerateSearchQueries(string name)
        {
            var queries = new List<string>
            {
                $"\"{name}\"",
                $"{name}",
                $"\"{name}\" profile",
                $"\"{name}\" bio",
                $"\"{name}\" about",
                $"{name} site:linkedin.com",
                $"{name} site:facebook.com",
                $"{name} site:twitter.com",
                $"{name} site:instagram.com"
            };

            // Add variations for names with spaces
            if (name.Contains(" "))
            {
                var parts = name.Split(' ');
                if (parts.Length >= 2)
                {
                    queries.Add($"{parts[0]} {parts[parts.Length - 1]}");
                    queries.Add($"\"{parts[0]}\" \"{parts[parts.Length - 1]}\"");
                }
            }

            return queries;
        }

        private HttpClient GetRandomHttpClient()
        {
            var client = _httpClients[_random.Next(_httpClients.Length)];
            var userAgent = _userAgents[_random.Next(_userAgents.Count)];
            
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");

            return client;
        }

        private int CalculateEnhancedRelevance(string text, string searchName, string url)
        {
            var score = 0;
            var lowerText = text.ToLower();
            var lowerName = searchName.ToLower();
            var lowerUrl = url.ToLower();

            // Exact match in title (high score)
            if (lowerText.Contains(lowerName))
                score += 100;

            // Partial match
            var nameWords = lowerName.Split(new char[] { ' ', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var matchedWords = 0;
            foreach (var word in nameWords)
            {
                if (word.Length > 2 && lowerText.Contains(word))
                {
                    matchedWords++;
                    score += 20;
                }
            }

            // Bonus for multiple word matches
            if (nameWords.Length > 1 && matchedWords == nameWords.Length)
                score += 50;

            // URL-based scoring
            if (lowerUrl.Contains(lowerName.Replace(" ", "")))
                score += 80;
            if (lowerUrl.Contains(lowerName.Replace(" ", "-")))
                score += 80;
            if (lowerUrl.Contains(lowerName.Replace(" ", "_")))
                score += 80;

            // Platform-specific bonuses
            if (lowerUrl.Contains("linkedin.com"))
                score += 30;
            if (lowerUrl.Contains("facebook.com"))
                score += 25;
            if (lowerUrl.Contains("twitter.com") || lowerUrl.Contains("x.com"))
                score += 25;
            if (lowerUrl.Contains("instagram.com"))
                score += 20;
            if (lowerUrl.Contains("youtube.com"))
                score += 20;
            if (lowerUrl.Contains("github.com"))
                score += 25;

            // News and professional content bonus
            if (lowerUrl.Contains("news") || lowerUrl.Contains("article") || lowerUrl.Contains("press"))
                score += 15;
            if (lowerUrl.Contains("profile") || lowerUrl.Contains("bio") || lowerUrl.Contains("about"))
                score += 10;

            return Math.Max(score, 1);
        }

        private string CleanGoogleUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";
            
            if (url.StartsWith("/url?q="))
            {
                var decoded = HttpUtility.UrlDecode(url.Substring(7));
                var ampIndex = decoded.IndexOf("&");
                if (ampIndex > 0)
                    decoded = decoded.Substring(0, ampIndex);
                return decoded;
            }
            
            return url;
        }

        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            // Remove extra whitespace and special characters
            text = Regex.Replace(text, @"\s+", " ");
            text = text.Trim();
            
            // Remove common prefixes/suffixes
            text = Regex.Replace(text, @"^(\.\.\.|\u2026)", "");
            text = Regex.Replace(text, @"(\.\.\.|\u2026)$", "");
            
            return text;
        }

        private bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return "";
            
            try
            {
                var uri = new Uri(url);
                return $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}".ToLower();
            }
            catch
            {
                return url.ToLower();
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

        private async Task SearchBaiduAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Baidu search implementation
            await Task.CompletedTask;
        }

        private async Task SearchInstagramAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Instagram-specific search
            await Task.CompletedTask;
        }

        private async Task SearchFacebookAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Facebook-specific search
            await Task.CompletedTask;
        }

        private async Task SearchTikTokAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // TikTok-specific search
            await Task.CompletedTask;
        }

        private async Task SearchPinterestAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Pinterest-specific search
            await Task.CompletedTask;
        }

        private async Task SearchBusinessDirectoriesAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Business directory search
            await Task.CompletedTask;
        }

        private async Task SearchAcademicPlatformsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Academic platform search
            await Task.CompletedTask;
        }

        private async Task SearchBlogsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Blog search
            await Task.CompletedTask;
        }

        private async Task SearchPodcastsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Podcast search
            await Task.CompletedTask;
        }

        private async Task SearchForumsEnhancedAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Enhanced forum search
            await Task.CompletedTask;
        }

        private async Task SearchStackOverflowAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // StackOverflow search
            await Task.CompletedTask;
        }

        private async Task SearchQuoraAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Quora search
            await Task.CompletedTask;
        }

        private async Task SearchImagePlatformsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Image platform search
            await Task.CompletedTask;
        }

        private async Task SearchDirectoriesAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Directory search
            await Task.CompletedTask;
        }

        private async Task SearchPublicRecordsAsync(string name, ConcurrentBag<SearchResult> results, CancellationToken cancellationToken)
        {
            // Public records search (where legally accessible)
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            foreach (var client in _httpClients)
            {
                client?.Dispose();
            }
            _rateLimitSemaphore?.Dispose();
            _browsingContext?.Dispose();
        }
    }
}