using System;

namespace MVC.Models.Process
{
    public class GenCode
    {
      
        public string AutoGenCode(string? lastCode, string defaultPrefix = "DL", int numberWidth = 2)
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
