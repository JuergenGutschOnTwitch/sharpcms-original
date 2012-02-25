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

  <xsl:template mode="edit" match="sitetree">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Sites</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        <xsl:text> </xsl:text>
      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlAddLanguage</xsl:text>
        </xsl:attribute>
			  <xsl:attribute name="value">
          <xsl:text>.</xsl:text>
        </xsl:attribute>
        <xsl:text>Add language</xsl:text>
		  </a>
    </div>
    <div class="tree tree_body">
      <ul id="sites" class="filetree">
        <xsl:for-each select="*">
          <xsl:call-template name="SiteTreeElement">
            <xsl:with-param name="current-path">
              <xsl:value-of select="name()" />
            </xsl:with-param>
            <xsl:with-param name="isLanguage">
              <xsl:text>true</xsl:text>
            </xsl:with-param>
          </xsl:call-template>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>

  <xsl:template name="SiteTreeElement">
    <xsl:param name="current-path" />
    <xsl:param name="isLanguage" />
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:text>/admin/page/edit/</xsl:text>
          <xsl:value-of select="$current-path" />
          <xsl:text>/</xsl:text>
        </xsl:attribute>
        <span>
          <xsl:choose>
            <xsl:when test="$isLanguage = 'true'">
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
            <xsl:call-template name="SiteTreeElement">
              <xsl:with-param name="current-path">
                <xsl:value-of select="$current-path" />
                <xsl:text>/</xsl:text>
                <xsl:value-of select="name()" />
              </xsl:with-param>
              <xsl:with-param name="isLanguage">
                <xsl:text>false</xsl:text>
              </xsl:with-param>
            </xsl:call-template>
          </xsl:for-each>
        </ul>
      </xsl:if>
    </li>
  </xsl:template>
</xsl:stylesheet>