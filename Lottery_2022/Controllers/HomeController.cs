using Lottery_2022.Models;
using Lottery_2022.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lottery_2022.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIndexService _indexService;
        private readonly IGameService _gameService;
        private readonly IResultService _resultService;
        private readonly ITestService _testService;

        #region constructor
        public HomeController(ILogger<HomeController> logger, IIndexService indexService, IGameService gameService, IResultService resultService, ITestService testService)
        {
            _logger = logger;
            _indexService = indexService;
            _gameService = gameService;
            _resultService = resultService;
            _testService = testService;
        }
        #endregion constructor

        #region index page
        //Display current jackpot & amount of players on index page
        [AllowAnonymous]
        public IActionResult Index(IndexViewModel model)
        {
            model = _indexService.GetCurrentDrawData();
            return View(model);
        }
        #endregion
        #region results page
        //Get short GUID code and request DB to display personnalized results page
        [HttpPost]
        public IActionResult GetResultsWithCode(string code)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Session", "Home");
            }
            GetResultsWithCodeViewModel model = _resultService.GetResultsWithCode(code);
            return View(model);
        }
        #endregion
        #region game page
        //Simply redirect to session-grid page
        [AllowAnonymous]
        public IActionResult Session()
        {
            return View();
        }
        #endregion
        #region record session job
        //Post selected numbers and insert into DB, then redirect to session validation page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecordSession(List<int> numbers)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Session", "Home");
            }
            SessionValidationViewModel model = _gameService.ValidateGameSession(numbers);
            // TempData["datacontainer"] = model; //ajouté pour régler problème URL
            return RedirectToAction("SessionValidation", model);

        }
        #endregion
       [HttpGet]
        #region session validation page
        //Display the summary of game session to user
        public IActionResult SessionValidation(SessionValidationViewModel model)
        {
            return View( model);
        }
        #endregion
        #region information page
        //Give more information about short GUID and how it's used to record session and find results
        public IActionResult GuidInformations()
        {
            return View();
        }
        //Give more information about the draw and how the gains are calculated
        public IActionResult GameInformations()
        {
            return View();
        }
        //Give more details about RGPD and conditions of use
        public IActionResult LegalInformations()
        {
            return View();
        }
        #endregion
        #region error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
        #region test data & seed insert
        // Tool used to add test data to DB (like seed method)
        public IActionResult addData()
        {
            _testService.AddGenericData();
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}