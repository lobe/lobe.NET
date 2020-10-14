using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reactive.Disposables;

namespace lobe
{
    public abstract class ImageClassifier : IDisposable
    {
        private static readonly ConcurrentDictionary<string, Func<ImageClassifier>> Factories = new ConcurrentDictionary<string, Func<ImageClassifier>>(StringComparer.OrdinalIgnoreCase);

        public static void Register(string format, Func<ImageClassifier> factory)
        {
            Factories[format] = factory;
        }

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        protected void RegisterForDisposal(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public Signature Signature { get; private set; }

        public abstract ClassificationResults Classify(float[] input);

        public abstract void InitialiseFromSignature(Signature signature, string modelFileName = null);

        protected void RegisterForDisposal(Action onDispose)
        {
            _disposables.Add(Disposable.Create(onDispose));
        }

        public void InitialiseFromSignatureFile(FileInfo signatureFile, string modelFileName = null)
        {
            InitialiseFromSignature(Signature.FromFile(signatureFile), modelFileName);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        public static ImageClassifier CreateFromSignatureFile(FileInfo signatureFile, string modelFileName = null,
            string format = null)

        {
            return CreateFromSignature(Signature.FromFile(signatureFile), modelFileName, format);
        }

        public static ImageClassifier CreateFromSignature(Signature signature, string modelFileName = null,
            string engineType = null)

        {
            engineType ??= signature.Format;
            var classifier = Factories[engineType]();
            classifier.Signature = signature;
            classifier.InitialiseFromSignature(signature, modelFileName);
          
            return classifier;
        }
    }
}
