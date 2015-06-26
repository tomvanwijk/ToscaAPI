using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BKR.Test.ToscaAPI.Shared
{
    public static class FileComparer
    {
        public static string Compare (string expectedFile, string actualFile, string wildcard = "*")
        {
            if (!File.Exists(expectedFile))
            {
                throw new FileNotFoundException(String.Format("The file '{0}' was not found.", expectedFile));
            }
            if (!File.Exists(actualFile))
            {
                throw new FileNotFoundException(String.Format("The file '{0}' was not found.", actualFile));
            }

            String[] linesA = File.ReadAllLines(expectedFile);
            String[] linesB = File.ReadAllLines(actualFile);

            IEnumerable<String> onlyB = linesB.Except(linesA);

            if (onlyB.Count() == 0)
                return String.Empty;

            string result = "ActualFile contains the following differences:" + Environment.NewLine;
            result += String.Join(Environment.NewLine, onlyB.ToArray());
            return result;
        }
    }
}
