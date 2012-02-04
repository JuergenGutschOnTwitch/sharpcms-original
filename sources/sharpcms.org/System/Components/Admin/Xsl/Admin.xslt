<?xml version="1.0" encoding="utf-8" ?>

<xsl:stylesheet
	version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns="http://www.w3.org/1999/xhtml">
  <xsl:output
		  method="html"
		  doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"
		  doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
		  omit-xml-declaration="yes"
		  indent="yes" />

  <xsl:include href="..\..\..\..\Custom\Components\Snippets.xslt" />

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <base href="{/data/basepath}/" />
        <script type="text/javascript">
          <xsl:text>var basePath = '</xsl:text>
          <xsl:value-of select="/data/basepath" />
          <xsl:text>';</xsl:text>
        </script>
        <title>
          <xsl:text>Admin - Sharpcms.net</xsl:text>
          <xsl:if test="/data/currentuser/basedata">
            <xsl:text> [</xsl:text>
            <xsl:value-of select="/data/currentuser/basedata" />
            <xsl:text>]</xsl:text>
          </xsl:if>
        </title>
        <link type="text/css" rel="stylesheet" href="/System/Components/Admin/Styles/base.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Scripts/tree/dtree.css" />
        <link type="text/css" rel="StyleSheet" href="/System/Components/Admin/Scripts/jquery/ui.tabs.min.css" />
        <!--[if IE 7]>
	      <link type="text/css" rel="stylesheet" href="/System/Components/Admin/Styles/ie.css" />
        <![endif]-->
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery-1.7.1.min.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery-ui-1.8.17.custom.min.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/jquery/jquery.url.packed.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/tree/dtree.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/modal.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/sessvars.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/eventhandler.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/collapseexpand.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript" src="/System/Components/Admin/Scripts/tiny_mce/jquery.tinymce.js">
          <xsl:text> </xsl:text>
        </script>
        <script type="text/javascript">
          $(function() {
            $('textarea.mceeditor').tinymce({
              script_url : '/System/Components/Admin/Scripts/tiny_mce/tiny_mce.js',
              
              theme : 'advanced',
              plugins : 'sharpcmschooser,fullscreen,spellchecker,safari,spellchecker,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,inlinepopups,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,imagemanager,filemanager',

              theme_advanced_buttons1 : 'fullscreen,formatselect,cleanup,forecolor,bold,italic,underline,strikethrough,justifyleft,justifycenter,justifyright,image,indent,outdent,bullist,numlist,undo,redo,link,unlink',
              theme_advanced_buttons2 : 'tablecontrols,|,sharpcmslinkchooser,sharpcmsfilechooser,sharpcmsimagechooser,|,code',
              theme_advanced_buttons3 : '',
              theme_advanced_buttons4 : '',
              theme_advanced_toolbar_location : 'top',
              theme_advanced_toolbar_align : 'left',
              theme_advanced_statusbar_location : 'bottom',
              theme_advanced_resizing : false,
              
              content_css : '/System/Components/Admin/Styles/tinystyle.css',
            });

            $('#tabs').tabs();
            $('#pada_body_tabs').tabs();
          });
        </script>
      </head>
      <body>
        <a class="anchor" name="top">
          <xsl:text> </xsl:text>
        </a>
        <form action="Default.aspx" name="systemform" method="post" encType="multipart/form-data">
          <div class="page">
            <!-- Begin of primary hidden settings -->
            <input type="hidden" name="event_main" value="" />
            <input type="hidden" name="event_mainvalue" value="" />
            <input type="hidden" name="process">
              <xsl:attribute name="value">
                <xsl:value-of select="data/query/other/process" />
              </xsl:attribute>
            </input>
            <!-- End of primary hidden settings -->

            <!-- Begin Header -->
            <div class="header">
              <a href="{/data/basepath}">
                <img src="System/Components/Admin/Images/logo.gif" />
              </a>
            </div>
            <!-- End Header -->

            <!-- Begin TopMenu -->
            <div class="mainnavi">
              <ul class="left">
                <xsl:apply-templates mode="topmenu" select="/data/basedata/adminmenu/left/item">
                  <xsl:sort select="@weight" order="ascending" />
                </xsl:apply-templates>
              </ul>
              <ul class="right">
                <li>
                  <a href="{/data/basepath}" target="_blank">
                    <xsl:text>Show website</xsl:text>
                  </a>
                </li>
                <xsl:apply-templates mode="topmenu" select="/data/basedata/adminmenu/right/item">
                  <xsl:sort select="@weight" order="ascending" />
                </xsl:apply-templates>
              </ul>
            </div>
            <!-- End TopMenu -->

            <!-- Begin Content -->
            <div class="content">
              <div class="left">
                <xsl:apply-templates select="data/navigationplace/*" mode="edit" />
              </div>
              <div class="right">
                <xsl:if test="/data/messages">
                  <div class="menu admin_menu">
                    <ul>
                      <xsl:for-each select="/data/messages/item">
                        <li>
                          <xsl:value-of select="." />
                        </li>
                      </xsl:for-each>
                    </ul>
                  </div>
                </xsl:if>
                <xsl:apply-templates select="data/contentplace/*" mode="edit" />
              </div>
            </div>
            <!-- End Content -->

          </div>
        </form>
      </body>
    </html>
  </xsl:template>

  <xsl:template mode="topmenu" match="item">
    <li>
      <a>
        <xsl:attribute name="href">
          <xsl:if test="@path">
            <xsl:value-of select="@path" />
            <xsl:text>.aspx</xsl:text>
          </xsl:if>
          <xsl:if test="@javascript">
            <xsl:text>javascript:</xsl:text>
            <xsl:value-of select="@javascript" />
          </xsl:if>
          <xsl:if test="@event">
            <xsl:text>javascript:ThrowEvent('</xsl:text>
            <xsl:value-of select="@event" />
            <xsl:text>', '');</xsl:text>
          </xsl:if>
        </xsl:attribute>
        <xsl:value-of select="." />
      </a>
    </li>
  </xsl:template>
</xsl:stylesheet>