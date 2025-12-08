namespace PleasanterBridge.src.DataRepository.Service
{
    internal interface IGetReferenceProcess
    {
        public string Execute(string id, bool isReferenceId);
    }
}