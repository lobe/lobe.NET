using System.Collections.Generic;

namespace lobe
{
    public class ClassificationResults
    {
        public ClassificationResults(Classification classification, IEnumerable<Classification> classifications)
        {
            Classification = classification;
            Classifications = classifications;
        }

        public Classification Classification { get; }
        public IEnumerable<Classification> Classifications { get; }
    }
}