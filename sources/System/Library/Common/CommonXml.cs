//Sharpcms.net is licensed under the open source license GPL - GNU General Public License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using System.IO;


namespace InventIt.SiteSystem.Library
{
	public static class CommonXml
	{
		public static string TransformXsl(string xsl, XmlDocument document,Cache cache)
		{
			XslCompiledTransform transform = GetTransform(xsl, cache);

			MemoryStream memoryStream = new MemoryStream();

			XmlWriterSettings ws = new XmlWriterSettings();
			ws.Encoding = Encoding.Unicode;
            ws.OmitXmlDeclaration = true;
            
			ws.Indent = true;

            TextWriter test = new StringWriter();

            transform.Transform(document,null,test);
            return test.ToString();
		}

        public static List<string> GetXslIncludes(string xsl)
        {
            List<string> includes = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(xsl);

            XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
            manager.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");
            XmlNodeList nodeList = doc.SelectNodes("//xsl:include", manager);
            foreach (XmlNode node in nodeList)
            {
                string filename = GetAttributeValue(node, "href");
                string fullFilename = Common.CombinePaths(Path.GetDirectoryName(xsl), filename);
                includes.Add(fullFilename);

                List<string> additionalIncludes = GetXslIncludes(fullFilename);

                if (additionalIncludes != null && additionalIncludes.Count > 0)
                {
                    foreach (string file in additionalIncludes)
                    {
                        includes.Add(file);
                    }
                }
            }

            return includes;
        }

        public static void MergeXml(XmlDocument baseDocument, XmlDocument xmlDocument, params string[] uniqueIdentifiers)
        {
            doMergeXml(baseDocument.DocumentElement, xmlDocument.DocumentElement, uniqueIdentifiers);
        }

        private static void doMergeXml(XmlNode baseXmlNode, XmlNode mergeXmlNode, params string[] uniqueIdentifiers)
        {
            foreach (XmlNode xmlNode in mergeXmlNode)
            {
                if (xmlNode.Name != "#text")
                {
                    EmptyNodeHandling emptyNode; 
                    if (Common.StringArrayContains(uniqueIdentifiers, xmlNode.Name))
                    {
                        emptyNode = EmptyNodeHandling.ForceCreateNew;
                    }
                    else
                    {
                        emptyNode = EmptyNodeHandling.CreateNew;
                    }

                    XmlNode currentBaseXmlnode = GetNode(baseXmlNode, xmlNode.Name, emptyNode);
                    foreach (XmlAttribute xmlAttribute in xmlNode.Attributes)
                    {
                        CommonXml.SetAttributeValue(currentBaseXmlnode, xmlAttribute.Name, xmlAttribute.Value);
                    }
                    if (xmlNode.HasChildNodes)
                    {
                        doMergeXml(currentBaseXmlnode, xmlNode, uniqueIdentifiers);
                    }
                }
                else
                {
                    XmlNode textNode = baseXmlNode.SelectSingleNode("text()");
                    if (textNode != null)
                    {
                        textNode.InnerText = xmlNode.Value;
                    }
                    else
                    {
                        baseXmlNode.AppendChild(baseXmlNode.OwnerDocument.CreateTextNode(xmlNode.Value));
                    }
                }
            }
        }

        public static XslCompiledTransform GetTransform(string xsl, Cache cache)
		{
			string cacheKey = "transform_" + Common.CleanToSafeString(xsl);
			FileInfo fileDependency = new FileInfo(xsl);

            object cacheTransform = cache[cacheKey, fileDependency];
			if (cacheTransform != null)
			{
                bool useCache = true;

                // Check included files
                Dictionary<string, string> includedFiles = cache[cacheKey + "_includeFiles"] as Dictionary<string, string>;
                foreach (string file in includedFiles.Keys)
                {
                    if (File.GetLastWriteTime(file).ToString() != includedFiles[file])
                    {
                        useCache = false;
                    }
                }

                if (useCache)
                {
                    return cacheTransform as XslCompiledTransform;
                }
			}
             
			// The file has changed or is not in memory
			XslCompiledTransform transform = new XslCompiledTransform(true);
            
            XsltSettings  xsltSettings = new XsltSettings(true, true);
            transform.Load(xsl, xsltSettings, new XmlUrlResolver());

            cache[cacheKey, fileDependency] = transform;

            List<string> includeFiles = GetXslIncludes(xsl);
            Dictionary<string, string> includeDictionary = new Dictionary<string, string>();
            foreach (string includeFile in includeFiles)
            {
                includeDictionary[includeFile] = File.GetLastWriteTime(includeFile).ToString();
            }
            cache[cacheKey + "_includeFiles"] = includeDictionary;

            return transform;
        }

