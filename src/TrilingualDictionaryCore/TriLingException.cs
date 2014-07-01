using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrilingualDictionaryCore
{
    class TriLingException : Exception
    {
        public TriLingException()
        {
        }

        public TriLingException(string message)
            : base(message)
        {
        }

        public TriLingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
