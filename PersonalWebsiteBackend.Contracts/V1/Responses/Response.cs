namespace PersonalWebsiteBackend.Contracts.V1.Responses
{
    // default response for the data
    // intention: since we maybe want to store additional meta-data to the actualy data,
    //    for example via a pagedResult, we we dont want
    //    - 1. to store this metadata directly on the level of the resulting data and
    //    - 2. to store only the pagedResult in a data-Attribute since that would break our
    //        contract in general (e.g. getall returns data in the data-property and all others
    //        return data directly without a data-property in json)
    // because of that we need to make all others (except getall, since getall returns data already
    // in a data-property in json) return data in a data-property
    public class Response<T>
    {
        public T Data { get; set; }

        // default ctor since our sdk needs it
        public Response()
        {
        }

        public Response(T response)
        {
            Data = response;
        }
    }
}