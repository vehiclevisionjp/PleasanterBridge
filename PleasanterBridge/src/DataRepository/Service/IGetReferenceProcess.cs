namespace PleasanterBridge.src.DataRepository.Service
{
    public interface IGetReferenceProcess
    {
        public string Execute(long id, bool isReferenceId);
    }
}