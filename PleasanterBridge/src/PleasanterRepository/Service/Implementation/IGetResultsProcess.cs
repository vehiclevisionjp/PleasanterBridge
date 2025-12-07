namespace PleasanterAPIBridge.src.PleasanterRepository.Service.Implementation
{
    public interface IGetResultsProcess
    {
        public Dictionary<string, object> Execute(string resultId);
    }
}
