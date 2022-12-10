using Lottery_2022.Models;
using Lottery_2022.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Headers;

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
        #endregion constructor

        #region actions
        //Display current jackpot & amount of players on index page
        public IActionResult Index(IndexViewModel model)
        {
            model = _gameService.GetCurrentDrawData();
            return View(model);
        }
        //Get short GUID code and request DB to display personnalized results page
        [HttpGet("code")]
        [AllowAnonymous]
        public IActionResult GetResultsWithCode(string code)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Session", "Home");
            }
            GetResultsWithCodeViewModel model = _gameService.GetResultsWithCode(code);
            return View(model);
        }

        //Simply redirect to session-grid page
        public IActionResult Session()
        {
            return View();
        }

        //Post selected numbers and insert into DB, then redirect to session summary page with personnalized display
        [HttpGet()]
        [AllowAnonymous]
        [Route("SessionValidation")]
        public IActionResult SessionValidation([FromQuery(Name = "number")] List<int> numbers)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Session", "Home");
            }
            SessionValidationViewModel model = _gameService.ValidateGameSession(numbers);
            return View(model);
        }

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

        // Tool used to add test data to DB (like seed method)
        public IActionResult addData()
        {
            _gameService.AddGenericData();
            return RedirectToAction("Index", "Home");
        }
       



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion actions
    }
}