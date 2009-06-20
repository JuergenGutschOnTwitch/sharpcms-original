<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
		version="1.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		xmlns="http://www.w3.org/1999/xhtml">
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
        <link type="text/css" rel="stylesheet" href="System/Components/Admin/Styles/base.css" />
        <link type="text/css" rel="StyleSheet" href="System/Components/Admin/Scripts/tree/dtree.css" />
        <script type="text/javascript" src="System/Components/Admin/Scripts/tree/dtree.js">alert('There is something wrong.');</script>
        <script type="text/javascript" src="System/Components/Admin/Scripts/modal.js">alert('There is something wrong.');</script>
      </head>
      <body>
        <div class="choose">
          <div class="header">
            <xsl:text>Choose</xsl:text>
          </div>
          <div class="content">
            <xsl:apply-templates select="data/contentone/*" mode="choose" />
          </div>
        </div>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>