using Microsoft.AspNetCore.Mvc;
using Texnokaktus.ProgOlymp.Data.DataAccess.Repositories.Abstractions;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController(IContestRepository contestRepository) : Controller
{
    public async Task<IActionResult> Index()
    {
        var contests = await contestRepository.GetAllAsync();
        return View(contests);
    }

    [Route("[controller]/{contestId:int}")]
    public async Task<IActionResult> Contest(int contestId)
    {
        var contest = await contestRepository.GetAsync(contestId);
        return contest is not null ?
                   View(contest) :
                   NotFound();
    }
}
