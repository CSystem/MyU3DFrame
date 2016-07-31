namespace U3DFrame.Interface
{
    //具有接受消息的模块
    public interface IReciever
    {
        void ProcessMessage(IMessage message);
    }
}
