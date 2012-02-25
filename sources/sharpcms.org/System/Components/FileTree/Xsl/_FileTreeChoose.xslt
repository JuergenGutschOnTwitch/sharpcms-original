<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template  mode="choose" match="filetree">
    <ul id="pages" class="filetree">
      <xsl:apply-templates mode="filetreechoose" select="folder/*">
        <xsl:with-param name="current-path" />
      </xsl:apply-templates>
    </ul>
	</xsl:template>

  <xsl:template mode="filetreechoose" match="folder">
    <xsl:param name="current-path" />
    <li>
      <xsl:choose>
        <xsl:when test="/data/query/other/process = 'admin/choose/folder'">
          <a>
            <xsl:attribute name="class">
              <xsl:text>hlCloseForm</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="value">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
            </xsl:attribute>
            <span>
              <xsl:attribute name="class">
                <xsl:text>folder</xsl:text>
              </xsl:attribute>
              <xsl:value-of select="@name" />
            </span>
          </a>
        </xsl:when>
        <xsl:otherwise>
          <span>
            <xsl:attribute name="class">
              <xsl:text>folder</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="@name" />
          </span>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:if test="*">
        <ul>
          <xsl:apply-templates mode="filetreechoose" select="*">
            <xsl:with-param name="current-path">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
              <xsl:text>/</xsl:text>
            </xsl:with-param>
          </xsl:apply-templates>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>

  <xsl:template mode="filetreechoose" match="file">
    <xsl:param name="current-path" />
    <li>
      <xsl:choose>
        <xsl:when test="/data/query/other/process = 'admin/choose/file'">
          <a>
            <xsl:attribute name="class">
              <xsl:text>hlCloseForm</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="value">
              <xsl:value-of select="$current-path" />
              <xsl:value-of select="@name" />
            </xsl:attribute>
            <span>
              <xsl:attribute name="class">
                <xsl:text>file</xsl:text>
              </xsl:attribute>
              <xsl:value-of select="@name" />
            </span>
          </a>
        </xsl:when>
        <xsl:otherwise>
          <span>
            <xsl:attribute name="class">
              <xsl:text>file</xsl:text>
            </xsl:attribute>
            <xsl:value-of select="@name" />
          </span>
        </xsl:otherwise>
      </xsl:choose>
    </li>
  </xsl:template>
</xsl:stylesheet>