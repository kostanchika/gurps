using CommunicationService.Application.Interfaces;

namespace CommunicationService.Presentation.Models
{
    public class AttachmentAdapter(IFormFile formFile) : IAttachment
    {
        public string FileName => formFile.FileName;

        public Stream Stream => formFile.OpenReadStream();
    }
}
