

    public interface IUiDesigner
    {
        void SetUIBase(U3DFrame.UI.Controller.CUiBase iUIBase);
        void Init(System.Object iUIObj);
        void Destory();
        void SetVisible(bool bVisible);
        void SetWindowDepth(int nDepth);
    }



