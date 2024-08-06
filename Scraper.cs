using HtmlAgilityPack;
using ScraperDll.Entity;
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
                yield return publication;
            }
        }

        private async Task SummaryToPublication(PublicationSummary summary)
        {
            Publication publication = scrapeService.ConvertSummaryToBook(summary);
        }


        private Publication ConvertSummaryToBookAsync(PublicationSummary summary)
        {
            Publication book = new Publication();

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load(summary.Url);

            string title = book.ScrapeText(document, "p.itemTitle");
            string author = book.ScrapeText(document, "ui.AuthorsName");
            string date = book.ScrapeDate(document);
            string page = book.ScrapeText(document, "div.mainItemTable tr:contains(頁数) td");
            string ISBN = book.ScrapeText(document, "//div[contains(@class, 'mainItemTable')]//tr[td[contains(text(), 'ISBN')]]//span[contains(@class, 'codeSelect')]");
            string price = book.ScrapePrice(document);
            string publisher = book.ScrapeText(document, "div.mainItemTable tr:contains(出版社名) td");
            string mainImageUrl = book.ScrapeMainImageUrl(document);

            book.Title = title;
            book.Author = author;
            book.Date = date;
            book.Page = page;
            book.Publisher = publisher;
            book.MainImageUrl = mainImageUrl;
            book.ISBN = ISBN;
            book.Price = ((int.Parse(price) * ScraperConfig.BOOK_MARGIN) / 10 * 10).ToString();
            book.Description = GenerateDescription(document, book);

            return book;
        }

    }
}
