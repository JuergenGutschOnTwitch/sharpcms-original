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

  <xsl:include href="-PageContent.xslt" />
  <xsl:include href="-SiteTree.xslt" />
  <xsl:include href="..\..\Snippets.xslt" />

  <xsl:template match="/" xmlns="http://www.w3.org/1999/xhtml">
    <html xml:lang="de" lang="de" xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <base>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>/</xsl:text>
          </xsl:attribute>
        </base>
        <title>
          <xsl:text>[template] - </xsl:text>
          <xsl:value-of select="/data/contentplace/page/attributes/pagename" />
        </title>
        <meta http-equiv="content-type" content="text/html; charset=utf-8" />
        <meta name="keywords">
          <xsl:attribute name="content">
            <xsl:value-of select="/data/contentplace/page/attributes/metakeywords" />
          </xsl:attribute>
        </meta>
        <meta name="description">
          <xsl:attribute name="content">
            <xsl:value-of select="/data/contentplace/page/attributes/metadescription" />
          </xsl:attribute>
        </meta>
        <meta name="author" content="[author]" />
        <meta name="generator" content="sharpcms" />
        <meta name="revisit-after" content="7 days" />
        <meta name="robots" content="index, follow" />
        <link id="styleLink" rel="stylesheet" type="text/css">
          <xsl:attribute name="href">
            <xsl:text>/Custom/Components/FrontEnd/Styles/base.css</xsl:text>
          </xsl:attribute>
        </link>
      </head>
      <body>
        <div id="page">
          <div id="center">
            <div id="header">
              <a>
                <xsl:attribute name="class">
                  <xsl:text>left</xsl:text>
                </xsl:attribute>
                <xsl:attribute name="href">
                  <xsl:value-of select="/data/basepath" />
                  <xsl:text>/</xsl:text>
                </xsl:attribute>
                <img>
                  <xsl:attribute name="src">
                    <xsl:text>/Custom/Components/FrontEnd/Images/slogo.gif</xsl:text>
                  </xsl:attribute>
                </img>
              </a>
              <xsl:choose>
                <xsl:when test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
                  <xsl:call-template name="LogOut" />
                </xsl:when>
                <xsl:otherwise>
                  <a>
                    <xsl:attribute name="class">
                      <xsl:text>right</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                      <xsl:value-of select="/data/basepath" />
                      <xsl:text>/login/</xsl:text>
                      <xsl:if test="/data/attributes/pagepath != ''">
                        <xsl:text>?redirect=show/</xsl:text>
                        <xsl:value-of select="/data/attributes/pagepath" />
                      </xsl:if>
                    </xsl:attribute>
                    <span>Log in</span>
                  </a>
                </xsl:otherwise>
              </xsl:choose>
            </div>
            <xsl:call-template name="TopMenu" />
            <div id="content">
              <xsl:apply-templates mode="show" select="/data/contentplace/page/containers/container[@name='content']" />
            </div>
            <div id="footer">
              <p>
                <xsl:text>Powered by </xsl:text>
                <a>
                  <xsl:attribute name="href">
                    <xsl:text>http://www.sharpcms.org/</xsl:text>
                  </xsl:attribute>
                  <span>sharpcms</span>
                </a>
              </p>
            </div>
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>