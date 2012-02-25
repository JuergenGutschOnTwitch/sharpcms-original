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

  <xsl:template name="TopMenu">
    <div id="menu">
      <ul>
        <xsl:for-each select="data/navigationplace/sitetree/*[@inpath='true' and @status='open']/*[@status='open']">
          <li>
            <a>
              <xsl:attribute name="class">
                <xsl:choose>
                  <xsl:when test="@inpath='true'">
                    <xsl:text>select</xsl:text>
                  </xsl:when>
                </xsl:choose>
              </xsl:attribute>
              <xsl:attribute name="href">
                <xsl:text>show/</xsl:text>
                <xsl:value-of select="//data/attributes/pageroot" />
                <xsl:text>/</xsl:text>
                <xsl:value-of select="name()" />
                <xsl:text>/</xsl:text>
              </xsl:attribute>
              <span>
                <xsl:value-of select="@menuname" />
              </span>
              <span class="hide"> | </span>
            </a>
          </li>
        </xsl:for-each>
        <xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
          <li>
            <a>
              <xsl:attribute name="class">
                <xsl:choose>
                  <xsl:when test="@inpath='true'">
                    <xsl:text>select</xsl:text>
                  </xsl:when>
                </xsl:choose>
              </xsl:attribute>
              <xsl:attribute name="href">
                <xsl:value-of select="/data/basepath" />
                <xsl:text>/admin/</xsl:text>
              </xsl:attribute>
              <span>Administation</span>
              <span class="hide"> | </span>
            </a>
          </li>
        </xsl:if>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>