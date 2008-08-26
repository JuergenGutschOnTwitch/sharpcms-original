using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.SiteTree
{
    public class Page : DataElement
    {
		private ContainerList m_Containers;
		private SiteTree m_SiteTree;
		private XmlNode m_TreeNode;
             
		public ContainerList Containers
		{
			get
			{
				return m_Containers;
			}
		}

		public XmlNode TreeNode
		{
			get
			{
				return m_TreeNode;
			}
		}

		public string Name
		{
			get
			{
				return this["name"];
			}
			set
			{
				this["name"] = value;
			}
		}

		public string MenuName
		{
			get
			{
				return this["menuname"];
			}
			set
			{
				this["menuname"] = value;
			}
		}

		public string PageIdentifier
		{
			get
			{
				return CommonXml.GetXPath(this.m_TreeNode);
			}
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
        public XmlNode getAttribute(string name)
        {
            string xPath = string.Format("attributes/{0}", name);
            return GetNode(xPath, EmptyNodeHandling.CreateNew);
        }

		public Page(XmlNode pageNode, XmlNode treeNode, SiteTree siteTree)
			: base(pageNode)
		{
			m_TreeNode = treeNode;
			m_SiteTree = siteTree;

			CopyInfoFromTree();

			m_Containers = new ContainerList(CommonXml.GetNode(Node,"containers"));
		}

		private void CopyInfoFromTree()
		{
			this["name"] = m_TreeNode.Name;
			this["menuname"] = CommonXml.GetAttributeValue(m_TreeNode, "menuname");
			this["pageidentifier"] = CommonXml.GetXPath(m_TreeNode);
            this["status"] = CommonXml.GetAttributeValue(m_TreeNode, "status");
		}

		private void HandleAttributeChange(string name, string value)
		{
			switch (name)
			{
                case "name":
                    m_SiteTree.Rename(this, value);
                    break;
                default:
                     CommonXml.SetAttributeValue(m_TreeNode, name, value);
                     break;

			}
            //m_SiteTree.Save();
		}

		public void Save() 
		{
            m_SiteTree.Save();
			m_SiteTree.SavePage(this);
		}
    }
}