using PleasanterBridge.src.APIBridge.Entity;

namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class PleasanterApiRequest
    {
        public string? Title { get; }

        public string? Body { get; }

        // issues用プロパティ
        public string? StartTime { get; }

        public string? CompletionTime { get; }

        public decimal? WorkValue { get; }

        public decimal? ProgressRate { get; }

        public Dictionary<string, string> ClassHash { get; set; } = new();
        public Dictionary<string, decimal> NumHash { get; set; } = new();
        public Dictionary<string, string> DateHash { get; set; } = new();
        public Dictionary<string, string> DescriptionHash { get; set; } = new();
        public Dictionary<string, bool> CheckHash { get; set; } = new();
        public Dictionary<string, string> AttachmentsHash { get; set; } = new();
        public Dictionary<string, ImageEntity> ImageHash { get; set; } = new();

        public PleasanterApiRequest(
            string? title,
            string? body,
            string? startTime,
            string? completionTime,
            decimal? workValue,
            decimal? progressRate
        )
        {
            Title = title;
            Body = body;
            StartTime = startTime;
            CompletionTime = completionTime;
            WorkValue = workValue;
            ProgressRate = progressRate;
        }
    }
}
