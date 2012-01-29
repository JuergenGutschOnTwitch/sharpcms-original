// sharpcms is licensed under the open source license GPL - GNU General Public License.

using System.Xml;
using Sharpcms.Library;
using Sharpcms.Library.Common;

namespace Sharpcms.Data.SiteTree
{
    public class Page : DataElement
    {
        private readonly ContainerList _containers;
        private readonly SiteTree _siteTree;
        private readonly XmlNode _treeNode;

        public Page(XmlNode pageNode, XmlNode treeNode, SiteTree siteTree) : base(pageNode)
        {
            _treeNode = treeNode;
            _siteTree = siteTree;

            CopyInfoFromTree();

            _containers = new ContainerList(CommonXml.GetNode(Node, "containers"));
        }

        public ContainerList Containers
        {
            get { return _containers; }
        }

        public XmlNode TreeNode
        {
            get { return _treeNode; }
        }

        public string Name
        {
            get { return this["name"]; }
        }

        public string PageIdentifier
        {
            get { return CommonXml.GetXPath(_treeNode); }
        }

        public string this[string name]
        {
            get
            {
                string xPath = string.Format("attributes/{0}", name);
                return GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText;
            }
            set
            {
                string xPath = string.Format("attributes/{0}", name);
                GetNode(xPath, EmptyNodeHandling.CreateNew).InnerText = value;
                HandleAttributeChange(name, value);
            }
        }

        public XmlNode GetAttribute(string name)
        {
            string xPath = string.Format("attributes/{0}", name);
            return GetNode(xPath, EmptyNodeHandling.CreateNew);
        }

        private void CopyInfoFromTree()
        {
            this["name"] = _treeNode.Name;
            this["menuname"] = CommonXml.GetAttributeValue(_treeNode, "menuname");
            this["pageidentifier"] = CommonXml.GetXPath(_treeNode);
            this["status"] = CommonXml.GetAttributeValue(_treeNode, "status");
        }

        private void HandleAttributeChange(string name, string value)
        {
            switch (name)
            {
                case "name":
                    _siteTree.Rename(this, value);
                    break;
                default:
                    CommonXml.SetAttributeValue(_treeNode, name, value);
                    break;
            }
            //m_SiteTree.Save();
        }

        public void Save()
        {
            _siteTree.Save();
            _siteTree.SavePage(this);
        }
    }
}