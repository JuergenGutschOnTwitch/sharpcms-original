<xsl:stylesheet
		version="1.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		xmlns="http://www.w3.org/1999/xhtml">

  <xsl:template mode="edit" match="changelog">
    <b>Changelog</b>
    <br/>
    This is the changelog - to keep you updated with changes on your website.
    <br/>
    <br/>
    <div style="width:400px;">
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