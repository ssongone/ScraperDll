using HtmlAgilityPack;
using ScraperDll.Entity;
using System;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace ScraperDll
{
    public class Scraper
    {
        private HttpClient client = new HttpClient();
        private ScrapePolicy scrapePolicy;

        public Scraper(ScrapePolicy scrapePolicy) 
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 인코딩 등록
            this.scrapePolicy = scrapePolicy;
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml,text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36 OPR/67.0.3575.137");
            client.DefaultRequestHeaders.TryAddWithoutValidation("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Connection", "keep-alive");
        }

        public async Task<List<PublicationSummary>> ScrapeRankingPage(int option)
        {
            string url = scrapePolicy.GenerateListUrl(option);
            HtmlDocument document = await GetDocumentAsync(url);
            Debug.WriteLine(document);
            // nodes null이면 에러 처리
            var nodes = document.DocumentNode.SelectNodes("//div[@id='contents']//div[@class='detail']//p[@class='title']//a");

            var result = new List<PublicationSummary>();

            foreach (var node in nodes) 
            {
                string name = node.InnerText;
                string hrefValue = node.GetAttributeValue("href", string.Empty);
                result.Add(new PublicationSummary(name, hrefValue));
            }

            return result;
        }

        public async Task<HtmlDocument> GetDocumentAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var contentStream = await response.Content.ReadAsStreamAsync();

            using (var reader = new StreamReader(contentStream, Encoding.GetEncoding("shift_jis")))
            {
                string pageContents = await reader.ReadToEndAsync();
                var document = new HtmlDocument();
                document.LoadHtml(pageContents);
                return document;
            }
        }

        public async Task<List<Publication>> ScrapePublicationDetailParallel(List<PublicationSummary> summaries)
        {
            var publications = new List<Publication>(summaries.Count);

            var tasks = summaries.Select(async summary =>
            {
                var publication = await SummaryToPublication(summary);
                return publication;
            });

            var results = await Task.WhenAll(tasks);

            publications.AddRange(results);

            return publications;
        }


        public async Task<List<Publication>> ScrapePublicationDetail(List<PublicationSummary> summaries)
        {
            var publications = new List<Publication>(summaries.Count);

            await foreach (var publication in ProcessSummariesAsync(summaries))
            {
                publications.Add(publication);
            }

            return publications;
        }

        public async IAsyncEnumerable<Publication> ProcessSummariesAsync(IEnumerable<PublicationSummary> summaries)
        {
            foreach (var summary in summaries)
            {
                Debug.WriteLine(summary.Url);
                var publication = await SummaryToPublication(summary);
                yield return publication;
            }
        }
        public async Task<Publication> SummaryToPublication(PublicationSummary summary)
        {
            HtmlDocument document = await GetDocumentAsync(summary.Url);
            Debug.WriteLine(document.DocumentNode.OuterHtml);
            Publication publication = scrapePolicy.ConvertSummaryToPublication(summary, document);
            GenerateDescription(publication, document);
            return publication;
        }

        private void GenerateDescription(Publication publication, HtmlDocument document)
        {
            String description = CreateImgTag(publication.MainImageUrl);
            description += "<br><br>";
            description += scrapePolicy.GenerateDescriptionTable(publication);
            description += "<br><br>";
            description += scrapePolicy.GenerateDescriptionDetail(document);
            description += "<br><br>";

            description += CreateImgTag(ScraperConfig.DEFAULT_IMAGE_URL);
            publication.Description = description;
        }

        public string CreateImgTag(string imgUrl)
        {
            return "<img src=\"" + imgUrl + "\">";
        }

    }
}
