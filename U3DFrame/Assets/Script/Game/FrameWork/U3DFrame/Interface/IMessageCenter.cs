

namespace U3DFrame.Interface
{
    //消息中心接口，以后可能增加多个消息中心协同工作
    public interface IMessageCenter
    {
        void Register(IReciever iReciever, ushort[] msgId);
        void RemoveMessage(IReciever iReciever, ushort[] msgId);
        bool HasMessage(ushort msgId);
        void SendMessage(IMessage message);
        void Update(double delta);
    }
}
