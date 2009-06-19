using System;
using System.Collections.Generic;
using System.Text;
using InventIt.SiteSystem.Library;
using System.Xml;

namespace InventIt.SiteSystem.Data.Users
{
    public class GroupList : DataElementList
    {
        public GroupList(XmlNode parentNode)
			: base(parentNode)
		{
		}

		public Group Create(string name)
		{
			XmlNode node = Document.CreateElement("group");
			ParentNode.AppendChild(node);
			Group group = new Group(node);
            group.Name = name;
            return group;
		}
        
        public void Remove(int index)
        {
            Group group = this[index];
            ParentNode.RemoveChild(group.Node);
        }
        public void Remove(string name)
        {
            Group group = this[name];
            ParentNode.RemoveChild(group.Node);
        }
        public void Clear()
        {
            ParentNode.RemoveAll();
        }
		public Group this[int index]
		{
			get
			{
				string xPath = string.Format("group[{0}]", index + 1);
				XmlNode node = ParentNode.SelectSingleNode(xPath);

				if (node != null)
				{
                    Group group = new Group(node);
                    return group;
				}
				return null;
			}
		}
        public Group this[string name]
        {// TODO: group is not consistent yet
            get
            {
                string xPath = string.Format("group[@name='{0}']",Common.CleanToSafeString(name) );
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    Group group = new Group(node);
                    return group;
                }
                return null;
            }
        }
    }
}