        public static void SaveXmlDocument(string filename, XmlDocument document)
        {
            if (document == null)
            {
                return;
            }

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            XmlWriter writer = XmlWriter.Create(filename, writerSettings);

            document.WriteContentTo(writer);

            writer.Close();
        }

        public static string GetXPath(XmlNode node)
        {
            List<string> xPath = new List<string>();

            XmlNode currentNode = node;
            while (currentNode != node.OwnerDocument.DocumentElement)
            {
                xPath.Add(currentNode.Name);
                currentNode = currentNode.ParentNode;
            }

            xPath.Reverse();
            return string.Join("/", xPath.ToArray());
        }

        #region Attribute-handling
        public static string GetAttributeValue(XmlNode node, string name)
        {
            if (node == null)
            {
                return string.Empty;
            }

            XmlAttribute attributeNode = node.Attributes[name];
            if (attributeNode == null)
            {
                return string.Empty;
            }

            return attributeNode.Value;
        }

        public static void SetAttributeValue(XmlNode node, string name, string value)
        {
            if (node == null)
            {
                return;
            }

            XmlAttribute attribute = node.Attributes[name];
            if (attribute != null)
            {
                attribute.Value = value;
            }
            else
            {
                attribute = node.OwnerDocument.CreateAttribute(name);
                attribute.Value = value;
                node.Attributes.Append(attribute);
            }
        }

        public static void AppendAttribute(XmlNode node, string name, string value)
        {
            XmlAttribute attributeNode = node.OwnerDocument.CreateAttribute(name);
            attributeNode.Value = value;
            node.Attributes.Append(attributeNode);
        }

        public static void CopyAttributes(XmlNode copyFrom, XmlNode copyTo)
        {
            foreach (XmlAttribute fromAttribute in copyFrom.Attributes)
            {
                XmlAttribute toAttribute = copyTo.OwnerDocument.CreateAttribute(fromAttribute.Name);
                toAttribute.Value = fromAttribute.Value;
                copyTo.Attributes.Append(toAttribute);
            }
        }
        #endregion

        #region Various GetNode's
        /// <summary>
        /// Returns the requested node and creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlDocument to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlDocument document, string path)
        {
            return GetNode(document, path, EmptyNodeHandling.CreateNew);
        }

        /// <summary>
        /// Returns the requested node and optionally creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlDocument to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <param name="emptyNode">Specifies whether to create a node if it does not exist.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlDocument document, string path, EmptyNodeHandling emptyNode)
        {
            if (document == null)
            {
                return null;
            }

            // If document is empty, create the root node
            if (document.ChildNodes.Count == 0)
            {
                if (emptyNode == EmptyNodeHandling.CreateNew)
                {
                    int indexOfSlash = path.IndexOf("/");
                    string rootNodeName = path.Substring(0, indexOfSlash);
                    XmlNode rootNode = document.CreateElement(rootNodeName);
                    document.AppendChild(rootNode);
                    path = path.Substring(indexOfSlash + 1);
                }
                else
                {
                    return null;
                }
            }

            return GetNode(document.DocumentElement, path, emptyNode);
        }

        /// <summary>
        /// Returns the requested node and creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlNode to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode fromXmlNode, string path)
        {
            return GetNode(fromXmlNode, path, EmptyNodeHandling.CreateNew);
        }

        /// <summary>
        /// Returns the requested node and optionally creates it if it does not exist.
        /// </summary>
        /// <param name="document">XmlNode to get the node from.</param>
        /// <param name="path">Path for the node.</param>
        /// <param name="emptyNode">Specifies whether to create a node if it does not exist.</param>
        /// <returns></returns>
        public static XmlNode GetNode(XmlNode fromXmlNode, string path, EmptyNodeHandling emptyNode)
        {
            string[] pathParts = path.Split('/');
            pathParts[0] = RenameIntegerPath(pathParts[0]);

            XmlNode xmlNode;
            XmlNode xmlOldNode;
            if (emptyNode != EmptyNodeHandling.ForceCreateNew)
            {
                xmlNode = fromXmlNode.SelectSingleNode(pathParts[0]);
            }
            else
            {
                xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]);
                fromXmlNode.AppendChild(xmlNode);
            }

