<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output
    method="html" 
     doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"  
     doctype-public="-//W3C//DTD XHTML 1.0 Transitional//EN"
     omit-xml-declaration="yes" 
     indent="yes"
	/>
  <xsl:include href="-PageContent.xslt"/>
	<xsl:include href="-SiteTree.xslt"/>
  <xsl:include href="..\..\Snippets.xslt"/>

  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
			<head>
        <base >
          <xsl:attribute name="href">
            <xsl:value-of select="/data/basepath"/>
            <xsl:text>/</xsl:text>
          </xsl:attribute>

        </base>
        <title>
          Sharpcms.net (<xsl:value-of select="/data/contenttwo/page/attributes/pagename"/>)
        </title>
        <meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />


        <link href="Custom/Components/FrontEnd/style.css" rel="stylesheet" type="text/css" />
				<link rel="stylesheet" type="text/css" href="Custom/Components/FrontEnd/niftycorners/niftyCorners.css"/>
				<script type="text/javascript" src="Custom/Components/FrontEnd/niftycorners/nifty.js">lsdkjf</script>
				<script type="text/javascript">
					<![CDATA[
				window.onload=function(){
				if(!NiftyCheck())
					return;
			
					Rounded("div#topdiv","top","#393439","#ffffff","smooth");
					Rounded("div#topdiv","bottom","#393439","#ffffff","smooth");

				} 
				]]>
				</script>
			</head>
			<body style="background:#393439;text-align:center;">
 
				<div id="topdiv">
					<div id="logo" style="width:779px;">
						<img src="Custom/Components/FrontEnd/Pictures/logo.gif"/>
					</div>
					<div id="topmenu">
						<xsl:call-template name="TopMenu"/>
					</div>
					<div id="toppicture">
						<img src="Custom/Components/FrontEnd/Pictures/top.jpg"/>
					</div> 
					<div  id="submenu"> 
						<xsl:call-template name="SubMenu"/>
					</div>
					<div id="contentcontainer">
            <xsl:if test="/data/messages">
              <div class="adminmenu" style="background-color:#ffffe1">
                <ul>
                  <xsl:for-each select="/data/messages/item">
                    <li>
                      <xsl:value-of select="."/>
                    </li>
                  </xsl:for-each>
                </ul>
              </div>
            </xsl:if>
            
						<xsl:apply-templates mode="show" select="/data/contenttwo/page/containers/container[@name='content']"/>
						<xsl:text> </xsl:text>
					</div>
					<div style="clear:both;">
						
					</div>
				</div>
        


			</body>
		</html>

	</xsl:template>
</xsl:stylesheet>