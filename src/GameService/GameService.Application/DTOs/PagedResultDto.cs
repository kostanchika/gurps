namespace GameService.Application.Models
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int Count { get; set; }
    }
}
