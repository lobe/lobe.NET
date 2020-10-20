using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace lobe
{
    public class Signature
    {
        public static Signature FromFile(FileInfo signatureFile)
        {
            var json = JsonDocument.Parse(File.ReadAllText(signatureFile.FullName)).RootElement;
            var modelPath = new DirectoryInfo(Path.GetDirectoryName(signatureFile.FullName));
            var inputs = json.GetProperty("inputs");
            var outputs = json.GetProperty("outputs");
            var classes = json.GetProperty("classes").GetProperty("Label").EnumerateArray().Select(je => je.GetString()).ToArray();

            return new Signature
            {
                ModelPath = modelPath,
                Id = json.GetProperty("doc_id").GetString(),
                Name = json.GetProperty("doc_name").GetString(),
                FileName = json.GetProperty("filename").GetString(),
                Format = json.GetProperty("format").GetString(),
                Classes = classes,
                Inputs = inputs,
                Outputs = outputs
            };
        }

        private JsonElement Outputs { get; set; }

        private JsonElement Inputs { get; set; }

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

            string inputId;

            try
            {
                inputId = Inputs.GetProperty($"{inputLabel}").GetProperty("name").GetString();
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException($"Cannot locate inputs.{inputLabel}.name", e);
            }

            return inputId;
        }

        public int[] GetInputShape(string inputLabel)
        {
            if (string.IsNullOrWhiteSpace(inputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputLabel));
            }
            var values = new List<int>();
            try
            {
                var shapes = Inputs.GetProperty($"{inputLabel}").GetProperty("shape").EnumerateArray();

                foreach (var shape in shapes)
                {
                    switch (shape.ValueKind)
                    {
                        case JsonValueKind.Null:
                        case JsonValueKind.Undefined:
                            values.Add(1);
                            break;
                        case JsonValueKind.Number:
                            values.Add(shape.GetInt32());
                            break;
                        default:
                            throw new InvalidOperationException("");
                    }

                }
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException($"Cannot parse inputs.{inputLabel}.shape", e);
            }

            return values.ToArray();

        }

        public string GetInputType(string inputLabel)
        {
            if (string.IsNullOrWhiteSpace(inputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(inputLabel));
            }

            string inputType;
            try
            {
                inputType = Inputs.GetProperty($"{inputLabel}").GetProperty("dtype").GetString();
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException($"Cannot parse inputs.{inputLabel}.dtype", e);
            }

            return inputType;
        }

        public string GetOutputId(string outputLabel)
        {
            if (string.IsNullOrWhiteSpace(outputLabel))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputLabel));
            }

            string outputId;
            try
            {
                outputId = Outputs.GetProperty($"{outputLabel}").GetProperty("name").GetString();
            }
            catch (Exception e)
            {
                throw new KeyNotFoundException($"Cannot parse outputs.{outputLabel}.name", e);
            }
            return outputId;
        }

        public FileInfo GetModelFile(string fileName = null)
        {
            fileName ??= FileName;
            return Path.IsPathRooted(fileName)
                ? new FileInfo(fileName)
                : new FileInfo(Path.Combine(ModelPath.FullName, fileName ?? FileName));
        }
    }
}