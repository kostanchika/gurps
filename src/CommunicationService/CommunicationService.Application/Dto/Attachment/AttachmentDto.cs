namespace CommunicationService.Application.Dto.Attachment
{
    public class AttachmentDto
    {
        public string FileName { get; set; } = null!;
        public string FileUri { get; set; } = null!;
        public bool IsPreviewAvailable { get; set; }
        public int FileSizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
