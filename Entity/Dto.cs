namespace ScraperDll.Entity
{
    public class PublicationSummary
    {
        public string Name { get; set; }
        public string Url { get; set; }


        public PublicationSummary(string name, string url)
        {
            Name = name;
            Url = url;
        }
    }
}
