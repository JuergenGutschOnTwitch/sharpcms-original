<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template  mode="choose" match="filetree">
    <div class="dtree">
      <script type="text/javascript">
        <xsl:text>filetreechoose = new dTree('filetreechoose',true);</xsl:text>
        <xsl:choose>
          <xsl:when test="//data/query/other/process='admin/choose/folder'">
            <xsl:apply-templates mode="choosefolders" select="folder/*">
              <xsl:with-param name="current-path"></xsl:with-param>
              <xsl:with-param name="parent">-1</xsl:with-param>
            </xsl:apply-templates>
          </xsl:when>
          <xsl:otherwise>
            <xsl:apply-templates mode="choosefiles" select="folder/*">
              <xsl:with-param name="current-path"></xsl:with-param>
              <xsl:with-param name="parent">-1</xsl:with-param>
            </xsl:apply-templates>
          </xsl:otherwise>
        </xsl:choose>
        <xsl:text>document.write(filetreechoose);</xsl:text>
			</script>
		</div>
	</xsl:template>
	
	<xsl:template mode="choosefiles" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
    <xsl:text>filetreechoose.add(</xsl:text>
    <xsl:value-of select="($parent*-200)+position()" />
    <xsl:text>,</xsl:text>
    <xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>','','','','System/Components/Admin/Icons/Tree/folder.gif');</xsl:text>
		<xsl:apply-templates mode="choosefiles" select="*">
			<xsl:with-param name="current-path">
        <xsl:value-of select="$current-path" />
        <xsl:value-of select="@name" />
        <xsl:text>/</xsl:text>
      </xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="choosefiles" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
    <xsl:text>filetreechoose.add(</xsl:text>
    <xsl:value-of select="($parent*-200)+position()" />
    <xsl:text>,</xsl:text>
    <xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>','javascript:CloseForm(\'</xsl:text>
    <xsl:value-of select="$current-path" />
    <xsl:value-of select="@name" />
    <xsl:text>\')','','');</xsl:text>
	</xsl:template>

	<xsl:template mode="choosefolders" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
    <xsl:text>filetreechoose.add(</xsl:text>
    <xsl:value-of select="($parent*-200)+position()" />
    <xsl:text>,</xsl:text>
    <xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>','javascript:CloseForm(\'</xsl:text>
    <xsl:value-of select="$current-path" />
    <xsl:value-of select="@name" />
    <xsl:text>\')','','','System/Components/Admin/Icons/Tree/folder.gif');</xsl:text>
		<xsl:apply-templates mode="choosefolders" select="*">
			<xsl:with-param name="current-path">
        <xsl:value-of select="$current-path" />
        <xsl:value-of select="@name" />
        <xsl:text>/</xsl:text>
      </xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="choosefolders" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
	</xsl:template>
</xsl:stylesheet> 
