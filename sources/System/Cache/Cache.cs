using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace InventIt.SiteSystem
{
	public class Cache
	{
        public static System.Web.HttpApplicationState ApplicationState;

		public Hashtable CacheTable
		{
			get
			{
				if (ApplicationState["cache"] == null)
				{
					Clean();
				}
				return ApplicationState["cache"] as Hashtable;
			}
		}

		public object this[string key, FileInfo fileDependency]
		{
			get
			{
				object value = this[key];
				if (value == null)
				{
					return null;
				}

				string fileModified = fileDependency.LastWriteTime.ToString();
				string cacheModifiedKey = FormatModifiedKey(key);
				if (this[cacheModifiedKey] == null || this[cacheModifiedKey].ToString() != fileModified)
				{
					return null;
				}

				return value;
			}
			set
			{
				this[key] = value;

				string fileModified = fileDependency.LastWriteTime.ToString();
				string cacheModifiedKey = FormatModifiedKey(key);
				this[cacheModifiedKey] = fileModified;
			}
		}

		public object this[string key]
		{
			get
			{
				return CacheTable[key];
			}
			set
			{
				CacheTable[key] = value;
			}
		}

		public void Clean()
		{
			Hashtable cache = new Hashtable();
			ApplicationState["cache"] = cache;
		}

		private string FormatModifiedKey(string key)
		{
			return string.Format("{0}_filedependency", key);
		}
	}
}
