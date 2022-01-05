using CEMVC.Core.DAL.RemodelMAX;
using System.Collections.Generic;
using System.Linq;

namespace CEMVC.MasterData.BLL.Infrastructure
{
    public static class TemplateExtensions
    {
        public static IEnumerable<Template_Part> ActualParts(this Template template)
        {
            return template.TemplateParts.Where(t => t.deleted_at == null);
        }
    }
}
