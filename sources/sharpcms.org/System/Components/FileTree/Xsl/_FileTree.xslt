<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />

  <xsl:template  mode="edit" match="filetree">
    <div class="head tree_head">
      <div class="title">
        <b>
          <xsl:text>Directory</xsl:text>
        </b>
      </div>
      <div class="viewstate">
        
      </div>
    </div>
    <div class="menu tree_menu">
      <a class="button">
        <xsl:attribute name="class">
          <xsl:text>button</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventNew('addfolder','.','Type the name of the new folder');</xsl:text>
        </xsl:attribute>
        <xsl:text>Add new folder</xsl:text>
      </a>
    </div>
    <div class="tree tree_body">
		  <script type="text/javascript">
        <xsl:text>filetree = new dTree('filetree',true);</xsl:text>
			  <xsl:apply-templates mode="filetree" select="folder/*">
				  <xsl:with-param name="current-path" ></xsl:with-param>
				  <xsl:with-param name="parent" select="-1" />
			  </xsl:apply-templates>
        <xsl:text>document.write(filetree);</xsl:text>
		  </script>
	  </div>
	</xsl:template>
	
	<xsl:template mode="filetree" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
    <xsl:text>filetree.add(</xsl:text>
    <xsl:value-of select="($parent*-200)+position()" />
    <xsl:text>,</xsl:text>
				<xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>','admin/file/edit/folder/</xsl:text>
    <xsl:value-of select="$current-path" />
    <xsl:value-of select="@name" />
    <xsl:text>/','','','System/Components/Admin/Icons/Tree/folder.gif');</xsl:text>
		<xsl:apply-templates mode="filetree" select="*">
			<xsl:with-param name="current-path">
        <xsl:value-of select="$current-path" />
        <xsl:value-of select="@name" />
        <xsl:text>/</xsl:text>
      </xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="filetree" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
    <xsl:text>filetree.add(</xsl:text>
    <xsl:value-of select="($parent*-200)+position()" />
    <xsl:text>,</xsl:text>
		<xsl:value-of select="$parent" />
    <xsl:text>,'</xsl:text>
    <xsl:value-of select="@name" />
    <xsl:text>','admin/file/edit/file/</xsl:text>
    <xsl:value-of select="$current-path" />
    <xsl:value-of select="@name" />
    <xsl:text>/','','');</xsl:text>
	</xsl:template>
</xsl:stylesheet>