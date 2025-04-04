using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Data.Infrastructure.Clients.Abstractions;
using Texnokaktus.ProgOlymp.Data.Models;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController(IRegistrationDataServiceClient client) : Controller
{
    public IActionResult Index() => RedirectToAction(nameof(Contest), new { contestId = 1 });

    [Route("[controller]/{contestId:int}")]
    public async Task<IActionResult> Contest(int contestId) =>
        await GetContestRegistrationsAsync(contestId) is { } contestRegistrations
            ? View(contestRegistrations)
            : NotFound();

    [Route("[controller]/{contestId:int}/excel")]
    public async Task<IActionResult> Excel(int contestId)
    {
        if (await GetContestRegistrationsAsync(contestId) is not { } contestRegistrations)
            return NotFound();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add();

        var currentColumn = 1;
        foreach (var (blockName, columns) in Columns)
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
                cell.SetValue(column).IsHeader();
            }
        }

        var currentRow = 3;
        foreach (var registration in contestRegistrations.Registrations)
        {
            var column = 0;
            worksheet.Cell(currentRow, ++column).SetValue(registration.Id);
            worksheet.Cell(currentRow, ++column).SetValue(registration.Uid.ToString());
            worksheet.Cell(currentRow, ++column).SetValue(registration.Created.ToOffset(TimeSpan.FromHours(3)).DateTime);
            worksheet.Cell(currentRow, ++column).SetValue(registration.PersonalDataConsent);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Name.LastName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Name.FirstName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Name.Patronym);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.AgeGroup);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Grade);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.BirthDate.ToDateTime(new(0, 0)));
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Snils).FormatInvalidData(!registration.ParticipantData.IsSnilsValid);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Email);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.School);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParticipantData.Region);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParentData.Name.LastName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParentData.Name.FirstName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParentData.Name.Patronym);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParentData.Email);
            worksheet.Cell(currentRow, ++column).SetValue(registration.ParentData.Phone);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.Name.LastName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.Name.FirstName);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.Name.Patronym);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.Email);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.Phone);
            worksheet.Cell(currentRow, ++column).SetValue(registration.TeacherData.School);

            currentRow++;
        }

        worksheet.Columns().AdjustToContents();

        var stream = new MemoryStream();
        workbook.SaveAs(stream);

        if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);

        return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"registrations-{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(3)):s}.xlsx");
    }

    private async Task<ContestRegistrations?> GetContestRegistrationsAsync(int contestId)
    {
        if (await client.GetRegistrationsAsync(contestId) is not { } contestRegistrations)
            return null;

        return new(contestId,
                   contestRegistrations.Contest.Name,
                   contestRegistrations.Registrations.Select(registration => registration.MapRegistration()));
    }

    private static readonly IEnumerable<ColumnBlock> Columns =
    [
        new(null, ["ID регистрации", "UID", "Время регистрации", "Согласие на обработку ПД"]),
        new("Участник", [ "Фамилия", "Имя", "Отчество", "Возрастная группа", "Класс", "Дата рождения", "СНИЛС", "Email", "ОУ", "Регион" ]),
        new("Родитель", [ "Фамилия", "Имя", "Отчество", "Email", "Телефон" ]),
        new("Наставник", [ "Фамилия", "Имя", "Отчество", "Email", "Телефон", "ОУ" ]),
    ];
}

internal record ColumnBlock(string? Name, string[] Columns);

file static class ExcelExtensions
{
    public static IXLCell IsHeader(this IXLCell cell)
    {
        cell.Style.Font.Bold = true;
        return cell;
    }

    public static IXLCell FormatInvalidData(this IXLCell cell, bool condition)
    {
        if (condition)
        {
            cell.Style.Font.FontColor = XLColor.DarkRed;
            cell.Style.Fill.BackgroundColor = XLColor.Pink;
        }

        return cell;
    }
}

file static class MappingExtensions
{
    public static Registration MapRegistration(this Common.Contracts.Grpc.Data.Registration registration) =>
        new(registration.Id,
            new(registration.Uid.ToByteArray()),
            registration.User.MapUser(),
            registration.Created.ToDateTimeOffset(),
            registration.ParticipantData.MapParticipantData(),
            registration.ParentData.MapParentData(),
            registration.TeacherData.MapTeacherData(),
            registration.PersonalDataConsent);

    private static User MapUser(this Common.Contracts.Grpc.Data.User user) =>
        new(user.Id,
            user.Login,
            user.DisplayName,
            user.AvatarId);

    private static ParticipantData MapParticipantData(this Common.Contracts.Grpc.Data.ParticipantData participantData) =>
        new(participantData.Name.MapName(),
            DateOnly.FromDateTime(participantData.BirthDate.ToDateTime().ToUniversalTime()),
            participantData.Snils,
            participantData.IsSnilsValid,
            participantData.Email,
            participantData.School,
            participantData.Region,
            participantData.Grade);

    private static ParentData MapParentData(this Common.Contracts.Grpc.Data.ParentData parentData) =>
        new(parentData.Name.MapName(),
            parentData.Email,
            parentData.Phone);

    private static TeacherData MapTeacherData(this Common.Contracts.Grpc.Data.TeacherData parentData) =>
        new(parentData.Name.MapName(),
            parentData.Email,
            parentData.Phone,
            parentData.School);

    private static Name MapName(this Common.Contracts.Grpc.Data.Name name) =>
        new(name.FirstName, name.LastName, name.Patronym);
}
