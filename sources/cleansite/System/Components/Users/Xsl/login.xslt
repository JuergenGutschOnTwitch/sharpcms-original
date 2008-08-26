<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <html>
      <header>
        <base >
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath"/>
            <xsl:text>/</xsl:text>
          </xsl:attribute>

        </base>
        <title>Admin - Sharpcms.net</title>
        <script type="text/javascript" src="System/Components/Admin/Scripts/eventhandler.js">
          <xsl:text> </xsl:text>
        </script>
        <link href="System/Components/Admin/Xsl/style.css" rel="stylesheet" type="text/css" />
        <link rel="stylesheet" type="text/css" href="System/Components/Admin/niftycorners/niftyCorners.css"/>
        <script type="text/javascript" src="System/Components/Admin/niftycorners/nifty.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript">
          <![CDATA[
				window.onload=function(){
				if(!NiftyCheck())
					return;
			
					Rounded("div#topdiv","top","#393439","#ffffff","smooth");
					Rounded("div#topdiv","bottom","#393439","#ffffff","smooth");

				}
				]]>
        </script>
      </header>
      <body>
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
          <input type="hidden" name="event_main" value="login"/>
          <input type="hidden" name="event_mainvalue" value=""/>
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
          <br/>
          <br/>
          <table align="center" width="200" cellspacing="0" cellpadding="6" style="background:white;border:1px solid black;">
            <tr>
              <td colspan="2" style="padding:0px;background:#628498; background-image:url(System/Components/Admin/Picture/topbg.gif);">
                <img src="System/Components/Admin/Picture/logo.gif"/>
              </td>
            </tr>
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
            <tr>
              <td>
                Login
              </td>
              <td>
                <input type="text" name="data_login"/>
              </td>
            </tr>
            <tr>
              <td>
                Password
              </td>
              <td>
                <input type="password" name="data_password"/>
              </td>
            </tr>
            <tr>
              <td>
                <input type="submit" value="Login" style="align:center;width:80px;background:white;padding:2px;"/>
              </td>
            </tr>
          </table>
        </form>
      </body>
    </html>
  </xsl:template>

</xsl:stylesheet>