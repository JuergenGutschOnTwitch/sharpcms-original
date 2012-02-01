<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" />

  <xsl:template mode="edit" match="folder">
    <div class="head folderdata_head">
      <div class="title">
        <xsl:value-of select="@path" />
      </div>
      <div class="viewstate">
        <p id="foda_vs" class="button">
          <xsl:text>˅</xsl:text>
        </p>
      </div>
    </div>
    <div class="menu folderdata_menu">
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventConfirm('removefolder','</xsl:text>
          <xsl:value-of select="@path" />
          <xsl:text>','Do you want to delete the folder?');</xsl:text>
        </xsl:attribute>
        <xsl:text>Delete folder</xsl:text>
      </a>
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventNew('renamefolder','</xsl:text>
          <xsl:value-of select="@path" />
          <xsl:text>','Write the new name');</xsl:text>
        </xsl:attribute>
        <xsl:text>Rename folder</xsl:text>
      </a>
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ThrowEventNew('addfolder','</xsl:text>
          <xsl:value-of select="@path" />
          <xsl:text>','Type the name of the new folder');</xsl:text>
        </xsl:attribute>
        <xsl:text>Add subfolder</xsl:text>
      </a>
      <a class="button">
        <xsl:attribute name="href">
          <xsl:text>javascript:ModalDialogShow('</xsl:text>
          <xsl:value-of select="/data/basepath" />
          <xsl:text>/default.aspx?process=admin/choose/folder','ReturnMethodMoveFolder()');</xsl:text>
        </xsl:attribute>
        <xsl:text>Move folder</xsl:text>
      </a>
      <script type="text/javascript">
        function ReturnMethodMoveFolder() {
        tmpValue = ModalDialog.value;
        ModalDialogRemoveWatch();
        ThrowEvent('movefolder','<xsl:value-of select="@path" />*' +tmpValue);
        }
      </script>
    </div>
    <div class="body folderdata_body" id="foda_body">
      <xsl:call-template name="upload" />
    </div>
  </xsl:template>

  <xsl:template name="upload">
    <div class="adminmenu">
      <xsl:text>Upload new file in directory</xsl:text>
      <br />
      <xsl:call-template name="uploadfield">
        <xsl:with-param name="currentlevel">
          <xsl:text>1</xsl:text>
        </xsl:with-param>
        <xsl:with-param name="maxlevel">
          <xsl:text>10</xsl:text>
        </xsl:with-param>
      </xsl:call-template>
      <a class="button" href="javascript:ThrowEvent('uploadfile', '{@path}', 'Type the name of the new folder:');">
        <xsl:text>Upload</xsl:text>
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
          <a href="javascript:showAndHide('file_{$currentlevel + 1}', 'showlink_{$currentlevel + 1}');">
            <xsl:text>More files...</xsl:text>
          </a>
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