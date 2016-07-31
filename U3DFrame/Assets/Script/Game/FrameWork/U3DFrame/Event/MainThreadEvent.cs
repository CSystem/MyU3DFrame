namespace U3DFrame.Event
{
    public class MainThreadEvent
    {
        public event VoidObjectDelegate callFuncEvents;
        public event ParamDtoDelegate dtoCallFuncEvents;
        public event VoidIntDelegate callVoidIntFuncEvents;
        public int intParam;
        public object param;
        public BaseDto dto;

        public void Execute()
        {
            if (this.callFuncEvents != null)
            {
                this.callFuncEvents(this.param);
            }
            if (this.dtoCallFuncEvents != null)
            {
                this.dtoCallFuncEvents(this.dto);
            }
            if (this.callVoidIntFuncEvents != null)
            {
                this.callVoidIntFuncEvents(this.intParam);
            }
        }
    }
}
