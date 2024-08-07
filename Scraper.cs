using HtmlAgilityPack;
using ScraperDll.Entity;
using System;
using System.Reflection.Metadata;
using System.Text;

namespace ScraperDll
{
    public class Scraper
    {
        private static readonly HttpClient client = new HttpClient();
        private ScrapeService scrapeService;

        public Scraper(ScrapeService scrapeService) 
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // 인코딩 등록
            this.scrapeService = scrapeService;
        }

        public async Task<List<PublicationSummary>> ScrapeRankingPage(int option)
        {
            string url = scrapeService.GenerateListUrl(option);
            HtmlDocument document = await GetDocumentAsync(url);
            
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
        private async Task<HtmlDocument> GetDocumentAsyncWithNew(string url)
        {
            HttpClient c = new HttpClient();
            HttpResponseMessage response = await c.GetAsync(url);
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

        private async Task<HtmlDocument> GetDocumentAsync(string url)
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

        public async Task<List<Publication>> ScrapePublicationDetail(List<PublicationSummary> summaries)
        {
            var publications = new List<Publication>(summaries.Count);

            await foreach (var publication in ProcessSummariesAsync(summaries))
            {
                publications.Add(publication);
            }

            return publications;
        }

        private async IAsyncEnumerable<Publication> ProcessSummariesAsync(IEnumerable<PublicationSummary> summaries)
        {
            foreach (var summary in summaries)
            {
                var publication = await SummaryToPublication(summary);
                //var publication = await SummaryToPublicationAsync(summary);
                yield return publication;
            }
        }
        private async Task<Publication> SummaryToPublication(PublicationSummary summary)
        {
            HtmlDocument document = await GetDocumentAsyncWithNew(summary.Url);

            Publication publication = scrapeService.ConvertSummaryToPublication(summary, document);
            GenerateDescription(publication, document);
            return publication;
        }


        private async Task<Publication> SummaryToPublicationAsync(PublicationSummary summary)
        {
            using HttpClient httpClient = new HttpClient();
            var htmlContent = await httpClient.GetStringAsync(summary.Url);
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            Publication publication = scrapeService.ConvertSummaryToPublication(summary, document);
            GenerateDescription(publication, document);
            return publication;
        }

        private void GenerateDescription(Publication publication, HtmlDocument document)
        {
            String description = CreateImgTag(publication.MainImageUrl);
            //description += scrapeService.GenerateDescriptionTable(publication);
            description += "<br><br>";
            //description += scrapeService.GenerateDescriptionDetail(document);

            description += ScraperConfig.DEFAULT_IMAGE_URL;
            publication.Description = description;
        }

        public string CreateImgTag(string imgUrl)
        {
            return "<img src=\"" + imgUrl + "\">";
        }

    }
}
