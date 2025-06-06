namespace CommunicationService.Domain.Entities
{
    public class AttachmentEntity
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string FileUri { get; set; } = null!;
        public bool IsPreviewAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
