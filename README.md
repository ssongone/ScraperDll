# ScraperDll


## 상세 페이지 스크래핑 속도 개선
기존에 단순 스크래핑 로직을 만들었던 적이 있는데 20개 스크래핑하고 엑셀로 변환하는데 5분 정도 걸렸습니다. 이번에 로직을 짤 땐 병렬로 짜려고 했고 실제로 약 90% 빨라졌습니다.

[![2024-08-29-222814.png](https://i.postimg.cc/x8b68RVs/2024-08-29-222814.png)](https://postimg.cc/wRgXbDvJ)

#### 병렬 처리
``` C#
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
```
PublicationSummary를 Publication으로 바꾸는 로직입니다. 실제로 SummaryToPublication() 함수안에서 대량의 스크래핑 작업이 이루어지고 있습니다. 그것도 잡지냐 책이냐에 따라서 다르게 동작합니다. 

아무튼 비동기 함수인 SummaryToPublication()은 Task\<Publication>이 반환하고 이게 Select문 안에 있기 때문에 결과적으론 IEnumerable\<Task\<Publication>>이 반환됩니다. Task.WhenAll()의 인자로 IEnumerable\<Task\<Publication>>가 들어가고 이때 각각의 Task들이 일괄적으로 실행되고 결과를 받아올 수 있게됩니다.



#### 순차 처리
``` C#
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
```

이건 Task로 따로 빼놓지 않았기 때문에 20개가 하나의 흐름에서 실행되어 속도가 느려집니다.



## 전략패턴 구현
ScraperDll의 메인 기능은 Scraper 클래스에 정의되어 있습니다. 
잡지와 도서 모두 스크래핑 하는 메인로직은 비슷합니다. 페이지 상단에 있는 기본정보를 먼저 스크래핑한 후 그 정보와 페이지 하위 정보를 다시 스크래핑해서 상세페이지 정보를 만듭니다. 하지만 세부적으로 html 태그가 다르거나 출판물정보나 상세페이지 구성이 약간씩 다릅니다. 

원래는 Publication 추상 클래스를 만들고 Book과 Magazine 두가지 하위 클래스를 만들고 함수를 오버라이딩해서 써보려고 시도했으나 이는 우선적으로 Book이나 Magazine 클래스를 만들어야 했습니다. 사실 이 로직을 통해서 Book이나 Magazine 클래스를 만들어야 하는 거니까 모순에 빠졌고 디자인 패턴을 찾아보고 전략패턴을 적용하였습니다.

``` C#
public interface ScrapePolicy
{ 
    public string GenerateListUrl(int option);
    public Publication ConvertSummaryToPublication(PublicationSummary summary, HtmlDocument document);        
    public string GenerateDescriptionTable(Publication publication);
    public string GenerateDescriptionDetail(HtmlDocument document);
}

public class Scraper
{
    private ScrapePolicy scrapePolicy;
    ...

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
}
```

전략패턴을 적용하면서 Scraper 클래스는 세부적으로 어떻게 스크래핑 이루어지는지에는 관심을 두지 않고 메인로직에 집중할 수 있게 되었습니다.

이건 이전에 짰던 자바함수들입니다.

[![before.png](https://i.postimg.cc/TYBc7SWh/dd.png)](https://postimg.cc/GHJY9XK1)

무지성으로 그냥 함수이름으로 구분하고 IF문으로 구분하는 식으로 했었는데 훨씬 나아지긴 한것 같습니다.

