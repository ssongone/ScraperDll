using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using ScraperDll.Entity;
using System.Text;
using Org.BouncyCastle.Bcpg;
using System.Diagnostics;

public class ExcelExporter
{
    private XSSFWorkbook workbook; // 엑셀 쓰기전 workbook 지정
    private ISheet sheet;
    public bool IsBook {  get; set; }
    public int Option {  get; set; }

    private string code;
    private string today = DateTime.Now.ToString("yyMMdd_");

    public ExcelExporter(bool isBook, int option) 
    {
        IsBook = isBook;
        Option = option;
        code = GenerateCode();
    }

    public string RegisterAtOnce(List<Publication> publications)
    {
        string path = ExcelConfig.WINDOWS_FILE_PATH + "일괄등록_" + code + "_" + CalcCurrentTime() + ".xlsx";

        if (!Directory.Exists(ExcelConfig.WINDOWS_FILE_PATH))
        {
            Directory.CreateDirectory(ExcelConfig.WINDOWS_FILE_PATH);
        }

        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                workbook = new XSSFWorkbook();
                sheet = workbook.CreateSheet("sheet1");
                FillExcel(publications);
                workbook.Write(fs);
            }
        }
        catch (IOException e)
        {
            Debug.WriteLine(e.Message);
        }

        return path;
    }

    private void FillExcel(List<Publication> publications)
    {
        sheet.CreateRow(0).CreateCell(0).SetCellValue(0);
        sheet.CreateRow(1).CreateCell(0).SetCellValue(0);

        for (int i = 0; i < publications.Count; i++)
        {
            CreateCell(i + 4, publications[i]);
        }
    }

    private void CreateCell(int order, Publication publication)
    {
        IRow curRow = sheet.CreateRow(order);

        curRow.CreateCell(ExcelConfig.PRODUCT_CODE_COLUMN).SetCellValue(today+code);
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

        curRow.CreateCell(ExcelConfig.ISBN_COLUMN).SetCellValue("978-4-86593-532-5");
        curRow.CreateCell(77).SetCellValue(ExcelConfig.Y_VALUE); // 해당 열은 명시되어 있지 않으므로 임의로 값을 넣었습니다.
        curRow.CreateCell(ExcelConfig.PUBLICATION_DATE_COLUMN).SetCellValue(publication.Date);
        curRow.CreateCell(ExcelConfig.PUBLISHER_COLUMN).SetCellValue(publication.Publisher);
        curRow.CreateCell(ExcelConfig.WRITER_COLUMN).SetCellValue(ExcelConfig.ILLUSTRATOR_VALUE);
        curRow.CreateCell(ExcelConfig.ILLUSTRATOR_COLUMN).SetCellValue(ExcelConfig.ILLUSTRATOR_VALUE);
        curRow.CreateCell(ExcelConfig.INCOME_DEDUCTION_COLUMN).SetCellValue(ExcelConfig.Y_VALUE);
    }

    private string CalcCurrentTime()
    {
        return DateTime.Now.ToString("yyMMdd_HHmmss");
    }

    private string GenerateCode()
    {
        string prefix = IsBook ? "도서_" : "_잡지_";
        string optionText;

        if (IsBook)
        {
            switch (Option)
            {
                case 1:
                    optionText = "소설에세이";
                    break;
                case 2:
                    optionText = "논픽션교양";
                    break;
                case 3:
                    optionText = "문고신서";
                    break;
                case 4:
                    optionText = "사심역교";
                    break;
                case 5:
                    optionText = "비경사";
                    break;
                default:
                    optionText = "기타";
                    break;
            }
        }
        else
        {
            switch (Option)
            {
                case 1:
                    optionText = "1-20";
                    break;
                case 2:
                    optionText = "21-40";
                    break;
                case 3:
                    optionText = "41-60";
                    break;
                case 4:
                    optionText = "61-80";
                    break;
                case 5:
                    optionText = "81-100";
                    break;
                default:
                    optionText = "기타";
                    break;
            }
        }

        return prefix+optionText;
    }

}
