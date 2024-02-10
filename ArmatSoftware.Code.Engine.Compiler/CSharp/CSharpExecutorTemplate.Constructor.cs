using System;
using System.Linq;

namespace ArmatSoftware.Code.Engine.Compiler.CSharp
{
	/// <summary>
	/// Partial executor template for Configuration initialization
	/// </summary>
	public partial class CSharpExecutorTemplate
	{
		/// <summary>
		/// Configuration for compilation
		/// </summary>
		public ITemplateConfiguration Configuration { get; private set; }

		/// <summary>
		/// Hidden default constructor
		/// </summary>
		private CSharpExecutorTemplate() { }

		/// <summary>
		/// Default template constructor
		/// </summary>
		/// <param name="configuration"></param>
		public CSharpExecutorTemplate(ITemplateConfiguration configuration)
		{
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

			if (configuration.GetActions() == null)
			{
				throw new ArgumentNullException(nameof(configuration.GetActions));
			}

			if (string.IsNullOrWhiteSpace(configuration.GetNamespace()))
			{
				throw new ArgumentNullException(nameof(configuration.GetNamespace));
			}

			if (string.IsNullOrWhiteSpace(configuration.GetClassName()))
			{
				throw new ArgumentNullException(nameof(configuration.GetClassName));
			}
		}
	}
}
