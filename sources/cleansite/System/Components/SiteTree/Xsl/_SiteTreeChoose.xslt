<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />

  <xsl:template mode="choose" match="sitetree">
    <div class="dtree">
      <script type="text/javascript">
        <xsl:text>sitetreechoose = new dTree('sitetreechoose',false);</xsl:text>
        <xsl:for-each select="*">
          <xsl:call-template name="SiteTreeElementChoose">
            <xsl:with-param name="current-path">
              <xsl:value-of select="name()" />
            </xsl:with-param>
            <xsl:with-param name="number" select="position()" />
            <xsl:with-param name="parent" >-1</xsl:with-param>
          </xsl:call-template>
        </xsl:for-each>
        <xsl:text>document.write(sitetreechoose);</xsl:text>
      </script>
    </div>
  </xsl:template>

  <xsl:template name ="SiteTreeElementChoose">
    <xsl:param name="current-path" />
    <xsl:param name="number" />
    <xsl:param name="parent" />
    <xsl:text>sitetreechoose.add(</xsl:text>
    <xsl:value-of select="$number" />
    <xsl:text>,</xsl:text>
    <xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="name()" />
    <xsl:text>','javascript:CloseForm(\'</xsl:text>
    <xsl:value-of select="$current-path" />
    <xsl:text>\')','','');</xsl:text>
    <xsl:for-each select="*">
      <xsl:call-template name="SiteTreeElementChoose">
        <xsl:with-param name="current-path">
          <xsl:value-of select="$current-path" />
          <xsl:text>/</xsl:text>
          <xsl:value-of select="name()" />
        </xsl:with-param>
        <xsl:with-param name="parent" select="$number" />
        <xsl:with-param name="number" select="($number*-200)+position()" />
      </xsl:call-template>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>