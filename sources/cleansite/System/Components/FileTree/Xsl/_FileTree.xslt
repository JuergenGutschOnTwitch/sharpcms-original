<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" />

	<xsl:template mode="edit" match="filetree">
    <div class="head tree_head">
      <div class="title">
        Directory
      </div>
      <div class="viewstate">
        
      </div>
    </div>
    <div class="tree tree_body">
		  <script type="text/javascript">
			  filetree = new dTree('filetree',true);
			  <xsl:apply-templates mode="filetree" select="folder/*">
				  <xsl:with-param name="current-path" ></xsl:with-param>
				  <xsl:with-param name="parent" select="-1" />
			  </xsl:apply-templates>
			  document.write(filetree);
		  </script>
	  </div>
	</xsl:template>
	
	<xsl:template mode="filetree" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
			filetree.add(<xsl:value-of select="($parent*-200)+position()" />,
				<xsl:value-of select="$parent" />,
				'<xsl:value-of select="@name" />'
				,'admin/file/edit/folder/<xsl:value-of select="$current-path" /><xsl:value-of select="@name" />.aspx',
				'','','System/Components/Admin/Icons/Tree/folder.gif');
		<xsl:apply-templates mode="filetree" select="*">
			<xsl:with-param name="current-path"><xsl:value-of select="$current-path" /><xsl:value-of select="@name" />/</xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="filetree" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
			filetree.add(<xsl:value-of select="($parent*-200)+position()" />,
				<xsl:value-of select="$parent" />,
				'<xsl:value-of select="@name" />'
				,'admin/file/edit/file/<xsl:value-of select="$current-path" /><xsl:value-of select="@name" />.aspx','','');
	</xsl:template>
</xsl:stylesheet>