using System.Collections.Generic;
using U3DFrame.Enum;
using U3DFrame.Event;
using U3DFrame.Interface;

namespace U3DFrame.Manager
{
    public class EventManager
    {
        private IYxClient client;
        private Dictionary<YxEventType, VoidObjectDelegate> callFuncEventTypeDict = new Dictionary<YxEventType, VoidObjectDelegate>();
        private Dictionary<long, BaseDto> dtoEventDict = new Dictionary<long, BaseDto>();
        private Queue<MainThreadEvent> mainThreadEventsList = new Queue<MainThreadEvent>();
        public EventManager(IYxClient client)
        {
            this.client = client;
        }
        public BaseDto GetDto(long requestId)
        {
            if (!this.dtoEventDict.ContainsKey(requestId))
            {
                return null;
            }
            return this.dtoEventDict[requestId];
        }
        public void AddEventListener(long evtId, BaseDto dto)
        {
            this.dtoEventDict[evtId] = dto;
        }
        public void AddEventListener(BaseDto dto)
        {
            this.dtoEventDict[dto.requestId] = dto;
        }
        public void RemoveEventListener(BaseDto dto)
        {
            if (dto != null && this.dtoEventDict.ContainsKey(dto.requestId))
            {
                this.dtoEventDict.Remove(dto.requestId);
            }
        }
        public void AddEventListener(YxEventType eventType, VoidObjectDelegate callFunc)
        {
            this.callFuncEventTypeDict[eventType] = callFunc;
        }
        public void RemoveEventListener(YxEventType eventType)
        {
            if (this.callFuncEventTypeDict.ContainsKey(eventType))
            {
                this.callFuncEventTypeDict.Remove(eventType);
            }
        }
        public VoidObjectDelegate GetEventListener(YxEventType eventType)
        {
            if (this.callFuncEventTypeDict.ContainsKey(eventType))
            {
                return this.callFuncEventTypeDict[eventType];
            }
            return null;
        }
        public void DispatchEvent(YxEventType eventType, object param)
        {
            VoidObjectDelegate callFunc = GetEventListener(eventType);
            if (callFunc == null)
            {
                return;
            }

            if (!this.client.IsMainThread())
            {
                this.DispatchToMainThread(callFunc, param);
            }
            else
            {
                callFunc(param);
            }
        }

        public void DispatchToMainThread(VoidObjectDelegate call, object param)
        {
            MainThreadEvent evt = new MainThreadEvent();
            evt.callFuncEvents += call;
            evt.param = param;
            this.DispatchToMainThread(evt);
        }
        public void DispatchToMainThread(ParamDtoDelegate call, BaseDto dto)
        {
            MainThreadEvent evt = new MainThreadEvent();
            evt.dtoCallFuncEvents += call;
            evt.dto = dto;
            this.DispatchToMainThread(evt);
        }
        public void DispatchToMainThread(MainThreadEvent evt)
        {
            this.mainThreadEventsList.Enqueue(evt);
        }
        public void ExecuteMainEvents()
        {
            if (this.mainThreadEventsList.Count <= 0)
            {
                return;
            }
            MainThreadEvent evt = this.mainThreadEventsList.Dequeue();
            evt.Execute();
        }

        public void DispatchToMainThread(VoidIntDelegate call, int value)
        {
            MainThreadEvent evt = new MainThreadEvent();
            evt.callVoidIntFuncEvents += call;
            evt.intParam = value;
            this.DispatchToMainThread(evt);
        }
    }
}
