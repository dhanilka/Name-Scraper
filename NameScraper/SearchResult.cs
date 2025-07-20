using System;

namespace NameScraper
{
    public class SearchResult
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SearchEngine { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int Relevance { get; set; } = 0;
        public string ImageUrl { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
    }

    public class SearchProgress
    {
        public int PercentComplete { get; set; }
        public string CurrentAction { get; set; } = string.Empty;
        public string SearchEngine { get; set; } = string.Empty;
        public int ResultsFound { get; set; }
    }
}