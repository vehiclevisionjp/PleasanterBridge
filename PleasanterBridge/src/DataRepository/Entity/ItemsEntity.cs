using Microsoft.EntityFrameworkCore;

namespace PleasanterBridge.src.DataRepository.Entity
{
    [PrimaryKey(nameof(ReferenceId))]
    internal class ItemsEntity
    {
        public long? ReferenceId { get; set; }
        public int Ver { get; set; }
        public string? ReferenceType { get; set; }
        public long? SiteId { get; set; }
        public string? Title { get; set; }
        public string? FullText { get; set; }
        public DateTime? SearchIndexCreatedTime { get; set; }
        public string? Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
    }
}
