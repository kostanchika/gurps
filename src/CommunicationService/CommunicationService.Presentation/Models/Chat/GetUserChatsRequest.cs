namespace CommunicationService.Presentation.Models.Chat
{
    public class GetUserChatsRequest
    {
        public string? ChatName { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortBy { get; set; }
        public string? SortType { get; set; }
    }
}
