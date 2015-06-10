namespace Gu.Settings
{
    using System;

    public static class Ensure
    {
        public static void NotNull(object o, string paramName, string message = null)
        {
            if (o == null)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }

        public static void NotNullOrEmpty(string s, string paramName, string message = null)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (message == null)
                {
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException(paramName, message);
            }
        }
    }
}
