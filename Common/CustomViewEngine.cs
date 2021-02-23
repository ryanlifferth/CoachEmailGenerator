using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace CoachEmailGenerator.Common
{
    public class CustomViewEngine : IViewLocationExpander
    {
        private static string[] _newPartialViewFormats = new[]
        {
            "~/Views/{1}/PartialViews/{0}.cshtml",
            "~/Views/Shared/PartialViews/{0}.cshtml"
        };

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Union(_newPartialViewFormats);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
