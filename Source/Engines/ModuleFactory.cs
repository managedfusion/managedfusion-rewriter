using System;
using System.Collections.Generic;
using System.Text;
using ManagedFusion.Rewriter.Configuration;

namespace ManagedFusion.Rewriter.Engines
{
	public class ModuleFactory
	{
		private Dictionary<string, Type> _localModules;

		public ModuleFactory()
		{
			_localModules = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);
		}

		public void AddModule(string name, Type type)
		{
			_localModules.Add(name, type);
		}

		public bool ContainsName(string name)
		{
			return GetModule(name) != null;
		}

		public Type GetModule(string name)
		{
			// check the local modules
			if (_localModules.ContainsKey(name))
				return _localModules[name];

			// check the global modules
			for (int i = 0; i < Manager.Configuration.Rewriter.Modules.Count; i++)
			{
				RewriterModuleItem item = Manager.Configuration.Rewriter.Modules[i];

				if (String.Equals(name, item.Name, StringComparison.CurrentCultureIgnoreCase))
					return item.Type;
			}

			return null;
		}
	}
}
