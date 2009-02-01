<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet 
		version="1.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		xmlns="http://www.w3.org/1999/xhtml">
	<xsl:include href="..\..\..\..\Custom\Components\Snippets.xslt" />
	<xsl:template match="/">
		<html>
			<head>
        <base>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>/</xsl:text>
          </xsl:attribute>
        </base>
				<link href="System/Components/Admin/Xsl/style.css" rel="stylesheet" type="text/css" />
				<link rel="StyleSheet" href="System/Components/Admin/Scripts/tree/dtree.css" type="text/css" />
				<script type="text/javascript" src="System/Components/Admin/Scripts/tree/dtree.js">alert('There is something wrong.')</script>			
				<script type="text/javascript" src="System/Components/Admin/Scripts/modal.js">alert('There is something wrong.')</script>			
				<link rel="StyleSheet" href="System/Components/Admin/Scripts/tree/dtree.css" type="text/css" />
			</head>
			<body>
				<div class="top" style="height:30px;font-size:18px;padding:6px;">Choose</div>
				<div class="choosecontent"><xsl:apply-templates select="data/contentone/*" mode="choose" /></div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>