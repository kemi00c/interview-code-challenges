namespace OneBeyondApi.Model
{
    public class OverdueReturns
    {
        public Guid Id { get; set; }
        public Borrower? Borrower { get; set; }
        public int NumberOfOverdueReturns { get; set; }
    }
}
