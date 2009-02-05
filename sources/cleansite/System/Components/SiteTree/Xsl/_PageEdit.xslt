<?xml version="1.0" encoding="UTF-8" ?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:sharpcms="urn:my-scripts">
	<xsl:template mode="edit" match="page">
    <script type="text/javascript">
      function copyPage() {
        if (ModalDialog.value != undefined &amp;&amp; ModalDialog.value != "") {
          ThrowEvent('pagecopyto', '<xsl:value-of select="attributes/pageidentifier" />¤' + ModalDialog.value + '¤' + prompt('Page name:', '<xsl:value-of select="sharpcms:Escape(/data/contenttwo/page/attributes/pagename)" />'));
        }
        ModelDialogRemoveWatch();
      }
      
      function movePage() {
        if (ModalDialog.value != undefined &amp;&amp; ModalDialog.value != "") {
          ThrowEvent('pagemove', ModalDialog.value);
        }
        ModelDialogRemoveWatch();
      }

      <xsl:if test="/data/query/events/mainvalue='openwindow'">
        <xsl:text>open_window ('show/</xsl:text>
        <xsl:value-of select="attributes/pageidentifier" />
        <xsl:text>.aspx', 'showwebsite');</xsl:text>
      </xsl:if>
    </script>
		<input type="hidden" name="data_pageidentifier">
			<xsl:attribute name="value">
				<xsl:value-of select="attributes/pageidentifier" />
			</xsl:attribute>
		</input>
		<input type="hidden" name="page">
			<xsl:attribute name="value">
				<xsl:value-of select="attributes/pageidentifier" />
			</xsl:attribute>
		</input>
    <div class="head pagedata_head">
      <div class="title">
        Page data
      </div>
      <div class="viewstate">
        <p id="pada_vs">˅</p>
      </div>
    </div>
    <div class="menu pagedata_menu top">
      <a class="button" href="javascript:ThrowEvent('save','');">Save</a>
      <a class="button" href="javascript:ThrowEvent('save','openwindow');">Save and show</a>
      <select name="adminmoreactions" id="adminmoreactions" onChange="eval(adminmoreactions.options[adminmoreactions.selectedIndex].value);">
        <option value="">More actions...</option>
        <option value="ThrowEventConfirm('removepage', '{attributes/pageidentifier}', 'Do you want to delete the page?');">
          &#160;&#160;Delete page
        </option>
        <option value="ThrowEventNew('addpage','{attributes/pageidentifier}','Type the name of the new page:');">
          &#160;&#160;Add subpage
        </option>
        <option value="ThrowEventNew('pagecreatcontainer','','Type the name of the new container:');">
          &#160;&#160;Add container
        </option>
        <option disabled="disabled" />
        <option disabled="disabled">Move</option>
        <option value="ThrowEvent('pagemoveup','{attributes/pageidentifier}');">
          &#160;&#160;Move up
        </option>
        <option value="ThrowEvent('pagemovedown','{attributes/pageidentifier}');">
          &#160;&#160;Move down
        </option>
        <option value="ThrowEvent('pagemovetop','{attributes/pageidentifier}');">
          &#160;&#160;Move Top
        </option>
        <option value="ThrowEvent('pagemovebottom','{attributes/pageidentifier}');">
          &#160;&#160;Move Bottom
        </option>
        <option value="ModalDialogShow('{/data/basepath}/admin/choose/page.aspx', 'movePage()');">&#160;&#160;Move To</option>
        <option value="ModalDialogShow('{/data/basepath}/admin/choose/page.aspx', 'copyPage()');">&#160;&#160;Copy To</option>
        <option disabled="disabled" />
        <option value="ThrowEvent('setstandardpage', '');">Set as default page</option>
      </select>
    </div>
    <div class="body pagedata_body" id="pada_body">
      <xsl:call-template name="topelements" />
		</div>
    <div id="tabpagecontent" class="tab-pane">
      <script type="text/javascript">
        tp1 = new WebFXTabPane(document.getElementById("tabpagecontent"), true, <xsl:value-of select="count(containers/container)" />);
      </script>
			<xsl:apply-templates mode="form" select="containers/container" />
		</div>
		<script type="text/javascript">
			// setupAllTabs();
		</script>
    <div class="menu pagedata_menu bottom">
			<a class="button" href="javascript:ThrowEvent('save','');">Save</a>
      <a class="button" href="javascript:ThrowEvent('save','openwindow');">
        Save and show
      </a>
    </div>
	</xsl:template>

	<xsl:template mode="form" match="container">
		<div class="tab-page">
			<xsl:attribute name="id">
				<xsl:text>tab</xsl:text><xsl:value-of select="@name" />
			</xsl:attribute>
			<h2 class="tab">
				<xsl:value-of select="@name" />
			</h2>
      <script type="text/javascript">
        <xsl:text>tp1.addTabPage(document.getElementById("tab</xsl:text>
        <xsl:value-of select="@name" />
        <xsl:text>"));</xsl:text>
      </script>

      <div class="menu element_menu">
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEventConfirm('pageremovecontainer','</xsl:text>
            <xsl:number count="container" />
            <xsl:text>','This will remove the container.\n\nAre you sure?');</xsl:text>
          </xsl:attribute>
          X
        </a>
        <select>
          <xsl:attribute name="onChange">
            <xsl:text>ThrowEvent('addelement','text_</xsl:text>
            <xsl:number count="container" />
            <xsl:text>');</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="name">
            <xsl:text>data_container_</xsl:text>
            <xsl:number count="container" />
          </xsl:attribute>
          <option>Add element...</option>
          <!--//data/basedata/elementlist/top/*[not(name()='top') and (@name=@filter or not(@filter)) ]-->
          <xsl:for-each select="//data/basedata/elementlist/*[not(name()='top') and (@name=@filter or not(@filter)) ]">
            <option>
              <xsl:attribute name="value">
								<xsl:value-of select="name()" />
							</xsl:attribute>
              &#160;&#160;<xsl:value-of select="name()" />
						</option>
					</xsl:for-each>
				</select>
			</div>

      <xsl:apply-templates mode="form" select="elements/element" />
		</div>
	</xsl:template>
  
	<xsl:template mode="form" match="*">
		<xsl:call-template name="elementtop" />
		<div class="element_body">
			<xsl:variable name="currentelement" select="." />
			<xsl:variable name="type" select="@type" />
			<xsl:apply-templates mode="formelement" select="//data/basedata/elementlist/*[name()=$type]/*">
				<xsl:with-param name="currentelement" select="$currentelement" />
				<xsl:with-param name="id">
					<xsl:text>data_element_</xsl:text>
					<xsl:number count="container" />_<xsl:number count="element" />
				</xsl:with-param>
			</xsl:apply-templates>
		</div>
	</xsl:template>
  
  <xsl:template name="topelements">
      <xsl:variable name="currentelement" select="." />
      <xsl:variable name="type" select="@type" />
      <xsl:apply-templates mode="formelement" select="//data/basedata/elementlist/top/*[name(//*[@currentpage='true']/..)=@filter or not(@filter) ]">
        <xsl:with-param name="currentelement" select="//data/contenttwo/page/attributes" />
        <xsl:with-param name="id">
          <xsl:text>data_attribute</xsl:text>
        </xsl:with-param>
      </xsl:apply-templates>
  </xsl:template>
  
	<xsl:template mode="formelement" match="item[@type='tinymce']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of  select="@name" />
    </label>
    <div class="item">
      <textarea class="mceeditor" name="{$id}_{@attribute}">
        <xsl:if test="@height">
          <xsl:attribute name="style">
            <xsl:text>width:614px; height:</xsl:text>
            <xsl:value-of select="@height" />
            <xsl:text>px;</xsl:text>
          </xsl:attribute>
        </xsl:if>
        <xsl:value-of select="$currentelement/*[name()=$attribute]" />
      </textarea>
		</div>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='textarea']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of  select="@name" />
    </label>
    <div class="item">
			<textarea name="{$id}_{@attribute}">
				<xsl:value-of select="$currentelement/*[name()=$attribute]" />
			</textarea>
		</div>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='text']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
			<input type="text" name="{$id}_{@attribute}" value="{$currentelement/*[name()=$attribute]}" />
		</div>
	</xsl:template>

  <xsl:template mode="formelement" match="item[@type='statuslist']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <xsl:variable name="status" select="$currentelement/*[name()=$attribute]" />
      <select>
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
        </xsl:attribute>
        <xsl:for-each select="//data/basedata/pagestatus/*">
          <option >
            <xsl:attribute name="value">
              <xsl:value-of select="name()" />
            </xsl:attribute>
            <xsl:if test="$status=name()">
              <xsl:attribute name="selected">selected</xsl:attribute>
            </xsl:if>
            <xsl:value-of select="." />
          </option>
        </xsl:for-each>
      </select>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='listtemplate']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <xsl:variable name="status" select="$currentelement/*[name()=$attribute]" />
      <select>
        <xsl:attribute name="name">
          <xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
        </xsl:attribute>
        <option value="">Default</option>
        <xsl:for-each select="/data/templates/*">
          <option >
            <xsl:attribute name="value">
              <xsl:value-of select="name()" />
            </xsl:attribute>
            <xsl:if test="$status=name()">
              <xsl:attribute name="selected">selected</xsl:attribute>
            </xsl:if>
            <xsl:value-of select="name()" />
          </option>
        </xsl:for-each>
      </select>
    </div>
  </xsl:template>

	<xsl:template mode="formelement" match="item[@type='hidden']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
		<input type="hidden">
			<xsl:attribute name="name">
				<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
			</xsl:attribute>
			<xsl:attribute name="value">
				<xsl:value-of select="@value" />
			</xsl:attribute>
		</input>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='date']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
			<input class="date_day" type="text" size="2">
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="concat(@attribute, 'day')" />
				</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="$currentelement/*[name() = concat($attribute, 'day')]" />
				</xsl:attribute>
			</input>
			<xsl:text>. </xsl:text>
      <input class="date_month" type="text" size="2">
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="concat(@attribute, 'month')" />
				</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="$currentelement/*[name() = concat($attribute, 'month')]" />
				</xsl:attribute>
			</input>
			<xsl:text> </xsl:text>
      <input class="date_year" type="text" size="4">
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="concat(@attribute, 'year')" />
				</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="$currentelement/*[name() = concat($attribute, 'year')]" />
				</xsl:attribute>
			</input>
		</div>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='choosepage']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <script type="text/javascript">
			function ReturnMethod<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />() {
			  document.systemform.<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />.value = ModalDialog.value;
			  ModalDialogRemoveWatch();
			}
		</script>
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
			<input type="text">
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
				</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="$currentelement/*[name()=$attribute]" />
				</xsl:attribute>
			</input>
			<a class="button">
				<xsl:attribute name="href">
          <xsl:text>javascript:ModalDialogShow('</xsl:text><xsl:value-of select="/data/basepath" /><xsl:text>/default.aspx?process=admin/choose/page','ReturnMethod</xsl:text>
					<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
					<xsl:text>()');</xsl:text>
				</xsl:attribute>
				Choose
			</a>
		</div>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='choosefile']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
		<script type="text/javascript">
			function ReturnMethod<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />() {
			  document.systemform.<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />.value = ModalDialog.value;
			  ModalDialogRemoveWatch();
      }
		</script>
    <label>
      <xsl:value-of select="@name" />
    </label>
		<div class="item">
			<input type="text">
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
				</xsl:attribute>
				<xsl:attribute name="value">
					<xsl:value-of select="$currentelement/*[name()=$attribute]" />
				</xsl:attribute>
			</input>
			<a class="button">
				<xsl:attribute name="href">
					<xsl:text>javascript:ModalDialogShow('</xsl:text><xsl:value-of select="/data/basepath" /><xsl:text>/default.aspx?process=admin/choose/file','ReturnMethod</xsl:text>
          <xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
          <xsl:text>()');</xsl:text>
        </xsl:attribute>
        Choose
      </a>
    </div>
  </xsl:template>

  <xsl:template mode="formelement" match="item[@type='choosefolder']">
    <xsl:param name="currentelement" />
    <xsl:param name="id" />
    <xsl:variable select="@attribute" name="attribute" />
    <script type="text/javascript">
      function ReturnMethod<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />() {
        document.systemform.<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />.value = ModalDialog.value;
        ModalDialogRemoveWatch();
      }
    </script>
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
      <input type="text" name="{$id}_{@attribute}" value="{$currentelement/*[name()=$attribute]}" />
      <a class="button" href="javascript:ModalDialogShow('{/data/basepath}/default.aspx?process=admin/choose/folder','ReturnMethod{$id}_{@attribute}()');">
				Choose
			</a>
		</div>
	</xsl:template>

	<xsl:template mode="formelement" match="item[@type='list']">
		<xsl:param name="currentelement" />
		<xsl:param name="id" />
		<xsl:variable select="@attribute" name="attribute" />
    <label>
      <xsl:value-of select="@name" />
    </label>
    <div class="item">
			<select>
				<xsl:attribute name="name">
					<xsl:value-of select="$id" />_<xsl:value-of select="@attribute" />
				</xsl:attribute>
				<xsl:for-each select="*">
					<option>
						<xsl:if test="($currentelement/*[name()=$attribute]) = .">
							<xsl:attribute name="selected">selected</xsl:attribute>
						</xsl:if>
						<xsl:attribute name="value">
							<xsl:value-of select="." />
						</xsl:attribute>
						<xsl:value-of select="." />
					</option>
				</xsl:for-each>
			</select>
		</div>
	</xsl:template>

  <xsl:template name="elementtop">
		<div class="menu element_head">
      <div class="title">
        <xsl:value-of select="@type" />
      </div>
      <div class="buttons">
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEventConfirm('remove','element-</xsl:text>
            <xsl:number count="container" />-<xsl:number count="element" />
            <xsl:text>','Are you sure you want to delete this element?');</xsl:text>
          </xsl:attribute>
          X
        </a>
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEvent('copy','element-</xsl:text>
            <xsl:number count="container" />-<xsl:number count="element" />
            <xsl:text>');</xsl:text>
          </xsl:attribute>
          Copy
        </a>
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEvent('moveup','element-</xsl:text>
            <xsl:number count="container" />-<xsl:number count="element" />
            <xsl:text>');</xsl:text>
          </xsl:attribute>
          Up
        </a>
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEvent('movedown','element-</xsl:text>
            <xsl:number count="container" />-<xsl:number count="element" />
            <xsl:text>');</xsl:text>
          </xsl:attribute>
          Down
        </a>
        <a class="button">
          <xsl:attribute name="href">
            <xsl:text>javascript:ThrowEvent('movetop','element-</xsl:text><xsl:number count="container" />-<xsl:number count="element" />
            <xsl:text>');</xsl:text>
          </xsl:attribute>
          Top
        </a>
      </div>
    </div>
	</xsl:template>
</xsl:stylesheet>