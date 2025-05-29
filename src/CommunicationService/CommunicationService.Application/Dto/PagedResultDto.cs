namespace CommunicationService.Application.Dto
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int TotalCount { get; set; }
    }
}
