<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">

  <xsl:output method="html" encoding="utf-8" indent="yes" />

  <xsl:template mode="show" match="element[@type='picture']">
    <xsl:if test="picture and not(picture='')">
      <p>
        <img style="border:none 0px;">
          <xsl:attribute name="src">
            <xsl:text>/?process=download/</xsl:text>
            <xsl:value-of select="picture" />
          </xsl:attribute>
          <xsl:attribute name="alt">
            <xsl:value-of select="alttext" />
          </xsl:attribute>
        </img>
        <xsl:if test="not(text = '') and text">
          <br />
          <xsl:value-of select="text" />
        </xsl:if>
      </p>
    </xsl:if>
    <p>
      <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
    </p>
  </xsl:template>

  <xsl:template name="edit">
    <xsl:if test="//data/basedata/currentuser/groups/group[contains(., 'admin')]">
      <div>
        <xsl:attribute name="class">
          <xsl:text>editlink</xsl:text>
        </xsl:attribute>
        <a>
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath" />
            <xsl:text>admin/page/edit/</xsl:text>
            <xsl:value-of select="/data/contentplace/page/attributes/pageidentifier" />
            <xsl:text>/?c=</xsl:text>
            <xsl:number count="container" />
            <xsl:text disable-output-escaping="no">&amp;e=</xsl:text>
            <xsl:number count="element" />
          </xsl:attribute>
          <span>
            <xsl:text>Edit</xsl:text>
          </span>
        </a>
      </div>
    </xsl:if>
  </xsl:template>

  <xsl:template name="LogOut">
    <form name="systemform" id="systemform" method="post" enctype="multipart/form-data">
      <xsl:attribute name="class">
        <xsl:text>right</xsl:text>
      </xsl:attribute>
      <input type="hidden" name="event_main" value="logout" />
      <input type="hidden" name="event_mainvalue" value="" />
      <input type="submit" value="Log out">
        <xsl:attribute name="class">
          <xsl:text>linkbutton</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:text>Log out</xsl:text>
        </xsl:attribute>
      </input>
    </form>
  </xsl:template>

  <xsl:template mode="show" match="element[@type='header']">
    <xsl:if test="text and not(text='')">
      <xsl:choose>
        <xsl:when test="headerstyle='Header1'">
          <h1>
            <xsl:value-of select="text" />
          </h1>
        </xsl:when>
        <xsl:when test="headerstyle='Header2'">
          <h2>
            <xsl:value-of select="text" />
          </h2>
        </xsl:when>
        <xsl:when test="headerstyle='Header3'">
          <h3>
            <xsl:value-of select="text" />
          </h3>
        </xsl:when>
      </xsl:choose>
    </xsl:if>
  </xsl:template>

  <xsl:template mode="show" match="element[@type='textblock']">
    <xsl:if test="not(text = '') and text">
      <xsl:value-of disable-output-escaping="yes" select="text" />
    </xsl:if>
  </xsl:template>

  <xsl:template mode="show" match="element[@type='page']">
    <a>
      <xsl:attribute name="href">
        <xsl:text>show/</xsl:text>
        <xsl:value-of select="text" />
      </xsl:attribute>
      <xsl:value-of select="text" />
    </a>
  </xsl:template>

  <xsl:template mode="show" match="element[@type='file']">
    <a>
      <xsl:attribute name="href">
        <xsl:text>show/</xsl:text>
        <xsl:value-of select="text" />
      </xsl:attribute>
      <xsl:value-of select="text" />
    </a>
  </xsl:template>

  <xsl:template mode="show" match="element[@type='image']">
    <img>
      <xsl:attribute name="src">
        <xsl:text>show/</xsl:text>
        <xsl:value-of select="text" />
      </xsl:attribute>
    </img>
  </xsl:template>
  
  <xsl:template mode="show" match="*">
      <xsl:for-each select="elements/element">
        <xsl:call-template name="edit" />
        <xsl:if test="@publish = '' or @publish = 'true' or //data/basedata/currentuser/groups/group[contains(., 'admin')]">
          <xsl:apply-templates mode="show" select="." />
        </xsl:if>
      </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>