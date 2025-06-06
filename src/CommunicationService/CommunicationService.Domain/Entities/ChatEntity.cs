namespace CommunicationService.Domain.Entities
{
    public class ChatEntity
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LogoUri { get; set; } = null!;
        public string OwnerLogin { get; set; } = null!;
        public List<string> Participants { get; set; } = [];
    }
}
