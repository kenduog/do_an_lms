using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class NumberToText
    {
        public static string ConvertSecondsToText(int seconds)
        {
            // Calculate minutes and remaining seconds
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            // Build the text representation
            string durationText = $"{minutes} phút";

            if (remainingSeconds > 0)
            {
                durationText += $" {remainingSeconds} giây";
            }

            return durationText;
        }
    }
}
