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
  
  <xsl:template mode="edit" match="changelog">
    <b>
      <xsl:text>Changelog</xsl:text>
    </b>
    <br/>
    <xsl:text>This is the changelog - to keep you updated with changes on your website.</xsl:text>
    <br/>
    <br/>
    <div style="width: 400px;">
      <xsl:for-each select="log">
        <div class="elementtop">
          <xsl:value-of select="date" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="category" />
        </div>
        <div class="element">
          <xsl:value-of select="text" />
        </div>
      </xsl:for-each>
    </div>
  </xsl:template>
</xsl:stylesheet>