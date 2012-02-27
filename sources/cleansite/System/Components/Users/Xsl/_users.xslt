<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output
		method="html"
		doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
		doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
		omit-xml-declaration="yes"
		indent="yes" />
  
  <xsl:template mode="edit" match="users">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Users</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddUser</xsl:text>
        </xsl:attribute>
        <xsl:text>Add user</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
      <ul id="users" class="filetree">
        <xsl:for-each select="user">
          <li>
            <a>
              <xsl:attribute name="href">
                <xsl:text>/admin/user/</xsl:text>
                <xsl:value-of select="login" />
              </xsl:attribute>
              <span>
                <xsl:attribute name="class">
                  <xsl:text>user</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="login" />
              </span>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template mode="edit" match="groups">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Groups</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddGroup</xsl:text>
        </xsl:attribute>
        <xsl:text>Add group</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
      <ul id="groups" class="filetree">
        <xsl:for-each select="group">
          <li>
            <a>
              <xsl:attribute name="class">
                <xsl:text>hlDeleteGroup</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="@name" />
              </xsl:attribute>
              <span>
                <xsl:attribute name="class">
                  <xsl:text>group</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="@name" />
                <xsl:text> Delete</xsl:text>
              </span>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template mode="edit" match="user">
    <div class="head userdata_head">
      <div class="title">
        <b>
          <xsl:text>User: </xsl:text>
          <xsl:value-of select="login" />
        </b>
      </div>
      <div class="viewstate">
        <a id="usda_vs" class="button expand">
          <xsl:text>˅</xsl:text>
        </a>
      </div>
    </div>
    <div class="menu userdata_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlSaveUser</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="login" />
        </xsl:attribute>
        <xsl:text>Save</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlDeleteUser</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="login" />
        </xsl:attribute>
        <xsl:text>Delete user</xsl:text>
      </a>
    </div>
    <div class="tab-pane" id="usda_body" style="float: left;">
      <div id="usda_body_tabs">
        <ul>
          <li>
            <a href="#ptabs1">
              <xsl:text>User Data</xsl:text>
            </a>
          </li>
        </ul>
        <div id="ptabs1" class="tab-page">
          <div class="tab_page_body">
            <label>
              <xsl:text>Login</xsl:text>
            </label>
            <div class="item">
              <input>
                <xsl:attribute name="name">
                  <xsl:text>data_user_login</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="type">
                  <xsl:text>text</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="value">
                  <xsl:value-of select="login" />
                </xsl:attribute>
              </input>
            </div>
            <label>
              <xsl:text>Password</xsl:text>
            </label>
            <div class="item">
              <input>
                <xsl:attribute name="name">
                  <xsl:text>data_user_password</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="type">
                  <xsl:text>password</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="value">
                  <xsl:text>emptystring</xsl:text>
                </xsl:attribute>
              </input>
            </div>
            <label>
              <xsl:text>Groups</xsl:text>
            </label>
            <div class="item">
              <ul class="checkbox_list">
                <xsl:variable select="groups" name="groups" />
                <xsl:for-each select="//data/navigationplace/groups/group">
                  <xsl:variable select="@name" name="name" />
                  <li>
                    <input class="checkbox" type="checkbox" name="data_user_groups">
                      <xsl:attribute name="id">
                        <xsl:text>data_user_groups_</xsl:text>
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:attribute name="value">
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:if test="$groups/group[@name=$name]">
                        <xsl:attribute name="checked">
                          <xsl:text>checked</xsl:text>
                        </xsl:attribute>
                      </xsl:if>
                    </input>
                    <label>
                      <xsl:attribute name="for">
                        <xsl:text>data_user_groups_</xsl:text>
                        <xsl:value-of select="@name" />
                      </xsl:attribute>
                      <xsl:value-of select="@name" />
                    </label>
                  </li>
                </xsl:for-each>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </xsl:template>
</xsl:stylesheet>