using System;
using System.Linq;

namespace ArmatSoftware.Code.Engine.Compiler.Vb
{
	/// <summary>
	/// Partial executor template for Configuration initialization
	/// </summary>
	public partial class VbExecutorTemplate
	{
		/// <summary>
		/// Configuration for compilation
		/// </summary>
		public ITemplateConfiguration Configuration { get; private set; }

		/// <summary>
		/// Hidden default constructor
		/// </summary>
		private VbExecutorTemplate() { }

		/// <summary>
		/// Default template constructor
		/// </summary>
		/// <param name="configuration"></param>
		public VbExecutorTemplate(ITemplateConfiguration configuration)
		{
			Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

			if (configuration.GetActions() == null || !configuration.GetActions().Any())
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
