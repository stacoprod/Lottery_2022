using Lottery_2022.Models;
using Lottery_2022.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Lottery_2022.Controllers
{
    public class HomeController : Controller
    {
        #region constructor / properties
        private readonly IIndexService _indexService;
        private readonly IGameService _gameService;
        private readonly IResultService _resultService;
        private readonly ITestService _testService;

        public HomeController(IIndexService indexService, IGameService gameService, IResultService resultService, ITestService testService)
        {
            _indexService = indexService;
            _gameService = gameService;
            _resultService = resultService;
            _testService = testService;
        }
        #endregion

        #region index page
        //Displays current jackpot & amount of players on index page:
        [AllowAnonymous]
        public IActionResult Index()
        {
            IndexViewModel model = _indexService.GetCurrentDrawData();
            return View(model);
        }
        #endregion

        #region results page
        //Gets short GUID code and request DB to display personnalized results page:
        [HttpPost]
        public IActionResult GetResultsWithCode(string code)
        {
            if (!ModelState.IsValid)
            {
                return View(code);
            }
            else
            {
                GetResultsWithCodeViewModel model = _resultService.GetResultsWithCode(code);

                if (model.ErrorMessage == null)
                {
                    // Success
                    return View(model);
                }
                else
                {
                    // 404
                    ErrorsViewModel errorModel = new ErrorsViewModel()
                    {
                        Status = "404",
                        ErrorMessage = model.ErrorMessage
                    };
                    return RedirectToAction("Errors", model);
                }
                    
            }
        }
        #endregion

        #region game page
        //Simply redirects to new session page:
        [AllowAnonymous]
        public IActionResult Session()
        {
            return View();
        }
        #endregion
        
        #region record session job
        //Posts selected numbers and insert into DB, then redirect to session validation page:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecordSession(List<int> numbers)
        {
            if (!ModelState.IsValid)
            {
                return View(numbers);
            }
            else
            {
                SessionValidationViewModel model = _gameService.ValidateGameSession(numbers);

                if (model.ErrorMessage == null)
                {
                    // Success
                    return RedirectToAction("SessionValidation", model);
                }   
                else
                {
                    // 404
                    ErrorsViewModel errorModel = new ErrorsViewModel()
                    {
                        Status = "404",
                        ErrorMessage = model.ErrorMessage
                    };
                    return RedirectToAction("Errors", model);
                }        
            }
        }
        #endregion

        #region session validation page
        //Displays the summary of game session to user:
        [HttpGet]
        public IActionResult SessionValidation(SessionValidationViewModel model)
        {
            return View( model);
        }
        #endregion

        #region information page
        //Gives more informations about game rules, gains, short guid and legals
        public IActionResult Informations()
        {
            return View();
        }
        #endregion

        #region error page
        // Display a personnalized message to user in case of wrong request:
        [HttpGet]
        public IActionResult Errors(ErrorsViewModel errorMessages)
        {
            return View(errorMessages);
        }
        #endregion

        #region test data & seed insert
        // Tool used to add test data to DB (like a seed method)
        public IActionResult AddData()
        {
            _testService.AddGenericData();
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}