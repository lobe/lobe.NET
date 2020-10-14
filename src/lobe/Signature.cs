using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace lobe
{
    public class Signature
    {
        public static Signature FromFile(FileInfo signatureFile)
        {
            var json = JObject.Parse(File.ReadAllText(signatureFile.FullName));
            var modelPath = new DirectoryInfo(Path.GetDirectoryName(signatureFile.FullName));
            var inputs = json["inputs"].Value<JObject>();
            var outputs = json["outputs"].Value<JObject>();
            var classes = json["classes"]["Label"].Values<string>().ToArray();

            return new Signature
            {
                ModelPath = modelPath,
                Id = json["doc_id"].Value<string>(),
                Name = json["doc_name"].Value<string>(),
                FileName = json["filename"].Value<string>(),
                Format = json["format"].Value<string>(),
                Classes = classes,
                Inputs = inputs,
                Outputs = outputs
            };
        }

        public JObject Outputs { get; private set; }

        public JObject Inputs { get; private set; }

        public IEnumerable<string> Classes { get; private set; }

        public string Format { get; private set; }

        public string FileName { get; private set; }

        public string Name { get; private set; }

        public DirectoryInfo ModelPath { get; private set; }

        public string Id { get; private set; }

        public string GetInputId(string inputLabel)
        {
            if (string.IsNullOrWhiteSpace(inputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputLabel));
            }
            return Inputs.SelectToken($"{inputLabel}.name")?.Value<string>() ?? throw new KeyNotFoundException($"Cannot locate inputs.{inputLabel}.name");
        }

        public int[] GetInputShape(string inputLabel)
        {
            if (string.IsNullOrWhiteSpace(inputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputLabel));
            }

            var shape = Inputs.SelectToken($"{inputLabel}.shape")?.Values<int?>()?.Select(f => f ?? 1).ToArray();
            return shape ?? throw new KeyNotFoundException($"Cannot locate inputs.{inputLabel}.shape");

        }

        public string GetInputType(string inputLabel)
        {
            if (string.IsNullOrWhiteSpace(inputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputLabel));
            }
            return Inputs.SelectToken($"{inputLabel}.dtype")?.Value<string>() ?? throw new KeyNotFoundException($"Cannot locate inputs.{inputLabel}.dtype");
        }

        public string GetOutputId(string outputLabel)
        {
            if (string.IsNullOrWhiteSpace(outputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputLabel));
            }
            return  Outputs.SelectToken($"{outputLabel}.name")?.Value<string>() ?? throw new KeyNotFoundException($"Cannot locate outputs.{outputLabel}.name");
        }

        public FileInfo GetModelFile(string fileName = null)
        { 
            fileName??= FileName;
            return Path.IsPathRooted(fileName) 
                ? new FileInfo(fileName) 
                : new FileInfo(Path.Combine(ModelPath.FullName, fileName?? FileName));
        }
    }
}