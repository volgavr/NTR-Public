namespace CEMVC.MasterData.BLL.Infrastructure
{
    [System.Flags]
    public enum TemplateUseEnum
    {        
        ClearEstimates = 0x0001,        
        Snaptimate = 0x0002,
        FollowUp = 0x0004
    }
}