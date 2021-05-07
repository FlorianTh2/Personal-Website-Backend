using System.Collections.Generic;

namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    // intentionally does not inherit from Response
    //    - maybe its a bad idea to make a contract class dependent of some other classes (or contracts)
    public class PagedResponse<T>
    {
        public IEnumerable<T> Data { get; set; }
        
        public int? PageNumber { get; set; }
        
        public int? PageSize { get; set; }

        public string NextPage { get; set; }

        public string PreviousPage { get; set; }

        public int PagesTotal { get; set; }

        public int ItemsTotal { get; set; }
        

        // default ctor since our sdk needs it
        public PagedResponse()
        {
            
        }

        public PagedResponse(IEnumerable<T> data)
        {
            Data = data;
        }
    }
}