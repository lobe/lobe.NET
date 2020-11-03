using System.Collections.Generic;

namespace lobe
{
    public class ClassificationResults
    {
        public ClassificationResults(Classification prediction, IEnumerable<Classification> classifications)
        {
            Prediction = prediction;
            Classifications = classifications;
        }

        public Classification Prediction { get; }
        public IEnumerable<Classification> Classifications { get; }
    }
}