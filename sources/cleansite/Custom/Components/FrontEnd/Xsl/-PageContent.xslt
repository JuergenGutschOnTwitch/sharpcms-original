<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:inventit="urn:my-scripts">
	<!--<ms:script implements-prefix="inventit" xmlns:ms="urn:schemas-microsoft-com:xslt" language="C#" >
		<![CDATA[
		public string textToHtml(string text) {
			return text.Replace("\n", "<br/>");
		}
		]]>
	</ms:script>-->

	<xsl:template mode="show" match="container[@name='content']">
		<xsl:apply-templates mode="show" select="elements/element"/>
	</xsl:template>

	<xsl:template mode="show" match="container[@name='news']">
		<xsl:apply-templates mode="show" select="elements/element"/>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='gallery']">
		<xsl:variable select="folder/@path" name="path"/>
		<xsl:if test="//data/query/data/picture">
			<img>
				<xsl:attribute name="src">
					<xsl:text>default.aspx?process=download/</xsl:text>
					<xsl:value-of select="$path"/>
					<xsl:text>/</xsl:text>
					<xsl:value-of select="//data/query/data/picture"/>
					<xsl:text>&amp;width=250</xsl:text>
				</xsl:attribute>
			</img>
			<br/>
			<br/>
		</xsl:if>
		<div>
			<xsl:for-each select="folder/folder/file[@extension='.jpg']">
				<a>
					<xsl:attribute  name="href">
						<xsl:text>default.aspx?process=</xsl:text>
						<xsl:value-of select="//data/query/other/process"/>
						<xsl:text>&amp;data_picture=</xsl:text>
						<xsl:value-of select="@name"/>
					</xsl:attribute>
					<img style="border:1px solid black;">
						<xsl:attribute name="src">
							<xsl:text>default.aspx?process=download/</xsl:text>
							<xsl:value-of select="$path"/>
							<xsl:text>/</xsl:text>
							<xsl:value-of select="@name"/>
							<xsl:text>&amp;width=50&amp;height=50</xsl:text>
						</xsl:attribute>
					</img>
				</a>
			</xsl:for-each>
		</div>


	</xsl:template>

	<xsl:template mode="show" match="element[@type='paragraph']">

		<div class="content">
			<xsl:if test="header and not(header='')">
				<xsl:choose>
					<xsl:when test="headerstyle='Header1'">
						<h1>
							<xsl:value-of select="header"/>
						</h1>
					</xsl:when>
					<xsl:when test="headerstyle='Header2'">
						<h2>
							<xsl:value-of select="header"/>
						</h2>
					</xsl:when>
				</xsl:choose>
			</xsl:if>


			<xsl:if test="picture and not(picture='')">
				<img>
					<xsl:attribute name="src">
						<xsl:text>default.aspx?process=download/</xsl:text>
						<xsl:value-of select="picture"/>

					</xsl:attribute>
					<xsl:attribute name="style">
						<xsl:choose>
							<xsl:when test="picturepos='Left'">
								<xsl:text>float:left;margin-top:0;margin-bottom:10px;margin-right:10px;</xsl:text>
							</xsl:when>
							<xsl:when test="picturepos='Right'">
								<xsl:text>float:right;margin-top:0;margin-bottom:10px;margin-left:10px;</xsl:text>
							</xsl:when>
						</xsl:choose>
					</xsl:attribute>
				</img>
			</xsl:if>
			<xsl:value-of select="text" disable-output-escaping="yes"/>
			<xsl:if test="not(link = '') and link">
				<a >
					<xsl:choose>
						<xsl:when test="substring(link, 1, 7) = 'http://'">
							<xsl:attribute name="href">
								<xsl:value-of select="link"/>
							</xsl:attribute>
							<xsl:attribute name="target">
								<xsl:text>_blank</xsl:text>
							</xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
							<xsl:attribute name="href">
								<xsl:text>default.aspx?process=show/</xsl:text>
								<xsl:value-of select="link"/>
							</xsl:attribute>
						</xsl:otherwise>
					</xsl:choose>
					<xsl:text>Read more</xsl:text>
				</a>
			</xsl:if>
		</div>
	</xsl:template>

	<xsl:template mode="show" match="element[@type='coworker']">
		<table cellpadding="5">
			<tr>
				<xsl:if test="not(picture = '') and picture">
					<td valign="top">
						<img>
							<xsl:attribute name="src">
								<xsl:text>default.aspx?process=download/</xsl:text>
								<xsl:value-of select="picture"/>
							</xsl:attribute>
							<xsl:if test="not(name = '') and name">
								<xsl:attribute name="title">
									<xsl:value-of select="name"/>
								</xsl:attribute>
							</xsl:if>
						</img>
					</td>
				</xsl:if>
				<td valign="top">
					<xsl:if test="not(name = '') and name">
						<strong>
							<xsl:value-of select="name"/>
						</strong>
						<br />
					</xsl:if>
					<xsl:if test="not(role = '') and role">
						<em>
							<xsl:value-of select="role"/>
						</em>
						<br />
					</xsl:if>
					<xsl:if test="not(email = '') and email">
						<a>
							<xsl:attribute name="href">
								<xsl:text>mailto:</xsl:text>
								<xsl:value-of select="email"/>
							</xsl:attribute>
							<xsl:value-of select="email"/>
						</a>
					</xsl:if>

					<xsl:if test="not(text = '') and text">
						<br/>
						<br/>
						<xsl:value-of select="text"/>
					</xsl:if>
				</td>
			</tr>
		</table>
	</xsl:template>

	<xsl:template mode="show" match="element">
		<div>
			<xsl:attribute name="class">
				<xsl:text>element-</xsl:text>
				<xsl:value-of select="@type"/>
			</xsl:attribute>
			<xsl:for-each select="*">
				<div>
					<xsl:attribute name="class">
						<xsl:text>item-</xsl:text>
						<xsl:value-of select="name()"/>
					</xsl:attribute>
					<xsl:value-of select="."/>
				</div>
			</xsl:for-each>
		</div>
	</xsl:template>
</xsl:stylesheet>