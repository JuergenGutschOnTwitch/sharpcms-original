<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" />

	<xsl:template mode="edit" match="file">
    <div class="head filedata_head">
      <div class="title">
        <xsl:value-of select="@path" />
      </div>
      <div class="viewstate">
        <xsl:if test="@mainmimetype='image'">
          <p id="fida_vs">˅</p>
        </xsl:if>
      </div>
    </div>
    <div class="menu filedata_menu">
      <a class="button">
        <xsl:attribute name="href">
          javascript:ThrowEventConfirm('removefile','<xsl:value-of select="@path" />','Do you want to delete the file?');
        </xsl:attribute>
        Delete file
      </a>
      <a class="button">
        <xsl:attribute name="href">
          javascript:ThrowEventNew('renamefile','<xsl:value-of select="@path" />','Write the new name');
        </xsl:attribute>
        Rename file
      </a>
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>default.aspx?process=download/</xsl:text>
          <xsl:value-of select="@path" />&amp;download=true
        </xsl:attribute>
        Download file
      </a>
      <a class="button">
        <xsl:attribute name="href">
          javascript:ModalDialogShow('<xsl:value-of select="/data/basepath" />/default.aspx?process=admin/choose/folder','ReturnMethodMoveFile()');
        </xsl:attribute>
        Move file
      </a>
      <script language="JavaScript">
        function ReturnMethodMoveFile() {
        tmpValue = ModalDialog.value;
        ModalDialogRemoveWatch();
        ThrowEvent('movefile','<xsl:value-of select="@path" />*' +tmpValue);
        }
      </script>
    </div>
    <xsl:if test="@mainmimetype='image'">
      <div class="body filedata_body" id="fida_body">
        <img>
          <xsl:attribute name="src">
            ?process=download/<xsl:value-of select="@path" />
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
              Width: <input type="text" name="width" maxlength="5" />px
            </td>
            <td class="field">
              Height: <input type="text" name="height" maxlength="5" />px
            </td>
            <td>
              <a class="button" href="javascript:ThrowEvent('doresize','');">
                Resize
              </a>
            </td>
          </tr>
          <tr>
            <td colspan="3">
              <strong>NB:</strong> The resized image will be placed in the thumbs folder.
            </td>
          </tr>
        </table>
      </div>
    </xsl:if>
	</xsl:template>
</xsl:stylesheet>