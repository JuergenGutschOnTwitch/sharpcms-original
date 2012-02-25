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

  <xsl:include href="..\..\..\..\Custom\Components\Snippets.xslt" />

  <xsl:template match="/">
    <html>
      <head>
        <base>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>/</xsl:text>
          </xsl:attribute>
        </base>
        <title>
          <xsl:text>Choose</xsl:text>
        </title>
        <link type="text/css" rel="stylesheet" href="/System/Components/Admin/Styles/base.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Styles/jquery/treeview/jquery.treeview.css" />
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery-1.7.1.min.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/treeview/jquery.treeview.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/choose.xslt.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/sharpcms.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/_sharpcms.js">
          <xsl:text> </xsl:text>
        </script>
      </head>
      <body>
        <div class="choose">
          <div class="header">
            <xsl:text>Choose</xsl:text>
          </div>
          <div class="content">
            <xsl:apply-templates select="data/navigationplace/*" mode="choose" />
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>