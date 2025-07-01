using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XWPF.UserModel;
using PaddleOCRSharp;
using Spire.Presentation;
using System.Text;

namespace FastKMSApi.Core.Service
{
    public class ImpService : IImpService
    {       
        public async Task<Dictionary<string, object>> ExcelAsync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    var keyList = new Dictionary<string, object>();
                    ms.Position = 0;
                    var workbook = WorkbookFactory.Create(ms);

                    var sheet = workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        sheet.GetRow(0).Cells.ForEach(a =>
                        {
                            if (!sheet.IsColumnHidden(a.ColumnIndex))
                            {
                                var key = a.StringCellValue.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
                                if (!string.IsNullOrEmpty(key) && !keyList.Keys.Contains(key))
                                    keyList.Add(key, a.ColumnIndex);
                            }
                        });

                        if (keyList.Count == 0)
                            return result;

                        for (var i = 1; i <= sheet.LastRowNum; i++)
                        {
                            var data = new Dictionary<string, object>();

                            var row = sheet.GetRow(i);
                            if (row == null)
                                continue;

                            if (row.ZeroHeight == true)
                                continue;

                            var cellCount = row.Cells.Count(b => !string.IsNullOrEmpty(b.ToStr().Trim()));
                            if (row.Cells.Count > 0 && cellCount > 0)
                            {
                                foreach (var key in keyList)
                                {
                                    var index = key.Value.ToStr().ToInt(-1);
                                    if (index != -1)
                                    {
                                        var cell = row.GetCell(index);
                                        var value = string.Empty;

                                        if (cell.CellType == CellType.Formula)
                                        {
                                            switch (cell.CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    {
                                                        value = cell.StringCellValue;
                                                        break;
                                                    }
                                                case CellType.Boolean:
                                                    {
                                                        value = cell.BooleanCellValue.ToStr();
                                                        break;
                                                    }
                                                case CellType.Numeric:
                                                    {
                                                        value = cell.NumericCellValue.ToStr();
                                                        break;
                                                    }
                                                case CellType.Error:
                                                    {
                                                        value = string.Empty;
                                                        break;
                                                    }
                                                default:
                                                    {
                                                        value = cell.StringCellValue;
                                                        break;
                                                    }
                                            }
                                        }
                                        else
                                            value = cell.ToStr().Trim();

                                        if (string.IsNullOrEmpty(value))
                                            continue;

                                        if (!data.Keys.Contains(key.Key))
                                            data.Add(key.Key, value);
                                        else
                                            data.Add($"{key.Key}1", value);
                                    }
                                }
                            }

                            result.Add($"text{i}", JsonConvert.SerializeObject(data));
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string, object>> TextAync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    var keyList = new Dictionary<string, object>();
                    ms.Position = 0;

                    var content = new StreamReader(ms, Encoding.Default).ReadToEnd();

                    result = content.JsonToDic(true);
                    if (result.Count > 0)
                        return result;

                    var dics = content.JsonToDics(true);
                    if (dics.Count > 0)
                    {
                        for (var i = 0; i < dics.Count; i++)
                        {
                            result.Add($"text{i}", JsonConvert.SerializeObject(dics[i]));
                        }

                        return result;
                    }

                    result.Add($"text", content);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string,object>> PdfAsync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    ms.Position = 0;
                    var pdf = new PdfDocument(new PdfReader(ms));

                    if (pdf.GetNumberOfPages() == 0)
                        return result;

                    for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
                    {
                        var content = new List<string>();
                        var text = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i));
                        result.Add($"text{i}", text);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string,object>> WordAsync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    ms.Position = 0;
                    var doc = new XWPFDocument(ms);
                    var content = new List<string>();

                    if (doc.Paragraphs.Count == 0)
                        return result;

                    for (int i = 1; i < doc.Paragraphs.Count; i++)
                    {
                        var text = doc.Paragraphs[i].Text;
                        if (!string.IsNullOrEmpty(text))
                            result.Add($"text{i}", text);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string,object>> ImageAsync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    ms.Position = 0;
                    var buff = new byte[ms.Length];
                    ms.Read(buff, 0, (int)ms.Length);

                    var engine = new PaddleOCREngine();
                    var  ocrResult = engine.DetectText(buff);
                    result.Add("text", ocrResult.Text);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }

        public async Task<Dictionary<string, object>> PptAsync(IFormFile file)
        {
            try
            {
                var result = new Dictionary<string, object>();
                if (file == null)
                    return result;

                using (var ms = new MemoryStream())
                {
                    var stream = file.OpenReadStream();
                    await stream.CopyToAsync(ms);

                    ms.Position = 0;

                    Presentation presentation = new Presentation();
                    presentation.LoadFromStream(ms, FileFormat.PPT);
                    var i = 0;
                    foreach (ISlide slide in presentation.Slides)
                    {
                        var content = new List<string>();
                        foreach(var text in slide.GetAllTextFrame())
                        {
                            if (string.IsNullOrEmpty(text.ToStr().Trim()))
                                continue;
                            content.Add(text.ToStr());
                        }

                        if (content.Count == 0)
                            continue;

                        result.Add($"text{i}", string.Join(",",content));
                        i++;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object>();
            }
        }
    }

    public interface IImpService
    {
        Task<Dictionary<string, object>> PptAsync(IFormFile file);
        Task<Dictionary<string, object>> ImageAsync(IFormFile file);
        Task<Dictionary<string, object>> WordAsync(IFormFile file);
        Task<Dictionary<string, object>> TextAync(IFormFile file);
        Task<Dictionary<string, object>> ExcelAsync(IFormFile file);
        Task<Dictionary<string, object>> PdfAsync(IFormFile file);
    }
}