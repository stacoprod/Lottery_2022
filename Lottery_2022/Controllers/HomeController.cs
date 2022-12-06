using Lottery_2022.Models;
using Lottery_2022.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;




namespace Lottery_2022.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGameService _gameService;

        #region constructor
        public HomeController(ILogger<HomeController> logger, IGameService gameService)
        {
            _logger = logger;
            _gameService = gameService;
        }
        #endregion

        #region actions
        [HttpGet]
        public IActionResult Index(IndexViewModel model)
        {
            model = _gameService.GetCurrentDrawData();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetResultsWithCode()
        {   
            var model = new GetResultsWithCodeViewModel()
            {
                ShortGuid = "9999999999999999999999"
            };
            return RedirectToAction("GameResults", "Home");
        }

        public IActionResult Session()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ValidateSession(ValidateSessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Session", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult SessionSummary(SessionSummaryViewModel model)
        {
            model = _gameService.GetSessionData();
            return View(model);
        }

        [HttpGet]
        public IActionResult GameResults(GameResultsViewModel model)
        {
            model = _gameService.GetGameResults();
            return View(model);
        }

        public IActionResult GuidInformation()
        {
            return View();
        }

        #endregion



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}