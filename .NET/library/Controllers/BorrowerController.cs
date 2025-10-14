using Microsoft.AspNetCore.Mvc;
using OneBeyondApi.DataAccess;
using OneBeyondApi.Model;
using System.Collections;

namespace OneBeyondApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BorrowerController : ControllerBase
    {
        private readonly ILogger<BorrowerController> _logger;
        private readonly IBorrowerRepository _borrowerRepository;
        private readonly ICatalogueRepository _catalogueRepository;

        public BorrowerController(ILogger<BorrowerController> logger, IBorrowerRepository borrowerRepository, ICatalogueRepository catalogueRepository)
        {
            _logger = logger;
            _borrowerRepository = borrowerRepository;
            _catalogueRepository = catalogueRepository;
        }

        [HttpGet]
        [Route("GetBorrowers")]
        public IList<Borrower> Get()
        {
            return _borrowerRepository.GetBorrowers();
        }

        [HttpPost]
        [Route("AddBorrower")]
        public Guid Post(Borrower borrower)
        {
            return _borrowerRepository.AddBorrower(borrower);
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
    }
}