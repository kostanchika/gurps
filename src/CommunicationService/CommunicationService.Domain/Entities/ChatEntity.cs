namespace CommunicationService.Domain.Entities
{
    public class ChatEntity
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsGroupChat { get; set; }
        public int? OwnerId { get; set; }
        public List<int> ParticipantIds { get; set; } = [];
    }
}
