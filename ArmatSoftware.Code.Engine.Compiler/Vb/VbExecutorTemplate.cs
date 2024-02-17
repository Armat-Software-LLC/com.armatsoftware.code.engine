﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace ArmatSoftware.Code.Engine.Compiler.Vb
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class VbExecutorTemplate : VbExecutorTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("\n");
            this.Write("\n");
            
            #line 10 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 foreach (var import in Configuration.GetImports()) { 
            
            #line default
            #line hidden
            this.Write("Imports ");
            
            #line 11 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(import));
            
            #line default
            #line hidden
            this.Write("\n");
            
            #line 12 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\nNamespace ");
            
            #line 14 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetNamespace()));
            
            #line default
            #line hidden
            this.Write("\n\tPublic Class ");
            
            #line 15 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetClassName()));
            
            #line default
            #line hidden
            this.Write("\n\t\tImplements IFactoryExecutor(Of ");
            
            #line 16 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(")\n\t\n\t\tPrivate _runtimeValues As Dictionary(Of String, Object) = new Dictionary(Of String, Object)\n\t    Private _logger As ICodeEngineLogger\n\t    Private _subject As ");
            
            #line 20 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write("\n\t\t\n\t\tPublic Readonly Property Subject As ");
            
            #line 22 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(" Implements IExecutor(Of ");
            
            #line 22 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(").Subject\n    \t\tGet\n            \tReturn _subject\n        \tEnd Get\n    \tEnd Property\n\n\t\tPublic ReadOnly Property Log As ICodeEngineLogger Implements IExecutor(Of ");
            
            #line 28 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(").Log\n\t        Get\n\t            Return _logger\n\t        End Get\n\t    End Property\n\n        Public Function Read(key As String) As Object Implements IExecutionContext.Read\n            Return _runtimeValues(key)\n        End Function\n        \n        Public Sub Save(key As String, value As Object) Implements IExecutionContext.Save\n            _runtimeValues.Add(key, value)\n        End Sub\n\n    \tPublic Function Clone() As IFactoryExecutor(Of ");
            
            #line 42 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(") Implements IFactoryExecutor(Of ");
            
            #line 42 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(").Clone\n\t\t\tReturn DirectCast(MemberwiseClone(), IFactoryExecutor(Of ");
            
            #line 43 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write("))\n\t\tEnd Function\n\n\t    Public Sub SetLogger(logger As ICodeEngineLogger) Implements IFactoryExecutor(Of ");
            
            #line 46 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(").SetLogger\n\t        _logger = logger\n\t    End Sub\n\n\t\tPublic Function Execute(subject As ");
            
            #line 50 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(") As ");
            
            #line 50 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(" Implements IExecutor(Of ");
            
            #line 50 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Configuration.GetSubjectType()));
            
            #line default
            #line hidden
            this.Write(").Execute\n\t\t\t_subject = subject\n\n");
            
            #line 53 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 foreach (var action in Configuration.GetActions()) { 
            
            #line default
            #line hidden
            this.Write("\t\t\t");
            
            #line 54 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Key));
            
            #line default
            #line hidden
            this.Write("()\n");
            
            #line 55 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\n\t\t\tReturn _subject\n\t\tEnd Function\n\t\t\n");
            
            #line 60 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 foreach (var action in Configuration.GetActions()) { 
            
            #line default
            #line hidden
            this.Write("\n\t\tPrivate Sub ");
            
            #line 62 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Key));
            
            #line default
            #line hidden
            this.Write("()\n\t\t\t");
            
            #line 63 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(action.Value));
            
            #line default
            #line hidden
            this.Write("\n\t\tEnd Sub\n");
            
            #line 65 "/Users/yurikazarov/Projects/com.armatsoftware.code.engine/ArmatSoftware.Code.Engine.Compiler/Vb/VbExecutorTemplate.tt"
 } 
            
            #line default
            #line hidden
            this.Write("\n\tEnd Class\nEnd Namespace");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class VbExecutorTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
