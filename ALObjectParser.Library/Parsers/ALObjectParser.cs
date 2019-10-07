using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ALObjectParser.Library
{
    /// <summary>
    /// Read/Write AL Language formatted files
    /// Base implementation that provides basic information processing
    /// </summary>
    public class ALObjectParser
    {
        #region Properties

        /// <summary>
        /// Config object that can be passed from PowerShell/Commadline, etc..
        /// </summary>
        public ALParserConfig Config { get; set; }

        /// <summary>
        /// Current AL Object that is being processed
        /// </summary>
        public IALObject ALObject { get; set; }

        /// <summary>
        /// File path of object to read from or write to 
        /// </summary>
        public string Path { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Base constructor
        /// </summary>
        public ALObjectParser()
        {
            ALObject = new ALObject();
            Config = new ALParserConfig();
        }

        /// <summary>
        /// Constructor for PowerShell/Commadline
        /// </summary>
        /// <param name="FilePath">Filesystem path of AL Object</param>
        public ALObjectParser(string FilePath): base()
        {
            Path = FilePath;
        }

        /// <summary>
        /// Constructor for PowerShell/Commadline
        /// Path is part of the config objectt in this case
        /// </summary>
        ///<param name="config"><see cref="ALParserConfig"/></param>
        public ALObjectParser(ALParserConfig config): base()
        {
            Config = config;
            Path = Config.FilePath;
        }

        #endregion

        #region Read Object from file

        /// <summary>
        /// Read File specified in "Path" property
        /// </summary>
        /// <returns></returns>
        public IALObject Read()
        {
            var Lines = File.ReadAllLines(this.Path);
            return Read(Lines.ToList());
        }

        /// <summary>
        /// Read File contents converted to String array
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <returns></returns>
        public IALObject Read(List<string> Lines)
        {
            GetObjectInfo(Lines, ALObject);
            GetMethods(Lines, ALObject);
            OnRead(Lines, ALObject);

            return ALObject;
        }

        /// <summary>
        /// Method to implement custom processing during parsing
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Target"></param>
        public virtual void OnRead(List<string> Lines, IALObject Target)
        { }

        /// <summary>
        /// Basic object information, such as Type, ID, Name
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <param name="Target">Current ALObject instance</param>
        public void GetObjectInfo(List<string> Lines, IALObject Target)
        {
            var pattern = @"([a-z]+)\s([0-9]+)\s(.*)";
            var line = Lines
                .Where(w => Regex.IsMatch(w.ToLower(), pattern))
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(line))
            {
                var items = Regex.Match(line, pattern);
                var type = items.Groups[1].Value.ToEnum<ALObjectType>();
                if (type != Target.Type)
                {
                    throw new FileLoadException($"This AL Object has a different type than referenced implementation. Expected: {Target.Type} -> Actual: {type}");
                }
                Target.Id = int.Parse(items.Groups[2].Value);
                Target.Name = items.Groups[3].Value;
            }

            OnGetObjectInfo(line, Target);
        }

        /// <summary>
        /// Method to implement custom  for extended classes
        /// </summary>
        /// <param name="Line">Top line of object definition</param>
        /// <param name="Target">Current ALObject instance</param>
        public virtual void OnGetObjectInfo(string Line, IALObject Target)
        { }

        /// <summary>
        /// Parse method of AL Object: triggers and procedures as well
        /// </summary>
        /// <param name="Lines">Array of textlines</param>
        /// <param name="Target">Current ALObject instance</param>
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

        /// <summary>
        /// Generate a new filecontent from a TestFeature set
        /// Prepared for PowerShell cmdlets
        /// </summary>
        /// <param name="Features">TestFeature set to generate AL Methods</param>
        public void Write(List<ITestFeature> Features = null)
        {
            var objectTxt = Write(ALObject, Features);
            File.WriteAllText(Path, objectTxt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Target">Current ALObject instance</param>
        /// <param name="Features">TestFeature set to be merged with AL Methods</param>
        /// <returns></returns>
        public string Write(IALObject Target, List<ITestFeature> Features = null)
        {
            return OnWrite(Target, Features);
        }

        /// <summary>
        /// Extensible function
        /// </summary>
        /// <param name="Target">Current ALObject instance</param>
        /// <param name="Features">TestFeature set to be merge with AL Methods</param>
        /// <returns></returns>
        public virtual string OnWrite(IALObject Target, List<ITestFeature> Features = null)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    OnWriteObjectHeader(writer, Target, Features);
                    OnWriteObjectMethods(writer, Target, Features);
                    OnWriteObjectFooter(writer, Target, Features);
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
            var methods = Target.Methods.Select(method => OnWriteObjectMethod(Target, method));
            var methodTxt = String.Join("\r\n\r\n    ", methods);

            writer.Indent++;
            writer.WriteLine(methodTxt);
            writer.Indent--;
        }

        public virtual void OnWriteObjectFooter(IndentedTextWriter writer, IALObject Target, List<ITestFeature> Features = null)
        {
            writer.WriteLine("}");
        }

        public virtual string OnWriteObjectMethod(IALObject Target, ALMethod method)
        {
            return method.Write();
        }

        #endregion
    }
}
