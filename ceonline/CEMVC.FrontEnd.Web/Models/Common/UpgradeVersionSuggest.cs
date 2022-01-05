namespace CEMVC.FrontEnd.Web.Models.Common
{
    public struct UpgradeVersionSuggest
    {
        public string Feature { get; set; }
        public UpgradeVersionSuggest(string featureName): this()
        {
            Feature = featureName;
        }
    }
}