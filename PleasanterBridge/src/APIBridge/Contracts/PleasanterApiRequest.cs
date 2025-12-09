namespace PleasanterBridge.src.APIBridge.Contracts
{
    internal class PleasanterApiRequest
    {
        public readonly string? Title;

        public readonly string? Body;

        // issues用プロパティ
        public readonly string? StartTime;

        public readonly string? CompletionTime;

        public readonly decimal? WorkValue;

        public readonly decimal? ProgressRate;

        public Dictionary<string, string> ClassHash { get; set; } = new();
        public Dictionary<string, decimal> NumHash { get; set; } = new();
        public Dictionary<string, DateTime> DateHash { get; set; } = new();
        public Dictionary<string, string> DescriptionHash { get; set; } = new();
        public Dictionary<string, bool> CheckHash { get; set; } = new();
        public Dictionary<string, string> AttachmentsHash { get; set; } = new();

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
