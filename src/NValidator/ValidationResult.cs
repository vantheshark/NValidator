
using System;
using System.Collections.Generic;
using System.Text;

namespace NValidator
{
    public class ValidationResult
    {
        public virtual string Message { get; set; }

        public virtual string PropertyName { get; set; }
        
        /// <summary>
        /// Gets or sets the property chain
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        public virtual string MemberName { get; set; }
    }

    public class FormattableMessageResult : ValidationResult
    {
        private readonly Dictionary<string, object> _param;

        public FormattableMessageResult(Dictionary<string, object> param)
        {
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
            _param = param;
        }

        private string _messageFormat;
        private string _message;

        public sealed override string Message
        {
            get
            {
                if (_message == null)
                {
                    var result = new StringBuilder(_messageFormat);
                    foreach (var p in _param)
                    {
                        result.Replace(p.Key, p.Value.ToString());
                    }
                    _message = result.ToString();
                }
                return _message;
            }
            set 
            { 
                _messageFormat = value;
                _message = null;
            }
        }
    }
}
