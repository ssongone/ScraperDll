using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using ScraperDll.Entity;

public class ExcelExporter
{
    private XSSFWorkbook workbook; // 엑셀 쓰기전 workbook 지정
    private ISheet sheet;

    public void RegisterAtOnce(List<Publication> publications)
    {
        string path = SetFilePath() + "일괄등록" + CalcCurrentTime() + ".xlsx";
        try (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            workbook = new XSSFWorkbook();
            sheet = workbook.CreateSheet("sheet1"); // 맨앞 시트만 쓰니까 지정해줌
            FillExcel(publications);
            workbook.Write(fs); // 작업이 끝난 후 해당 workbook 객체를 FileStream에 쓰기
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("오류오류");
        }
    }

    private void FillExcel(List<Publication> publications)
    {
        for (int i = 0; i < publications.Count; i++)
        {
            CreateCell(i + 4, publications[i]);
        }
    }

    private void TempMethod()
    {
        Publication bookInfo = new Publication();
        CreateCell(3, bookInfo);
    }

    private void CreateCell(int order, Publication publication)
    {
        IRow curRow = sheet.CreateRow(order);

        curRow.CreateCell(ExcelConfig.PRODUCT_CODE_COLUMN).SetCellValue("code");
        curRow.CreateCell(ExcelConfig.CATEGORY_CODE_COLUMN).SetCellValue(ExcelConfig.CATEGORY_CODE_VALUE);
        curRow.CreateCell(ExcelConfig.PRODUCT_NAME_COLUMN).SetCellValue(publication.Title);
        curRow.CreateCell(ExcelConfig.PRODUCT_PRICE_COLUMN).SetCellValue(publication.Price);
        curRow.CreateCell(ExcelConfig.PRODUCT_MAIN_IMAGE_COLUMN).SetCellValue(publication.MainImageUrl);
        curRow.CreateCell(ExcelConfig.PRODUCT_DESCRIPTION_COLUMN).SetCellValue(publication.Description);
        curRow.CreateCell(ExcelConfig.PRODUCT_STOCK_COLUMN).SetCellValue(ExcelConfig.PRODUCT_STOCK_VALUE);

        curRow.CreateCell(ExcelConfig.ORIGIN_CODE_COLUMN).SetCellValue(ExcelConfig.ORIGIN_CODE_VALUE);
        curRow.CreateCell(ExcelConfig.IMPORTER_COLUMN).SetCellValue(ExcelConfig.N_VALUE);
        curRow.CreateCell(ExcelConfig.MINOR_COLUMN).SetCellValue(ExcelConfig.Y_VALUE);
        curRow.CreateCell(ExcelConfig.DELIVERY_CODE_COLUMN).SetCellValue(ExcelConfig.DELIVERY_CODE_VALUE);

        curRow.CreateCell(ExcelConfig.AS_NUMBER_COLUMN).SetCellValue(ExcelConfig.AS_NUMBER_VALUE);
        curRow.CreateCell(ExcelConfig.AS_INFO_COLUMN).SetCellValue(ExcelConfig.AS_NUMBER_VALUE);

        // ISBN
        // curRow.CreateCell(ExcelConfig.ISBN_COLUMN).SetCellValue("978-4-86593-532-5");
        curRow.CreateCell(77).SetCellValue(ExcelConfig.Y_VALUE); // 해당 열은 명시되어 있지 않으므로 임의로 값을 넣었습니다.
        curRow.CreateCell(ExcelConfig.PUBLICATION_DATE_COLUMN).SetCellValue(publication.Date);
        curRow.CreateCell(ExcelConfig.PUBLISHER_COLUMN).SetCellValue(publication.Publisher);
        curRow.CreateCell(ExcelConfig.WRITER_COLUMN).SetCellValue(ExcelConfig.ILLUSTRATOR_VALUE);
        curRow.CreateCell(ExcelConfig.ILLUSTRATOR_COLUMN).SetCellValue(ExcelConfig.ILLUSTRATOR_VALUE);
        curRow.CreateCell(ExcelConfig.INCOME_DEDUCTION_COLUMN).SetCellValue(ExcelConfig.Y_VALUE);
    }

    private string SetFilePath()
    {
        string osName = Environment.OSVersion.Platform.ToString().ToLower();

        if (osName.Contains("win"))
            return ExcelConfig.WINDOWS_FILE_PATH;
        else
            return ExcelConfig.MAC_FILE_PATH;
    }

    private string CalcCurrentTime()
    {
        return DateTime.Now.ToString("yyMMddHHmmss");
    }
}
