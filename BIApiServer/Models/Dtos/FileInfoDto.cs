namespace BIApiServer.Models.Dtos
{
    public class FileInfoDto
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string Path { get; set; }
        public string FileType { get; set; }
        public DateTime CreateTime { get; set; }
    }
} 