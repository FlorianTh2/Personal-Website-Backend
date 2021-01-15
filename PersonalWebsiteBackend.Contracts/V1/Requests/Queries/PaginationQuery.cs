namespace PersonalWebsiteBackend.Contracts.V1.Requests.Queries
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 100;
        }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > 100 ? 100 : pageSize;
        }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }
}