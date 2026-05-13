using System.Text;

namespace BaseLibrary.Tools;

public static class Extensions
{
    extension(string str)
    {
        public string TruncateByWordsEfficient(int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            var words = str.Split(' ');
            var sb = new StringBuilder();

            foreach (var word in words)
            {
                // Проверяем, влезет ли слово + пробел
                if (sb.Length + word.Length + 1 > maxLength)
                    break;

                if (sb.Length > 0)
                    sb.Append(' ');

                sb.Append(str);
            }

            return sb.ToString();
        }
    }
}