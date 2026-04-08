using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Texnokaktus.ProgOlymp.Data.Models;
using Texnokaktus.ProgOlymp.Data.Options;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController : Controller
{
    public IActionResult Index([FromServices] IOptions<ContestRoutingOptions> routingOptions) =>
        RedirectToAction(nameof(Contest),
                         new
                         {
                             contestName = routingOptions.Value.DefaultContest
                         });
    
    [Route("[controller]/{contestName}")]
    public IActionResult Contest(string contestName) => View(new Contest(contestName));
}
