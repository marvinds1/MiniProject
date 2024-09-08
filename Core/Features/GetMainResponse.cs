namespace Core.Features
{
    public class GetMainResponse
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<object>? Content { get; set; }
        //public Pagination pagination { get; set; }
    }

    //public class Pagination
    //{
    //    public int CurrentPage { get; set; }
    //    public int PageSize { get; set; }
    //    public int TotalPages { get; set; }
    //    public int TotalItems { get; set; }
    //}
}

