namespace CommunicationService.Application.Dto.Chat
{
    public class ChatDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LogoUri { get; set; } = null!;
        public string OwnerLogin { get; set; } = null!;
        public List<string> Participants { get; set; } = [];
    }
}
