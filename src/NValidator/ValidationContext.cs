
using System.Collections.Generic;
using System.Linq;

namespace NValidator
{
    public class ValidationContext
    {
        public static readonly string IgnoredMembers = "IgnoredMembers";
        public object ContainerInstance { get; set; }
        public IDictionary<object, object> Items { get; set; }
        
        public ValidationContext()
        {
            Items = new Dictionary<object, object> {{IgnoredMembers, new List<string>()}};
        }
    }

    public static class ValidationContextExtensions
    {
        public static bool ShouldIgnore(this ValidationContext context, string memberName)
        {
            return memberName != null &&
                   context != null &&
                   context.Items != null &&
                   context.Items.Any(x => ValidationContext.IgnoredMembers.Equals(x.Key) && 
                                         (x.Value as List<string>).Any(memberName.StartsWith));
        }
    }
}
