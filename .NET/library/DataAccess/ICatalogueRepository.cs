using OneBeyondApi.Model;

namespace OneBeyondApi.DataAccess
{
    public interface ICatalogueRepository
    {
        public List<BookStock> GetCatalogue();

        public List<BookStock> SearchCatalogue(CatalogueSearch search);

        public bool Return(BookStock stock);

        public bool Reserve(BookStock stock, string borrowerName, DateTime dueDateTime);
    }
}
