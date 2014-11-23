<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template name="TopMenu">
    <div id="menu">
      <ul>
        <xsl:for-each select="data/navigationplace/sitetree/*[@inpath='true' and @status='open']/*[@status='open']">
          <li>
            <a>
              <xsl:if test="@inpath='true'">
                <xsl:attribute name="class">
                  <xsl:text>select</xsl:text>
                </xsl:attribute>
              </xsl:if>
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
            </a>
          </li>
        </xsl:for-each>
        <xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
          <li>
            <a>
              <xsl:if test="@inpath='true'">
                <xsl:attribute name="class">
                  <xsl:text>select</xsl:text>
                </xsl:attribute>
              </xsl:if>                
              <xsl:attribute name="href">
                <xsl:value-of select="/data/basepath" />
                <xsl:text>/admin/</xsl:text>
              </xsl:attribute>
              <span>Administation</span>
            </a>
          </li>
        </xsl:if>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>