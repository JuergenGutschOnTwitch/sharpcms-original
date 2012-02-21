<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />

  <xsl:template mode="choose" match="sitetree">
    <ul id="pages" class="filetree">
      <xsl:for-each select="*">
        <xsl:call-template name="SiteTreeElementChoose">
          <xsl:with-param name="current-path">
            <xsl:value-of select="name()" />
          </xsl:with-param>
        </xsl:call-template>
      </xsl:for-each>
    </ul>
  </xsl:template>

  <xsl:template name ="SiteTreeElementChoose">
    <xsl:param name="current-path" />
    <li>
      <a>
        <xsl:attribute name="class">
          <xsl:text>hlCloseForm</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="$current-path" />
        </xsl:attribute>
        <span>
          <xsl:choose>
            <xsl:when test="*">
              <xsl:attribute name="class">
                <xsl:text>folder</xsl:text>
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="class">
                <xsl:text>file</xsl:text>
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:value-of select="@menuname" />
        </span>
      </a>
      <xsl:if test="*">
        <ul>
          <xsl:for-each select="*">
            <xsl:call-template name="SiteTreeElementChoose">
              <xsl:with-param name="current-path">
                <xsl:value-of select="$current-path" />
                <xsl:text>/</xsl:text>
                <xsl:value-of select="name()" />
              </xsl:with-param>
            </xsl:call-template>
          </xsl:for-each>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>