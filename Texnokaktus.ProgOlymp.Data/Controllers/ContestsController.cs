using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Texnokaktus.ProgOlymp.Data.Options;

namespace Texnokaktus.ProgOlymp.Data.Controllers;

public class ContestsController : Controller
{
    public IActionResult Index([FromServices] IOptions<ContestRoutingOptions> routingOptions) =>
        RedirectToAction(nameof(ParticipantsController.Index), "ParticipantS",
                         new
                         {
                             contestName = routingOptions.Value.DefaultContest
                         });
}
