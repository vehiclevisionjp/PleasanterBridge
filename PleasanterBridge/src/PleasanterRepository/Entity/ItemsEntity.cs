using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Policy;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PleasanterAPIBridge.src.PleasanterRepository.Entity
{
    [PrimaryKey(nameof(ReferenceId))]
    public class ItemsEntity
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
