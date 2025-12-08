using Microsoft.EntityFrameworkCore;

namespace PleasanterBridge.src.DataRepository.Entity
{
    [PrimaryKey(nameof(BinaryId))]
    public class BinariesEntity
    {
        public long? BinaryId { get; set; }
        public int TenantId { get; set; }
        public long? ReferenceId { get; set; }
        public string? Guid { get; set; }
        public int? Ver { get; set; }
        public string? BinaryType { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public byte[]? Bin { get; set; }
        public byte[]? Thumbnail { get; set; }
        public byte[]? Icon { get; set; }
        public string? FileName { get; set; }
        public string? Extension { get; set; }
        public long? Size { get; set; }
        public string? ContentType { get; set; }
        public string? BinarySettings { get; set; }
        public string? Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
