using ClosedXML.Excel;
using Texnokaktus.ProgOlymp.Data.Models;
using Texnokaktus.ProgOlymp.Data.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Services;

internal class ExcelService : IExcelService
{
    private static readonly IEnumerable<ColumnBlock> Columns =
    [
        new(null, ["ID регистрации", "UID", "Время регистрации", "Согласие на обработку ПД"]),
        new("Участник", [ "Фамилия", "Имя", "Отчество", "Возрастная группа", "Класс", "Дата рождения", "СНИЛС", "Email", "ОУ", "Регион" ]),
        new("Родитель", [ "Фамилия", "Имя", "Отчество", "Email", "Телефон" ]),
        new("Наставник", [ "Фамилия", "Имя", "Отчество", "Email", "Телефон", "ОУ" ]),
    ];

    public Stream GenerateExcel(IEnumerable<Registration> registrations)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add();

        var currentColumn = 1;
        foreach (var (blockName, columnNames) in Columns)
        {
            if (blockName is not null)
            {
                var topCell = worksheet.Cell(1, currentColumn);
                topCell.SetValue(blockName).IsHeader();
                topCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Range(topCell, worksheet.Cell(1, currentColumn + columnNames.Length - 1)).Merge();
            }
            
            foreach (var column in columnNames)
            {
                var cell = worksheet.Cell(2, currentColumn++);
                cell.SetValue(column).IsHeader();
            }
        }

        var currentRow = 3;
        foreach (var registration in registrations)
        {
            foreach (var (cell, (cellValue, cellAction)) in GenerateColumns(worksheet, currentRow).Zip(GenerateCells(registration)))
                cell.SetValue(cellValue).Apply(cellAction);

            currentRow++;
        }

        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);

        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static IEnumerable<IXLCell> GenerateColumns(IXLWorksheet worksheet, int currentRow)
    {
        var column = 1;

        do
        {
            yield return worksheet.Cell(currentRow, column);
        } while (++column <= XLHelper.MaxColumnNumber);
    }

    private static IEnumerable<(XLCellValue cellValue, Action<IXLCell>? cellAction)> GenerateCells(Registration registration)
    {
        yield return (registration.Id, null);
        yield return (registration.Uid.ToString(), null);
        yield return (registration.Created.ToOffset(TimeSpan.FromHours(3)).DateTime, null);
        yield return (registration.PersonalDataConsent, null);
        yield return (registration.ParticipantData.Name.LastName, null);
        yield return (registration.ParticipantData.Name.FirstName, null);
        yield return (registration.ParticipantData.Name.Patronym, null);
        yield return (registration.ParticipantData.AgeGroup is { } ageGroup ? $"{ageGroup.StartGrade}\u2013{ageGroup.EndGrade} класс" : null, null);
        yield return (registration.ParticipantData.Grade, null);
        yield return (registration.ParticipantData.BirthDate.ToDateTime(new(0, 0)), null);
        yield return (registration.ParticipantData.Snils, cell => cell.FormatInvalidData(!registration.ParticipantData.IsSnilsValid));
        yield return (registration.ParticipantData.Email, null);
        yield return (registration.ParticipantData.School, null);
        yield return (registration.ParticipantData.Region, null);
        yield return (registration.ParentData.Name.LastName, null);
        yield return (registration.ParentData.Name.FirstName, null);
        yield return (registration.ParentData.Name.Patronym, null);
        yield return (registration.ParentData.Email, null);
        yield return (registration.ParentData.Phone, null);
        yield return (registration.TeacherData.Name.LastName, null);
        yield return (registration.TeacherData.Name.FirstName, null);
        yield return (registration.TeacherData.Name.Patronym, null);
        yield return (registration.TeacherData.Email, null);
        yield return (registration.TeacherData.Phone, null);
        yield return (registration.TeacherData.School, null);
    }

    private record ColumnBlock(string? Name, string[] ColumnNames);
}

file static class Extensions
{
    public static T Apply<T>(this T current, Action<T>? action)
    {
        action?.Invoke(current);
        return current;
    }

    extension(IXLCell cell)
    {
        public IXLCell IsHeader()
        {
            cell.Style.Font.Bold = true;
            return cell;
        }

        public IXLCell FormatInvalidData(bool condition)
        {
            if (!condition)
                return cell;

            cell.Style.Font.FontColor = XLColor.DarkRed;
            cell.Style.Fill.BackgroundColor = XLColor.Pink;

            return cell;
        }
    }
}
