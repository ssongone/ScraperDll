using HtmlAgilityPack;
using ScraperDll;
using ScraperDll.Entity;
using System.Diagnostics;

namespace ScraperDllTest
{
    public class UnitTest1
    {
        [Fact]
        public async void 랭킹페이지테스트()
        {
            Scraper scraper = new Scraper(new MagazinePolicy());
            List<PublicationSummary> result = await scraper.ScrapeRankingPage(1);

            foreach (PublicationSummary summary in result)
            {
                Debug.WriteLine(summary.Title);
            }
            Assert.Equal(20, result.Count);
        }

        [Fact]
        public async void SummaryToPublication테스트()
        {
            Scraper scraper = new Scraper(new BookPolicy());
            string url = "https://www.e-hon.ne.jp/bec/SA/Detail?refShinCode=0100000000000034450619&Action_id=121&Sza_id=A0";
            HtmlDocument document = await scraper.GetDocumentAsync(url);
            Publication p = await scraper.SummaryToPublication(new PublicationSummary("", url));
            Assert.Equal("成瀬は天下を取りにいく", p.Title);
        }

        [Fact]
        public async void BookDescription테스트()
        {
            Scraper scraper = new Scraper(new BookPolicy());
            string url = "https://www.e-hon.ne.jp/bec/SA/Detail?refShinCode=0100000000000034450619&Action_id=121&Sza_id=A0";
            HtmlDocument document = await scraper.GetDocumentAsync(url);
            Publication p = await scraper.SummaryToPublication(new PublicationSummary("", url));
            Debug.WriteLine(p.Description);
        }

        [Fact]
        public async void MagazineDescription테스트()
        {
            Scraper scraper = new Scraper(new MagazinePolicy());
            string url = "https://www.e-hon.ne.jp/bec/SA/DetailZasshi?refShinCode=0900000004910077010948&Action_id=101&Sza_id=A0";
            HtmlDocument document = await scraper.GetDocumentAsync(url);
            Publication p = await scraper.SummaryToPublication(new PublicationSummary("", url));
            Debug.WriteLine(p.Description);
        }

        [Fact]
        public async void 도서엑셀내보내기()
        {
            Scraper scraper = new Scraper(new BookPolicy());
            List<PublicationSummary> summaries = await scraper.ScrapeRankingPage(1);
            var result = await scraper.ScrapePublicationDetailParallel(summaries);

            ExcelExporter excelExporter = new ExcelExporter(true, 1);
            string path = excelExporter.RegisterAtOnce(result);

            Assert.True(File.Exists(path), $"엑셀 파일이 생성되지 않았습니다: {path}");
        }
    }

    public class PerformanceTest
    {
        [Fact]
        public async void ScrapePublicationDetail순차()
        {
            Scraper scraper = new Scraper(new BookPolicy());
            List<PublicationSummary> summaries = await scraper.ScrapeRankingPage(1);
            var result = await scraper.ScrapePublicationDetail(summaries);
            foreach (Publication r in result)
            {
                Debug.WriteLine(r.Title);
            }
        }

        [Fact]
        public async void ScrapePublicationDetail병렬()
        {
            Scraper scraper = new Scraper(new BookPolicy());
            List<PublicationSummary> summaries = await scraper.ScrapeRankingPage(1);
            var result = await scraper.ScrapePublicationDetailParallel(summaries);
            foreach (Publication r in result)
            {
                Debug.WriteLine(r.Title);
            }
        }
    }
}