using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ALObjectParser.Library
{
    public class ALObjectWriterBase
    {
        #region Write Object to file

        /// <summary>
        /// Generate a new filecontent from IALObject
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Path"></param>
        public void Write(IEnumerable<IALObject> Target, string Path)
        {
            var objectTxt = Write(Target);
            File.WriteAllText(Path, objectTxt);
        }

        public string Write(IEnumerable<IALObject> Target)
        {
            var objectTxts = Target.Select(s => Write(s));
            var objectTxt = string.Join($"{Environment.NewLine}", objectTxts);

            return objectTxt;
        }

        /// <summary>
        /// Generate a new filecontent from IALObject
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="Path"></param>
        public void Write(IALObject Target, string Path)
        {
            var objectTxt = Write(Target);
            File.WriteAllText(Path, objectTxt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Target">Current ALObject instance</param>
        /// <param name="Features">TestFeature set to be merged with AL Methods</param>
        /// <returns></returns>
        public string Write(IALObject Target)
        {
            return OnWrite(Target);
        }

        /// <summary>
        /// Extensible function
        /// </summary>
        /// <param name="Target">Current ALObject instance</param>
        /// <returns></returns>
        public virtual string OnWrite(IALObject Target)
        {
            var result = "";
            using (var stringWriter = new StringWriter())
            {
                using (var writer = new IndentedTextWriter(stringWriter))
                {
                    OnWriteObjectHeader(writer, Target);
                    OnWriteObjectProperties(writer, Target);
                    Target.Write(writer);
                    OnWriteObjectMethods(writer, Target);
                    OnWriteObjectFooter(writer, Target);
                }

                result = stringWriter.ToString();
            }

            return result;
        }

        public virtual void OnWriteObjectHeader(IndentedTextWriter writer, IALObject Target)
        {
            writer.WriteLine($"{Target.Type} {Target.Id} {(Target.Name.Contains(' ') ? $"\"{Target.Name}\"" : Target.Name)}");
            writer.WriteLine("{");
        }
        
        private void OnWriteObjectProperties(IndentedTextWriter writer, IALObject target)
        {
            foreach (ALProperty aLProperty in target.Properties)
                writer.WriteLine(aLProperty.Name + " = " + aLProperty.Value + ";");
        }


        public virtual void OnWriteObjectMethods(IndentedTextWriter writer, IALObject Target)
        {
            var methods = Target.Methods.Select(method => OnWriteObjectMethod(Target, method));
            var methodTxt = String.Join("\r\n\r\n    ", methods);

            writer.Indent++;
            writer.WriteLine(methodTxt);
            writer.Indent--;
        }

        public virtual void OnWriteObjectFooter(IndentedTextWriter writer, IALObject Target)
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
