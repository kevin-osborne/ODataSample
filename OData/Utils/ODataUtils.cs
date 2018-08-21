using System;

namespace OData.Utils
{
    public class ODataUtils
    {

        public static string StripEdmTypeString(string t)
        {
            string result = t;
            try
            {
                result = t.Substring(t.IndexOf('[') + 1).Split(' ')[0];
            }
            catch (Exception)
            {
                //logger.Error(e);
            }
            return result;
        }
    }
}