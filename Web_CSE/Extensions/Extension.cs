using System.Text.RegularExpressions;

namespace Web_CSE.Extensions
{
    public static class Extension
    {
        public static string ToUrlFriendy(this string url)
        {
            var result=url.ToLower().Trim();
            result = Regex.Replace(result, "áàảãạâấầẩẫậắăắằẳẵặ", "a");
            result = Regex.Replace(result, "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ", "a");
            result = Regex.Replace(result, "éèẹẻẽêếềệểễ", "e");
            result = Regex.Replace(result, "ÉÈẸẺẼÊẾỀỆỂỄ", "e");
            result = Regex.Replace(result, "óòọỏõôốồộổỗơớờợởỡ", "o");
            result = Regex.Replace(result, "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ", "o");
            result = Regex.Replace(result, "úùụủũưứừựửữ", "u");
            result = Regex.Replace(result, "ÚÙỤỦŨƯỨỪỰỬỮ", "u");
            result = Regex.Replace(result, "íìịỉĩ", "i");
            result = Regex.Replace(result, "ÍÌỊỈĨ", "i");
            result = Regex.Replace(result, "đ", "d");
            result = Regex.Replace(result, "Đ", "d");
            result = Regex.Replace(result, "ýỳỵỷỹ", "y");
            result = Regex.Replace(result, "ÝỲỴỶỸ", "y");
            return result;
        }
    }
}
