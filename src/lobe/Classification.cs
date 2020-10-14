namespace lobe
{
    public class Classification
    {
        public Classification(string label, double confidence)
        {
            Label = label;
            Confidence = confidence;
        }

        public string Label { get;}
        public double Confidence { get; }
    }
}