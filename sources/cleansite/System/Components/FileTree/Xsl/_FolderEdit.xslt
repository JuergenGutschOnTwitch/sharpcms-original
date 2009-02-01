<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" />
  
	<xsl:template mode="edit" match="folder">
    <div class="folderdata_header">
      <div class="title">
        <xsl:value-of select="@path" />
      </div>
      <div class="viewstate">
        <a id="foda_vs" href="javascript:collapseexpand('foda_body', 'foda_vs');">˅</a>
      </div>
    </div>
    <div class="menu folderdata_menu">
      <a class="button" href="javascript:ThrowEventConfirm('removefolder','{@path}','Do you want to delete the folder?')">
        Delete folder
      </a>
      <a class="button">
        <xsl:attribute name="href">
          javascript:ThrowEventNew('renamefolder','<xsl:value-of select="@path" />','Write the new name')
        </xsl:attribute>
        Rename folder
      </a>
      <a class="button">
        <xsl:attribute name="href">
          javascript:ThrowEventNew('addfolder','<xsl:value-of select="@path" />','Type the name of the new folder')
        </xsl:attribute>
        Add subfolder
      </a>
      <a class="button">
        <xsl:attribute name="href">
          javascript:ModalDialogShow('<xsl:value-of select="/data/basepath" />/default.aspx?process=admin/choose/folder','ReturnMethodMoveFolder()');
        </xsl:attribute>
        Move folder
      </a>
      <script type="text/javascript">
        function ReturnMethodMoveFolder() {
        tmpValue = ModalDialog.value;
        ModalDialogRemoveWatch();
        ThrowEvent('movefolder','<xsl:value-of select="@path" />*' +tmpValue);
        }
      </script>
    </div>
      <div class="folderdata_body" id="foda_body">
      <xsl:call-template name="upload" />
    </div>
	</xsl:template>
	
	<xsl:template name="upload">
		<div class="adminmenu">
			Upload new file in directory
      <br />
      <xsl:call-template name="uploadfield">
        <xsl:with-param name="currentlevel">1</xsl:with-param>
        <xsl:with-param name="maxlevel">10</xsl:with-param>
      </xsl:call-template>
			<a class="button" href="javascript:ThrowEvent('uploadfile', '{@path}', 'Type the name of the new folder:')">
				Upload
			</a>
		</div> 
	</xsl:template>

  <xsl:template name="uploadfield">
    <xsl:param name="currentlevel" />
    <xsl:param name="maxlevel" />
    <div id="file_{$currentlevel}">
      <xsl:if test="$currentlevel > 1">
        <xsl:attribute name="style">
          <xsl:text>display: none;</xsl:text>
        </xsl:attribute>
      </xsl:if>
      <input type="file" size="60" class="upload" name="datafile_{$currentlevel}" />
      <br />
      <br />
      <xsl:if test="$currentlevel &lt; $maxlevel">
        <div id="showlink_{$currentlevel + 1}">
          <a href="javascript:showAndHide('file_{$currentlevel + 1}', 'showlink_{$currentlevel + 1}');">More files...</a>
          <br />
          <br />
        </div>
        <xsl:call-template name="uploadfield">
          <xsl:with-param name="currentlevel" select="$currentlevel + 1" />
          <xsl:with-param name="maxlevel" select="$maxlevel" />
        </xsl:call-template>
      </xsl:if>
    </div>
  </xsl:template>
</xsl:stylesheet>
