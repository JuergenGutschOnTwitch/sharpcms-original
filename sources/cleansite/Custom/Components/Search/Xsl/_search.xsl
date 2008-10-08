<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet
		version="1.0"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		xmlns="http://www.w3.org/1999/xhtml">


	<xsl:template mode="show" match="element[@type='search']">
		<xsl:choose>
			<xsl:when test="contains(//data/attributes/pageroot, 'english')">
				<h1>Search sharpcms web site</h1>
			</xsl:when>
			<xsl:otherwise>
				<h1>sharpcms Website drchsuchen</h1>
			</xsl:otherwise>
		</xsl:choose>
		<div>
			<form name="systemform" method="post" encType="multipart/form-data">
				<!-- primary hidden settings -->
				<input type="hidden" name="event_main" value="search"/>
				<input type="hidden" name="event_mainvalue" value=""/>
				<input type="hidden" name="process">
					<xsl:attribute name="value">
						<xsl:value-of select="//data/query/other/process"/>
					</xsl:attribute>
				</input>
				<input type="hidden" name="data_pageidentifier">
					<xsl:attribute name="value">
						<xsl:value-of select="//data/contenttwo/page/attributes/pageidentifier"/>
					</xsl:attribute>
				</input>
				<input type="hidden" name="data_start" value=""/>

				<xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
					<a href="javascript:ThrowEventConfirm('index', '','Are you sure to rebuild index?')">
						<xsl:attribute name="class">button</xsl:attribute>
						<xsl:attribute name="onmouseover">this.style.backgroundColor='AliceBlue';</xsl:attribute>
						<xsl:attribute name="onmouseout">this.style.backgroundColor='White';</xsl:attribute>
						<xsl:text>Rebuild Index</xsl:text>
					</a>
				</xsl:if>
				<br />
				<input type="text" name="data_query" value="{query}" id="s" class="search" />
				<input type="submit" value="Suchen" class="but" />
			</form>
		</div>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='summary']">
		<p>
			<xsl:value-of select="." disable-output-escaping="yes"/>
			<hr />
		</p>
	</xsl:template >

	<xsl:template mode="show" match="element[@type='paging']">
		<p>
			<xsl:choose>
				<xsl:when test="contains(//data/attributes/pageroot, 'english')">
					<xsl:text>Result page: </xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:text>Ergebnisseiten: </xsl:text>
				</xsl:otherwise>
			</xsl:choose>
			<xsl:value-of select="." disable-output-escaping="yes"/>
		</p>
		<p>
			<span>Search Powered by </span>
			<a href="http://incubator.apache.org/lucene.net/">
				<xsl:text>Lucene.Net</xsl:text>
			</a>
		</p>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='result']">
		<p>
			<a class="link">
				<xsl:attribute name="href">
					<xsl:value-of select="path" />
				</xsl:attribute>
				<strong>
					<xsl:value-of select="title" />
				</strong>
			</a>
			<br />
			<span class="sample">
				<xsl:value-of select="sample" disable-output-escaping="yes"/>
			</span>
			<br />
			<a class="link">
				<xsl:attribute name="href">
					<xsl:value-of select="path" />
				</xsl:attribute>
				<xsl:value-of select="/data/basepath"/>
				<xsl:text>/</xsl:text>
				<xsl:value-of select="path"/>
			</a>
			<br />
			<br />
		</p>
	</xsl:template>
</xsl:stylesheet>