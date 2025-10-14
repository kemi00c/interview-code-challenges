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
        private readonly IBookRepository _bookRepository;
        private readonly IBorrowerRepository _borrowerRepository;

        public CatalogueController(ILogger<CatalogueController> logger, ICatalogueRepository catalogueRepository, IBookRepository bookRepository, IBorrowerRepository borrowerRepository)
        {
            _logger = logger;
            _catalogueRepository = catalogueRepository;
            _bookRepository = bookRepository;
            _borrowerRepository = borrowerRepository;
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

        [HttpPost]
        [Route("Reserve")]
        public ActionResult ReserveBook(string borrowerName, string bookTitle, DateOnly dueDate)
        {
            var borrower = _borrowerRepository.GetBorrowers().Where(b => b.Name == borrowerName).FirstOrDefault();
            var book = _bookRepository.GetBooks().Where(b => b.Name == bookTitle).FirstOrDefault();

            if (borrower == null)
            {
                return BadRequest($"No registered borrower with name {borrowerName} exists.");
            }

            if (book == null)
            {
                return BadRequest($"No book with title {bookTitle} exists.");
            }


            var dueDateTime = new DateTime(dueDate, new TimeOnly());

            if (dueDateTime.Date <= DateTime.Now.Date)
            {
                return BadRequest("Due date is in the past.");
            }

            var freeStock = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle && c.OnLoanTo == null).FirstOrDefault();
            if (freeStock != null)  // If there is free stock of the particular title, the borrower can lean it.
            {
                if (!_catalogueRepository.Reserve(freeStock, borrowerName, dueDateTime))
                {
                    return StatusCode(500);
                }
            }
            else   // If there is no free stock, we have to find the next available date
            {
                var lastLoan = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle).OrderByDescending(c => c.LoanEndDate).FirstOrDefault();

                if (lastLoan == null)
                {
                    return StatusCode(500);
                }

                var lastEndDate = lastLoan.LoanEndDate;

                if (dueDateTime <= lastEndDate)
                {
                    return BadRequest($"{bookTitle} will not be available until {dueDate}. Its first available date is {lastEndDate}.");
                }

                if (!_catalogueRepository.Reserve(lastLoan, borrowerName, dueDateTime))
                {
                    return StatusCode(500);
                }

            }

            return Ok();
        }

        [HttpGet]
        [Route("CheckAvailablity")]
        public ActionResult CheckAvailability(string borrowerName, string bookTitle)
        {
            var borrower = _borrowerRepository.GetBorrowers().Where(b => b.Name == borrowerName).FirstOrDefault();
            var book = _bookRepository.GetBooks().Where(b => b.Name == bookTitle).FirstOrDefault();

            if (borrower == null)
            {
                return BadRequest($"No registered borrower with name {borrowerName} exists.");
            }

            if (book == null)
            {
                return BadRequest($"No book with title {bookTitle} exists.");
            }

            var activeReserve = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle && c.OnLoanTo != null && c.OnLoanTo.Name == borrowerName && c.LoanEndDate > DateTime.Now.Date).FirstOrDefault();
            if (activeReserve != null)
            {
                var previousReserve = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle && c.LoanEndDate < activeReserve.LoanEndDate).OrderByDescending(c => c.LoanEndDate).FirstOrDefault();
                if (previousReserve == null || previousReserve.LoanEndDate <= DateTime.Now)    // if no previous reserve, or the previous end date is in the past the book is available with the current date.
                {
                    return Ok(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                }

                var previousEndDate = previousReserve.LoanEndDate.Value;

                return Ok(new DateOnly(previousEndDate.Year, previousEndDate.Month, previousEndDate.Day));  // If the book is currently on loan to someone else, the next available date is the previous end date.
            }
            else   // If no active reserve, check if there is a free copy not in loan
            {
                var freeStock = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle && c.OnLoanTo == null && c.LoanEndDate == null).FirstOrDefault();
                if (freeStock != null)  // If there is a free stock instance, it's available now.
                {
                    return Ok(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                }
                // If no free stock, check the previous reserve for availability
                var previousReserve = _catalogueRepository.GetCatalogue().Where(c => c.Book.Name == bookTitle).OrderByDescending(c => c.LoanEndDate).FirstOrDefault();

                if (previousReserve == null || previousReserve.LoanEndDate == null || previousReserve.LoanEndDate <= DateTime.Now.Date)    // If no previous reserve, or the end date is in the past the book is available now.
                {
                    return Ok(new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                }

                var previousEndDate = previousReserve.LoanEndDate.Value;
                return Ok(new DateOnly(previousEndDate.Year, previousEndDate.Month, previousEndDate.Day));
            }
        }
    }
}