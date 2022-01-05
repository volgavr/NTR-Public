using CEMVC.Utility.Attributes;

namespace CEMVC.MasterData.BLL.Infrastructure
{
    public enum Part_TypeEnum
    {
        [EnumGuid("159B0427-C358-EA11-A2E9-00155D011410")] Piece = 1,
        [EnumGuid("169B0427-C358-EA11-A2E9-00155D011410")] SqFoot = 2,
        [EnumGuid("179B0427-C358-EA11-A2E9-00155D011410")] LinFoot = 3,
        [EnumGuid("189B0427-C358-EA11-A2E9-00155D011410")] CuYd = 4,
        [EnumGuid("199B0427-C358-EA11-A2E9-00155D011410")] Pound = 5,
        [EnumGuid("1A9B0427-C358-EA11-A2E9-00155D011410")] Hour = 6
    }
}
