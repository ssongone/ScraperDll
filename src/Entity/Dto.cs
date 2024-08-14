namespace ScraperDll.Entity
{
    public class PublicationSummary
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public PublicationSummary() { }

        public PublicationSummary(string title, string url)
        {
            Title = title;
            Url = url;
        }
    }
}
