<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <head>
        <base >
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>/</xsl:text>
          </xsl:attribute>
        </base>
        <title>Admin - Sharpcms.net</title>
        <link type="text/css" rel="stylesheet" href="System/Components/Admin/Styles/base.css" />
        <link type="text/css" rel="stylesheet" href="System/Components/Admin/niftycorners/niftyCorners.css" />
        <script type="text/javascript" src="System/Components/Admin/Scripts/eventhandler.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="System/Components/Admin/niftycorners/nifty.js">
          <xsl:text> </xsl:text>
        </script>
      </head>
      <body>
        <div class="login">
          <form name="systemform" method="post" encType="multipart/form-data">
            <xsl:attribute name="action">
              <xsl:choose>
                <xsl:when test="/data/query/other/redirect and not(/data/query/other/redirect = '')">
                  <xsl:value-of select="/data/query/other/redirect" />
                  <xsl:text>.aspx</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>admin.aspx</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:attribute>
            <input type="hidden" name="event_main" value="login" />
            <input type="hidden" name="event_mainvalue" value="" />
            <input type="hidden" name="process">
              <xsl:attribute name="value">
                <xsl:choose>
                  <xsl:when test="/data/query/other/redirect and not(/data/query/other/redirect = '')">
                    <xsl:value-of select="/data/query/other/redirect" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>frontpage</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </input>
            <table align="center" cellspacing="0" cellpadding="0">
              <thead>
                <tr>
                  <td colspan="2">
                    <img src="System/Components/Admin/Images/logo.gif" alt="" />
                  </td>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>
                    Login
                  </td>
                  <td>
                    <input type="text" name="data_login" />
                  </td>
                </tr>
                <tr>
                  <td>
                    Password
                  </td>
                  <td>
                    <input type="password" name="data_password" />
                  </td>
                </tr>
                <tr>
                  <td>
                    <input class="submit" type="submit" value="Login" />
                  </td>
                </tr>
              </tbody>
              <tfoot>
                <xsl:if test="/data/query/other/error">
                  <tr>
                    <td colspan="2">
                      <div class="error">
                        <xsl:value-of select="/data/query/other/error" />
                      </div>
                    </td>
                  </tr>
                </xsl:if>
                <xsl:if test="/data/messages">
                  <tr>
                    <td>
                      <xsl:for-each select="/data/messages/*">
                        <div class="{@messagetype}">
                          <xsl:value-of select="." />
                        </div>
                        <br />
                      </xsl:for-each>
                    </td>
                  </tr>
                </xsl:if>
              </tfoot>
            </table>
          </form>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>