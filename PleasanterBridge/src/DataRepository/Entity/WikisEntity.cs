using Microsoft.EntityFrameworkCore;

namespace PleasanterBridge.src.DataRepository.Entity
{
    [PrimaryKey(nameof(WikiId))]
    internal class WikisEntity
    {
        public long SiteId { get; set; }
        public DateTime UpdatedTime { get; set; }
        public long WikiId { get; set; }
        public int Ver { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public bool? Locked { get; set; }
        public string? Comments { get; set; }
        public int? Creator { get; set; }
        public int? Updator { get; set; }
        public DateTime? CreatedTime { get; set; }
    }
}
