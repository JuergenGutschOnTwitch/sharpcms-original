<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />

  <xsl:template mode="edit" match="file">
    <div class="head filedata_head">
      <div class="title">
        <b>
          <xsl:value-of select="@path" />
        </b>
      </div>
      <div class="viewstate">
        <xsl:if test="@mainmimetype='image'">
          <a id="fida_vs" class="button expand">
            <xsl:text>˅</xsl:text>
          </a>
        </xsl:if>
      </div>
    </div>
    <div class="menu filedata_menu">
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRemoveFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Delete file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlRenameFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Rename file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="href">
          <xsl:text>/?process=download/</xsl:text>
          <xsl:value-of select="@path" />&amp;download=true
        </xsl:attribute>
        <xsl:text>Download file</xsl:text>
      </a>
      <a>
        <xsl:attribute name="class">
          <xsl:text>button hlMoveFile</xsl:text>
        </xsl:attribute>
        <xsl:attribute name="value">
          <xsl:value-of select="/data/basepath" />
          <xsl:text>;;</xsl:text>
          <xsl:value-of select="@path" />
        </xsl:attribute>
        <xsl:text>Move file</xsl:text>
      </a>
    </div>
    <xsl:if test="@mainmimetype='image'">
      <div class="body filedata_body" id="fida_body">
        <img>
          <xsl:attribute name="src">
            <xsl:text>?process=download/</xsl:text>
            <xsl:value-of select="@path" />
            <xsl:choose>
              <xsl:when test="/data/query/other/width">
                <xsl:text>&amp;width=</xsl:text>
                <xsl:value-of select="/data/query/other/width" />
              </xsl:when>
              <xsl:when test="/data/query/other/height">
                <xsl:text>&amp;height=</xsl:text>
                <xsl:value-of select="/data/query/other/height" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:text>&amp;width=400&amp;height=400</xsl:text>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </img>
        <table>
          <tr>
            <td class="field">
              <xsl:text>Width: </xsl:text>
              <input type="text" name="width" maxlength="5" />
              <xsl:text>px</xsl:text>
            </td>
            <td class="field">
              <xsl:text>Height: </xsl:text>
              <input type="text" name="height" maxlength="5" />
              <xsl:text>px</xsl:text>
            </td>
            <td>
              <a>
                <xsl:attribute name="class">
                  <xsl:text>button hlResizeFile</xsl:text>
                </xsl:attribute>
                <xsl:text>Resize</xsl:text>
              </a>
            </td>
          </tr>
          <tr>
            <td colspan="3">
              <strong>
                <xsl:text>NB:</xsl:text>
              </strong>
              <xsl:text>The resized image will be placed in the thumbs folder.</xsl:text>
            </td>
          </tr>
        </table>
      </div>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>