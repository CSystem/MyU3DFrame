using System;
using U3DFrame.Interface;

namespace U3DFrame.Manager
{
    public class DtoMgr
    {
        private IYxClient _client;
        public DtoMgr(IYxClient client)
        {
            this._client = client;
        }
        public IYxClient client
        {
            get { return this._client; }
        }
        public T GetDto<T>() where T : BaseDto
        {
            T ret = default(T);
            Type type = typeof(T);
            if (type != null)
            {
                BaseDto dto = (BaseDto)Activator.CreateInstance(type);
                dto.client = this.client;
                ret = (T)dto;
            }
            return ret;
        }
    }
}