            if (xmlNode == null)
            {
                if (emptyNode == EmptyNodeHandling.CreateNew)
                {
                    xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]);
                    fromXmlNode.AppendChild(xmlNode);
                }
                else if (emptyNode == EmptyNodeHandling.Ignore)
                {
                    return null;
                }
            }

            for (int i = 1; i < pathParts.Length; i++)
            {
                string pathPart = pathParts[i];
                pathPart = RenameIntegerPath(pathPart);

                if (pathPart != string.Empty)
                {
                    xmlOldNode = xmlNode;
                    if (emptyNode != EmptyNodeHandling.ForceCreateNew)
                    {
                        xmlNode = xmlOldNode.SelectSingleNode(pathPart);
                    }
                    else
                    {
                        xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathParts[0]);
                        fromXmlNode.AppendChild(xmlNode);
                    }

                    if (xmlNode == null)
                    {
                        if (emptyNode == EmptyNodeHandling.CreateNew)
                        {
                            xmlNode = fromXmlNode.OwnerDocument.CreateElement(pathPart);
                            xmlOldNode.AppendChild(xmlNode);
                        }
                        else if (emptyNode == EmptyNodeHandling.Ignore)
                        {
                            break;
                        }
                    }
                }
            }
            return xmlNode;
        }

        public static string RenameIntegerPath(string pathPart)
        {
            if (pathPart == string.Empty)
            {
                return pathPart;
            }

            if (!char.IsDigit(pathPart[0]))
            {
                return pathPart;
            }

            return "int_" + pathPart;
        }
        #endregion
        #region Move
        public static void MoveUp(XmlNode node)
        {
            XmlNode previousSibling = node.PreviousSibling;
            if (previousSibling == null)
            {
                return;
            }

            XmlNode nodeCopy = node.CloneNode(true);
            XmlNode previousCopy = previousSibling.CloneNode(true);

            node.ParentNode.ReplaceChild(nodeCopy, previousSibling);
            node.ParentNode.ReplaceChild(previousCopy, node);
        }

        public static void MoveTop(XmlNode node)
        {
            XmlNode firsChild = node.ParentNode.FirstChild;
            if (firsChild == null || firsChild == node)
            {
                return;
            }

            //node.ParentNode.RemoveChild(node);
            node.ParentNode.InsertBefore(node, firsChild);
        }

        public static void MoveBottom(XmlNode node)
        {
            XmlNode lastChild = node.ParentNode.LastChild;
            if (lastChild == null || lastChild == node)
            {
                return;
            }

            //node.ParentNode.RemoveChild(node);
            node.ParentNode.InsertAfter(node, lastChild);
        }

        public static void MoveDown(XmlNode node)
        {
            XmlNode nextSibling = node.NextSibling;
            if (nextSibling == null)
            {
                return;
            }

            XmlNode nodeCopy = node.CloneNode(true);
            XmlNode nextCopy = nextSibling.CloneNode(true);

            node.ParentNode.ReplaceChild(nodeCopy, nextSibling);
            node.ParentNode.ReplaceChild(nextCopy, node);
        }

        public static void Move(XmlNode node, XmlNode newParent)
        {
            Copy(node, newParent);

            node.ParentNode.RemoveChild(node);
        }

        public static void Copy(XmlNode node, XmlNode newParent)
        {
            XmlNode nodeCopy = node.CloneNode(true);

            newParent.AppendChild(nodeCopy);
        }
        #endregion
    }

    public enum EmptyNodeHandling
    {
        /// <summary>
        /// If the requested node does not exist, it is created, inserted and returned. 
        /// </summary>
        CreateNew,

        /// <summary>
        /// If the requested node does not exist, null is returned.
        /// </summary>
        Ignore,

        /// <summary>
        /// Always create a new node.
        /// </summary>
        ForceCreateNew
    }
}