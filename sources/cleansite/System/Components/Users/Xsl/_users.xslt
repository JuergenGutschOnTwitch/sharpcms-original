<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template mode="edit" match="users">
    <div class="head tree_head">
      <div class="title">Users</div>
      <div class="viewstate">

      </div>
    </div>
    <div class="menu tree_menu">
      <a class="button" href="javascript:ThrowEventNew('adduser','','Type the name of the new user');">Add user</a>
    </div>
    <div class="tree tree_body">
      <ul>
        <xsl:for-each select="user">
          <li>
            <a>
              <xsl:attribute name="href">
                <xsl:text>default.aspx?process=admin/user/</xsl:text>
                <xsl:value-of select="login" />
              </xsl:attribute>
              <xsl:value-of select="login" />
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template mode="edit" match="groups">
    <div class="head tree_head">
      <div class="title">Groups</div>
      <div class="viewstate">

      </div>
    </div>
    <div class="menu tree_menu">
      <a class="button" href="javascript:ThrowEventNew('addgroup','','Type the name of the new group');">Add group</a>
    </div>
    <div class="tree tree_body">
      <ul>
        <xsl:for-each select="group">
          <li>
            <xsl:value-of select="@name" />
            <a>
              <xsl:attribute name="href">
                <xsl:text>javascript:ThrowEventConfirm('deletegroup','</xsl:text>
                <xsl:value-of select="@name" />
                <xsl:text>','Are you sure you wnat to delete the group?');</xsl:text>
              </xsl:attribute>
              <xsl:text> Delete</xsl:text>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template mode="edit" match="user">
    <div class="head userdata_head">
      <div class="title">
        User: <xsl:value-of select="login" />
      </div>
      <div class="viewstate">
        <p id="usda_vs">˅</p>
      </div>
    </div>
    <div class="menu userdata_menu">
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEvent('saveuser','</xsl:text>
          <xsl:value-of select="login" />
          <xsl:text>');</xsl:text>
        </xsl:attribute>
        Save
      </a>
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventConfirm('deleteuser','</xsl:text>
          <xsl:value-of select="login" />
          <xsl:text>','Are you sure you wnat to delete the user?');</xsl:text>
        </xsl:attribute>
        Delete user
      </a>
    </div>
    <div class="body userdata_body" id="usda_body">
      <table>
        <tr>
          <td>
            Login
          </td>
          <td>
            <input name="data_user_login" type="text">
              <xsl:attribute name="value">
                <xsl:value-of select="login" />
              </xsl:attribute>
            </input>
          </td>
        </tr>
        <tr>
          <td>
            Password
          </td>
          <td>
            <input name="data_user_password" type="password" value="emptystring" />
          </td>
        </tr>
        <tr>
          <td>Groups</td>
          <td>
            <xsl:variable select="groups" name="groups" />
            <xsl:for-each select="//data/contentone/groups/group">
              <xsl:variable select="@name" name="name" />

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
                    checked
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
            </xsl:for-each>
          </td>
        </tr>
      </table>
    </div>
  </xsl:template>
</xsl:stylesheet>