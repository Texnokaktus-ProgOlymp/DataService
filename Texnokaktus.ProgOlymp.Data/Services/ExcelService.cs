using ClosedXML.Excel;
using Texnokaktus.ProgOlymp.Data.Models;
using Texnokaktus.ProgOlymp.Data.Services.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Services;

internal class ExcelService : IExcelService
{
    private static readonly IEnumerable<ColumnBlock> ColumnBlocks =
    [
        new(null,
        [
            new("ID регистрации", registration => registration.Id),
            new("UID", registration => registration.Uid.ToString()),
            new("Время регистрации", registration => registration.Created.ToOffset(TimeSpan.FromHours(3)).DateTime),
            new("Согласие на обработку ПД", registration => registration.PersonalDataConsent)
        ]),
        new("Участник",
        [
            new("Фамилия", registration => registration.ParticipantData.Name.LastName),
            new("Имя", registration => registration.ParticipantData.Name.FirstName),
            new("Отчество", registration => registration.ParticipantData.Name.Patronym),
            new("Возрастная группа", registration => registration.ParticipantData.AgeGroup is { } ageGroup ? $"{ageGroup.StartGrade}\u2013{ageGroup.EndGrade} класс" : null),
            new("Класс", registration => registration.ParticipantData.Grade),
            new("Дата рождения", registration => registration.ParticipantData.BirthDate.ToDateTime(new(0, 0))),
            new("СНИЛС", registration => registration.ParticipantData.Snils, (cell, registration) => cell.FormatInvalidData(!registration.ParticipantData.IsSnilsValid)),
            new("Email", registration => registration.ParticipantData.Email),
            new("ОУ", registration => registration.ParticipantData.School),
            new("Регион", registration => registration.ParticipantData.Region)
        ]),
        new("Родитель",
        [
            new("Фамилия", registration => registration.ParentData.Name.LastName),
            new("Имя", registration => registration.ParentData.Name.FirstName),
            new("Отчество", registration => registration.ParentData.Name.Patronym),
            new("Email", registration => registration.ParentData.Email),
            new("Телефон", registration => registration.ParentData.Phone)
        ]),
        new("Наставник",
        [
            new("Фамилия", registration => registration.TeacherData.Name.LastName),
            new("Имя", registration => registration.TeacherData.Name.FirstName),
            new("Отчество", registration => registration.TeacherData.Name.Patronym),
            new("Email", registration => registration.TeacherData.Email),
            new("Телефон", registration => registration.TeacherData.Phone),
            new("ОУ", registration => registration.TeacherData.School)
        ])
    ];

    public Stream GenerateExcel(IEnumerable<Registration> registrations)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add();

        var currentColumn = 1;
        foreach (var (blockName, columns) in ColumnBlocks)
        {
            if (blockName is not null)
            {
                var topCell = worksheet.Cell(1, currentColumn);
                topCell.SetValue(blockName).IsHeader();
                topCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Range(topCell, worksheet.Cell(1, currentColumn + columns.Length - 1)).Merge();
            }

            foreach (var column in columns)
            {
                var cell = worksheet.Cell(2, currentColumn++);
                cell.SetValue(column.Name).IsHeader();
            }
        }

        var currentRow = 3;
        foreach (var registration in registrations)
        {
            foreach (var (cell, (_, cellValueFactory, cellAction)) in GenerateColumns(worksheet, currentRow).Zip(ColumnBlocks.SelectMany(block => block.Columns)))
            {
                cell.SetValue(cellValueFactory.Invoke(registration))
                    .Apply(cellAction is not null
                               ? xlCell => cellAction.Invoke(xlCell, registration)
                               : null);
            }

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

    private record ColumnBlock(string? Name, Column[] Columns);

    private record Column(string Name, Func<Registration, XLCellValue> ValueFactory, Action<IXLCell, Registration>? CellAction = null);
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
