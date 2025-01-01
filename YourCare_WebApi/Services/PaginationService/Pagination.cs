namespace YourCare_WebApi.Services.PaginationService
{
    public class Pagination<T> : List<T>
    {
        public Pagination(List<T> data)
        {
            AddRange(data);
        }

        public static Pagination<T> Paginate(List<T> data, int pageNumber, int pageSize)
        {
            var items = data.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new Pagination<T>(items);
        }
    }
}
