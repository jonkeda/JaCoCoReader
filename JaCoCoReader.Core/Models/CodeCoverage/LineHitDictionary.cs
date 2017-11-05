using System.Collections.Generic;

namespace JaCoCoReader.Core.Models.CodeCoverage
{
    public class LineHitDictionary : Dictionary<int, bool>
    {
        public LineHitDictionary(LineCollection lines)
        {
            foreach (Line line in lines)
            {
                if (!ContainsKey(line.Nr))
                {
                    if (line.Ci > 0)
                    {
                        Add(line.Nr, true);
                    }
                    else
                    {
                        Add(line.Nr, false);
                    }
                }
            }

        }
    }
}