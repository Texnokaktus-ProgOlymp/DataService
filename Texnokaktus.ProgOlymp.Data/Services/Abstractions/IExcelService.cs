using Texnokaktus.ProgOlymp.Data.Models;

namespace Texnokaktus.ProgOlymp.Data.Services.Abstractions;

public interface IExcelService
{
    public Stream GenerateExcel(IEnumerable<Registration> registrations);
}
