using System;
using System.Text;

namespace MVC.Models.Helpers
{
    public class GenCode
    {
        public string AutoGenCode(string? lastCode, string defaultPrefix = "STD", int numberWidth = 3)
        {
            if (string.IsNullOrWhiteSpace(lastCode))
            {
                return defaultPrefix + 1.ToString("D" + numberWidth);
            }

            int idx = 0;
            while (idx < lastCode.Length && !char.IsDigit(lastCode[idx]))
            {
                idx++;
            }

            string prefix;
            string numberPart;
            if (idx == lastCode.Length)
            {
                prefix = defaultPrefix;
                numberPart = "";
            }
            else
            {
                prefix = lastCode.Substring(0, idx);
                numberPart = lastCode.Substring(idx);
            }

            int number = 0;
            if (!string.IsNullOrEmpty(numberPart) && int.TryParse(numberPart, out int parsed))
            {
                number = parsed;
            }

            number++;
            string nextNumber = number.ToString("D" + numberWidth);
            return prefix + nextNumber;
        }
    }
}
