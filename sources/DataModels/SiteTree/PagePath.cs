//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;

namespace InventIt.SiteSystem.Data.SiteTree
{
	public class PagePath
	{
		private string m_Path;
		private string m_Name;

		public string Path
		{
			get
			{
				return m_Path;
			}
		}

		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		public PagePath(string path)
		{
			int lastPathPosition = path.LastIndexOf("/");
			if (lastPathPosition > 0)
			{
				m_Path = path.Substring(0, lastPathPosition);
				m_Name = path.Substring(lastPathPosition + 1);
			}
			else
			{
				m_Path = string.Empty;
				m_Name = path;
			}
		}
	}
}
