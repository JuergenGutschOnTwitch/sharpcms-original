using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using InventIt.SiteSystem.Library;

namespace InventIt.SiteSystem.Data.Users
{
    public class Users 
    {
        private XmlDocument m_UserDocument;
        private string m_UserFileName;
        private Process m_Process;
        private UserList m_UserList;
        private GroupList m_GroupList;

        public UserList UserList
        {
            get
            {
                return m_UserList;
            }
        }
        
        public GroupList GroupList
        {
            get
            {
                return m_GroupList;
            }
        }

        public Users(Process process)
        {
            m_Process = process;
            m_UserFileName = process.Settings["users/filename"];
            m_UserDocument = new XmlDocument();
            m_UserDocument.Load(m_UserFileName);
            m_UserList = new UserList(CommonXml.GetNode(m_UserDocument.DocumentElement, "users"));
            m_GroupList = new GroupList(CommonXml.GetNode(m_UserDocument.DocumentElement, "groups"));
        }

        public void Save()
        {
            m_UserDocument.Save(m_UserFileName);
        }
    }
}
