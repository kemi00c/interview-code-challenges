using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly ILogger<CatalogueController> _logger;
        private readonly ICatalogueRepository _catalogueRepository;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueRepository catalogueRepository)
        {
            _logger = logger;
            _catalogueRepository = catalogueRepository;   
        }

        [HttpGet]
        [Route("GetCatalogue")]
        public IList<BookStock> Get()
        {
            return _catalogueRepository.GetCatalogue();
        }

        [HttpPost]
        [Route("SearchCatalogue")]
        public IList<BookStock> Post(CatalogueSearch search)
        {
            return _catalogueRepository.SearchCatalogue(search);
        }

        [HttpGet]
        [Route("OnLoan")]
        public List<BorrowerOnLoan> OnLoan()
        {
            var borrowersOnLoan = new List<BorrowerOnLoan>();
            var loans = _catalogueRepository.GetCatalogue().Where(b => b.OnLoanTo != null).GroupBy(c => c.OnLoanTo).ToList();
            foreach (var loan in loans)
            {
                var borrowerOnLoan = new BorrowerOnLoan();
                borrowerOnLoan.Borrower = loan.Key;
                borrowerOnLoan.BookTitles = loan.Select(stock => stock.Book.Name).ToList();

                borrowersOnLoan.Add(borrowerOnLoan);
            }

            return borrowersOnLoan;
        }

        [HttpPost]
        [Route("OnLoan")]
        public ActionResult OnLoanReturn(string borrowerName, string bookTitle)
        {
            var bookOnLoan = _catalogueRepository.GetCatalogue().Where(c => c.OnLoanTo?.Name == borrowerName && c.Book.Name == bookTitle).FirstOrDefault();
            if (bookOnLoan == null)
            {
                return NotFound($"{bookTitle} is not on loan to {borrowerName}");
            }

            if (_catalogueRepository.Return(bookOnLoan))
            {
                return Ok();
            }

            return StatusCode(500);
        }
    }
}