// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;

namespace Sharpcms.Library.Users
{
    public class GroupList : DataElementList
    {
        public GroupList(XmlNode parentNode) : base(parentNode)
        {
        }

        public Group this[int index]
        {
            get
            {
                string xPath = string.Format("group[{0}]", index + 1);
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    var group = new Group(node);
                    return group;
                }
                return null;
            }
        }

        public Group this[string name]
        {
            // ToDo: group is not consistent yet (old)
            get
            {
                string xPath = string.Format("group[@name='{0}']", Common.Common.CleanToSafeString(name));
                XmlNode node = ParentNode.SelectSingleNode(xPath);

                if (node != null)
                {
                    var group = new Group(node);
                    return group;
                }
                return null;
            }
        }

        public Group Create(string name)
        {
            XmlNode node = Document.CreateElement("group");
            ParentNode.AppendChild(node);
            var group = new Group(node) {Name = name};
            return group;
        }

        public void Remove(int index) //ToDo: Is a unused Method (T.Huber 18.06.2009)
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
    }
}