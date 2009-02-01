<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template  mode="choose" match="filetree">
		<div class="dtree">
			<script type="text/javascript">
				filetreechoose = new dTree('filetreechoose',true);
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
        document.write(filetreechoose);
			</script>
		</div>
	</xsl:template>
	
	<xsl:template mode="choosefiles" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
			filetreechoose.add(<xsl:value-of select="($parent*-200)+position()" />,
				<xsl:value-of select="$parent" />,
				'<xsl:value-of select="@name" />'
				,'','','','System/Components/Admin/Icons/Tree/folder.gif');
		<xsl:apply-templates mode="choosefiles" select="*">
			<xsl:with-param name="current-path"><xsl:value-of select="$current-path" /><xsl:value-of select="@name" />/</xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="choosefiles" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
			filetreechoose.add(<xsl:value-of select="($parent*-200)+position()" />,
			<xsl:value-of select="$parent" />,
			'<xsl:value-of select="@name" />'
			,'javascript:CloseForm(\'<xsl:value-of select="$current-path" /><xsl:value-of select="@name" />\')','','');
	</xsl:template>

	<xsl:template mode="choosefolders" match="folder">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
			filetreechoose.add(<xsl:value-of select="($parent*-200)+position()" />,
				<xsl:value-of select="$parent" />,
				'<xsl:value-of select="@name" />'
				,'javascript:CloseForm(\'<xsl:value-of select="$current-path" /><xsl:value-of select="@name" />\')',
				'','','System/Components/Admin/Icons/Tree/folder.gif');
		<xsl:apply-templates mode="choosefolders" select="*">
			<xsl:with-param name="current-path"><xsl:value-of select="$current-path" /><xsl:value-of select="@name" />/</xsl:with-param>
			<xsl:with-param name="parent" select="($parent*-200)+position()" />
		</xsl:apply-templates>
	</xsl:template>
	
	<xsl:template mode="choosefolders" match="file">
		<xsl:param name="current-path" />
		<xsl:param name="parent" />
	</xsl:template>
</xsl:stylesheet> 

