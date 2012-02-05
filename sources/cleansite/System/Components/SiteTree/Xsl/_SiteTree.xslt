<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sharpcms="urn:my-scripts">
  <ms:script implements-prefix="sharpcms" xmlns:ms="urn:schemas-microsoft-com:xslt" language="C#" xmlns:sharpcms="urn:my-scripts">
    <![CDATA[
      public string Escape(string text) { return text.Replace("'", @"\'"); }
	  ]]>
  </ms:script>

  <xsl:output method="html" />

  <xsl:template mode="edit" match="sitetree">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Sites</xsl:text>
        </b>
      </div>
      <div class="viewstate">

      </div>
    </div>
    <div class="menu tree_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button</xsl:text>
        </xsl:attribute>
			  <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventNew('addpage','.','Type the name of the file:');</xsl:text>
        </xsl:attribute>
        <xsl:text>Add language</xsl:text>
		  </a>
    </div>
    <div class="tree tree_body">
      <script type="text/javascript">
        <xsl:text>sitetree = new dTree('sitetree', false);</xsl:text>
        <xsl:for-each select="*">
          <xsl:call-template name="SiteTreeElement">
            <xsl:with-param name="current-path">
              <xsl:value-of select="name()" />
            </xsl:with-param>
            <xsl:with-param name="number" select="position()" />
            <xsl:with-param name="parent">
              <xsl:text>-1</xsl:text>
            </xsl:with-param>
          </xsl:call-template>
        </xsl:for-each>
        <xsl:text>document.write(sitetree);</xsl:text>
      </script>
    </div>
  </xsl:template>

  <xsl:template name="SiteTreeElement">
    <xsl:param name="current-path" />
    <xsl:param name="number" />
    <xsl:param name="parent" />
    <xsl:text>sitetree.add(</xsl:text>
    <xsl:value-of select="$number" />
    <xsl:text>, </xsl:text>
    <xsl:value-of select="$parent" />
    <xsl:text>, '</xsl:text>
    <span>
      <xsl:if test="@currentpage='true'">
        <xsl:attribute name="class">
          <xsl:text>select</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <xsl:value-of select="sharpcms:Escape(@menuname)" />
    </span>
    <xsl:text>', 'admin/page/edit/</xsl:text>
    <xsl:value-of select="sharpcms:Escape($current-path)" />
    <xsl:text>.aspx', '', '');</xsl:text>
    <xsl:text> </xsl:text>
    <xsl:for-each select="*">
      <xsl:call-template name="SiteTreeElement">
        <xsl:with-param name="current-path">
          <xsl:value-of select="$current-path" />
          <xsl:text>/</xsl:text>
          <xsl:value-of select="name()" />
        </xsl:with-param>
        <xsl:with-param name="parent" select="$number" />
        <xsl:with-param name="number" select="($number * -200) + position()" />
      </xsl:call-template>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>