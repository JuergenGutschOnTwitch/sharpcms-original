<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template name="TopMenu">
		<ul class="topmenu">
			<xsl:for-each select="data/contentone/sitetree/*[@inpath='true' and @status='open']/*[@status='open']">
				<li>
					<xsl:attribute name="class">
						<xsl:choose>
							<xsl:when test="@inpath='true'">current</xsl:when>
						</xsl:choose>
					</xsl:attribute>
					
						<a>
							<xsl:attribute name="class">
								<xsl:choose>
									<xsl:when test="@inpath='true'">current</xsl:when>
								</xsl:choose>
							</xsl:attribute>
							<xsl:attribute name="href">show/<xsl:value-of select="//data/attributes/pageroot"/>/<xsl:value-of select="name()"/>.aspx</xsl:attribute>
							<xsl:value-of select="@menuname"/>
						</a>
					
				</li>
			</xsl:for-each>
		</ul>
	</xsl:template>

	<xsl:template name="SubMenu">
		<xsl:for-each select="data/contentone/sitetree/*[@inpath='true' and @status='open']/*[@inpath='true' and @status='open']">

			<ul class="submenu">

				<xsl:apply-templates mode="submenu" select="*[@status='open']">
					<xsl:with-param name="path">
						<xsl:value-of select="name(..)"/>
						<xsl:text>/</xsl:text>
						<xsl:value-of select="name()"/>
					</xsl:with-param>
				</xsl:apply-templates>
			</ul>
		</xsl:for-each>
	</xsl:template>
	
	<xsl:template match="*" mode="submenu">
		<xsl:param name="path"/>
		<li>
			<xsl:attribute name="class">
				<xsl:choose>
					<xsl:when test="@inpath='true'">current</xsl:when>
				</xsl:choose>
			</xsl:attribute>

			<a>
				<xsl:attribute name="class">
					<xsl:choose>
						<xsl:when test="@inpath='true'">current</xsl:when>
					</xsl:choose>
				</xsl:attribute>
				<xsl:attribute name="href">show/<xsl:value-of select="$path"/>/<xsl:value-of select="name()"/>.aspx</xsl:attribute>
				<xsl:value-of select="@menuname"/>
			</a>

			<ul class="submenu">
				<xsl:text> </xsl:text>
				<xsl:apply-templates select="child::*[(../@inpath='true' or @inpath='true') and @status='open']" mode="submenu">
					<xsl:with-param name="path">
						<xsl:value-of select="$path"/>/<xsl:value-of select="name()"/>
					</xsl:with-param>
				</xsl:apply-templates>
			</ul>
		</li>
	</xsl:template>
	
	<xsl:template name="MenuTrail">
		<xsl:for-each select="data/contentone/sitetree">
			<xsl:call-template name="MenuTrailElement">
				<xsl:with-param name="level">0</xsl:with-param>
			</xsl:call-template>

		</xsl:for-each>
	</xsl:template>

	<xsl:template name="MenuTrailElement">
		<xsl:param name="currentpath"/>
		<xsl:param name="level"/>
			<xsl:for-each select="*">
				<xsl:if test="@inpath='true'">
					 <a>&#187;</a><a >
						<xsl:if test="$level!=0">
							<xsl:attribute name="href">show/<xsl:value-of select="$currentpath"/><xsl:value-of select="name()"/>.aspx</xsl:attribute>
						</xsl:if>
						<xsl:value-of select="@menuname"/> 
					</a>
				
					<xsl:call-template name="MenuTrailElement">
						<xsl:with-param name="currentpath"><xsl:value-of select="$currentpath"/><xsl:value-of select="name()"/>/</xsl:with-param>
						<xsl:with-param name="level" select="$level+1"/>
					</xsl:call-template>
				</xsl:if>
			</xsl:for-each>
	</xsl:template>
	
</xsl:stylesheet>