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
  
  <xsl:template mode="choose" match="usertree">
    <div id="chooseUserDialog" class="choose" title="Choose User">
      <ul id="users" class="filetree">
        <xsl:for-each select="user">
          <li>
            <a>
              <xsl:attribute name="class">
                <xsl:text>hlCloseDialog</xsl:text>
              </xsl:attribute>
              <xsl:attribute name="value">
                <xsl:value-of select="login" />
              </xsl:attribute>
              <span>
                <xsl:attribute name="class">
                  <xsl:text>user</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="login" />
              </span>
            </a>
          </li>
        </xsl:for-each>
      </ul>
    </div>
  </xsl:template>
</xsl:stylesheet>