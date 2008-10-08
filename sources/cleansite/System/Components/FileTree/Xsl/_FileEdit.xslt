<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>
	<xsl:template mode="edit" match="file">
			<div class="adminmenu">
				<div class="adminmenu">
					<a class="button">
						<xsl:attribute name="href">javascript:ThrowEventConfirm('removefile','<xsl:value-of select="@path"/>','Do you want to delete the file?')</xsl:attribute>			
						Delete file
					</a> 
					<a class="button">
						<xsl:attribute name="href">javascript:ThrowEventNew('renamefile','<xsl:value-of select="@path"/>','Write the new name')</xsl:attribute>			
						Rename file
					</a> 
					<a class="button">
						<xsl:attribute name="href">
              <xsl:text>default.aspx?process=download/</xsl:text>
              <xsl:value-of select="@path"/>&amp;download=true</xsl:attribute>			
						Download file
					</a>
					<a class="button">
						<xsl:attribute name="href">javascript:ModalDialogShow('<xsl:value-of select="/data/basepath"/>/default.aspx?process=admin/choose/folder','ReturnMethodMoveFile()');
          </xsl:attribute>
						Move file
					</a>
			
					<script language="JavaScript">		
						function ReturnMethodMoveFile()
						{
							tmpValue = ModalDialog.value;
							ModalDialogRemoveWatch();
							ThrowEvent('movefile','<xsl:value-of select="@path"/>*' +tmpValue);
						}
					</script>
				</div>
				<b><xsl:value-of select="@path"/></b><br/>

        <xsl:if test="@mainmimetype='image'">
          <div class="adminmenu">
            <img>
              <xsl:attribute name="src">
                ?process=download/<xsl:value-of select="@path"/>
                <xsl:choose>
                  <xsl:when test="/data/query/other/width">
                    <xsl:text>&amp;width=</xsl:text>
                    <xsl:value-of select="/data/query/other/width"/>
                  </xsl:when>
                  <xsl:when test="/data/query/other/height">
                    <xsl:text>&amp;height=</xsl:text>
                    <xsl:value-of select="/data/query/other/height"/>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:text>&amp;width=400&amp;height=400</xsl:text>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </img>
            <br/>
            <br/>
            Width: <input type="text" name="width" size="5" style="width: 32px; margin-right: 15px;"/>
            Height: <input type="text" name="height" size="5" style="width: 32px; margin-right: 15px;"/>
            <a class="button" href="javascript:ThrowEvent('doresize','');">
              Resize
            </a><br />
            <span style="color: #666666;">
              <strong>NB:</strong> The resized image will be placed in the thumbs folder.
            </span>
          </div>
        </xsl:if>
      </div>
	</xsl:template>
</xsl:stylesheet>