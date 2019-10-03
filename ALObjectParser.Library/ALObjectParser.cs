using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    public class ALObjectParser
    {
        public IALObject ALObject { get; set; }

        public string Path { get; set; }

        public ALObjectParser()
        {
            ALObject = new ALObject();
        }

        public ALObjectParser(string FilePath): base()
        {
            ALObject = new ALObject();
            Path = FilePath;
        }

        #region Read Object from file

        public IALObject Read()
        {
            var Lines = File.ReadAllLines(this.Path);
            return Read(Lines.ToList());
        }

        public IALObject Read(List<string> Lines)
        {
            GetObjectInfo(Lines, ALObject);
            GetMethods(Lines, ALObject);
            OnRead(Lines, ALObject);

            return ALObject;
        }

        public virtual void OnRead(List<string> Lines, IALObject info)
        { }

        public void GetObjectInfo(List<string> Lines, IALObject Target)
        {
            var pattern = @"([a-z]+)\s([0-9]+)\s(.*)";
            var line = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), pattern))
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(line))
            {
                var items = Regex.Match(line, pattern);
                var type = items.Groups[1].Value;
                if (type != Target.Type)
                {
                    throw new FileLoadException($"This AL Object has a different type than referenced implementation. Expected: {Target.Type} -> Actual: {type}");
                }
                Target.Id = int.Parse(items.Groups[2].Value);
                Target.Name = items.Groups[3].Value;
            }

            OnGetObjectInfo(line, Target);
        }

        public virtual void OnGetObjectInfo(string Line, IALObject Target)
        { }

        public void GetMethods(List<string> Lines, IALObject Target)
        {
            var pattern = @"(procedure|trigger)\s+(.*?)\((.*?)\)\:?(.*)";
            var procedures = Lines
                .Where(w => Regex.IsMatch(w, pattern))
                .ToList();

            Target.Methods = procedures
            .Select(s =>
            {
                var method = new ALMethod();

                method.IsLocal = s.Trim().StartsWith("local");
                var match = Regex.Match(s, pattern);
                method.MethodKind = match.Groups[1].Value;
                method.Name = match.Groups[2].Value;

                if (match.Groups.Count > 3)
                {
                    var paramTxt = match.Groups[3].Value.Trim();
                    if (!string.IsNullOrEmpty(paramTxt))
                    {
                        method.Parameters = paramTxt.Split(';').Select(s =>
                        {
                            var result = new ALParameter();
                            var parts = s.Split(':');
                            result.IsVar = parts[0].Trim().StartsWith("var ");
                            result.Name = parts[0].Replace("var ", "").Trim();
                            result.Type = parts[1];

                            return result;
                        }).ToList();
                    }
                }

                if (match.Groups.Count > 4)
                {
                    method.ReturnType = match.Groups[4].Value;
                }

                // Get Method body from var|begin to end;
                var start = Lines.IndexOf(s) + 1;
                var nextLine = Lines.GetRange(start, Lines.Count - start - 1).FirstOrDefault(f => Regex.IsMatch(f, pattern));
                var end = Lines.IndexOf(nextLine);
                if (end == -1)
                {
                    end = Lines.Count;
                }
                var body = string.Join("\r\n", Lines.GetRange(start, end-start - 1));
                method.Content = body;

                // Check for Test Attribute
                start = Lines.IndexOf(s) - 1;
                var testMethod = Lines.ElementAt(start);
                if (testMethod.ToLower().Contains("test"))
                {
                    method.TestMethod = true;
                }

                return method;
            })
            .ToList();
        }

        #endregion

        #region Write Object to file

        public void Write(List<ITestFeature> Features = null)
        {
            var objectTxt = Write(ALObject, Features);
            File.WriteAllText(Path, objectTxt);
        }

        public string Write(IALObject Target, List<ITestFeature> Features = null)
        {
            return OnWrite(Target, Features);
        }

        public virtual string OnWrite(IALObject Target, List<ITestFeature> Features = null)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    OnWriteObjectHeader(writer, Target, Features);                    
                    OnWriteObjectMethods(writer, Target, Features);
                    writer.WriteLine("}");
                }

                result = stringWriter.ToString();
            }

            return result;
        }

        public virtual void OnWriteObjectHeader(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            writer.WriteLine($"{Target.Type} {Target.Id} {Target.Name}");
            writer.WriteLine("{");
        }

        public virtual void OnWriteObjectMethods(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            var methods = Target.Methods.Select(s => OnWriteObjectMethod(s));
            var methodTxt = String.Join("\r\n\r\n    ", methods);

            writer.Indent++;
            writer.WriteLine(methodTxt);
            writer.Indent--;
        }

        public virtual string OnWriteObjectMethod(ALMethod method)
        {
            return method.Write();
        }

        #endregion

    }
}
