namespace GARCA_UTIL.Exceptions
{
    public class DonwloadPricesException : Exception
    {
        public DonwloadPricesException()
        {
        }

        public DonwloadPricesException(string message)
            : base(message)
        {
        }

        public DonwloadPricesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
