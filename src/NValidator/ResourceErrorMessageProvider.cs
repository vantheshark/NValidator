using System;
using System.Resources;

namespace NValidator
{
    public class ResourceErrorMessageProvider : IErrorMessageProvider
    {
        private readonly ResourceManager _rm;

        public ResourceErrorMessageProvider(Type resourceType)
        {
            if (resourceType == null)
            {
                throw new ArgumentNullException("resourceType");
            }
            _rm = new ResourceManager(resourceType);
            
        }

        public string GetError(object key)
        {
            if (key == null || string.Empty.Equals(key))
            {
                throw new ArgumentException("Resource key can not be null or empty.");
            }
            return _rm.GetString(key.ToString());
        }
    }
}