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
                <xsl:text>Page data - </xsl:text>
                <xsl:value-of select="attributes/menuname" />
            </div>
            <div class="viewstate">
                <p id="pada_vs" class="button">˅</p>
            </div>
        </div>
        <div class="menu pagedata_menu top">
            <a class="button" href="javascript:ThrowEvent('save','');">
                <xsl:text>Save</xsl:text>
            </a>
            <a class="button" href="javascript:ThrowEvent('save','openwindow');">
                <xsl:text>Save and show</xsl:text>
            </a>
            <a class="button" href="javascript:ThrowEvent('','openwindow');">
                <xsl:text>Show</xsl:text>
            </a>
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
        <div class="tab-pane" id="pada_body" style="float: left;">
            <div id="pada_body_tabs">
                <ul>
                    <li>
                        <a href="#ptabs1">Page Data</a>
                    </li>
                    <li>
                        <a href="#ptabs2">Editor Security</a>
                    </li>
                    <li>
                        <a href="#ptabs3">Extranet Security</a>
                    </li>
                </ul>
                <div id="ptabs1" class="tab-page">
                    <div class="tab_page_body">
                        <xsl:call-template name="topelements" />
                    </div>
                </div>
                <div id="ptabs2" class="tab-page">
                    <div class="tab_page_body">
                        <xsl:call-template name="security">
                            <xsl:with-param name="view">editors</xsl:with-param>
                        </xsl:call-template>
                    </div>
                </div>
                <div id="ptabs3" class="tab-page">
                    <div class="tab_page_body">
                        <xsl:call-template name="security">
                            <xsl:with-param name="view">visitors</xsl:with-param>
                        </xsl:call-template>
                    </div>
                </div>
            </div>
        </div>
        <div id="tabpagecontent" class="tab-pane" style="float: left;">
            <div id="tabs">
                <ul>
                    <xsl:apply-templates mode="tab" select="containers/container" />
                </ul>
                <xsl:apply-templates mode="panel" select="containers/container" />
            </div>
        </div>
        <div class="menu pagedata_menu bottom">
            <a class="button" href="javascript:ThrowEvent('save','');">
                <xsl:text>Save</xsl:text>
            </a>
            <a class="button" href="javascript:ThrowEvent('save','openwindow');">
                <xsl:text>Save and show</xsl:text>
            </a>
            <a class="button" href="javascript:ThrowEvent('','openwindow');">
                <xsl:text>Show</xsl:text>
            </a>
        </div>
    </xsl:template>

    <xsl:template mode="tab" match="container">
        <li>
            <a>
                <xsl:attribute name="href">
                    <xsl:text>#tabs-</xsl:text>
                    <xsl:number level="any" />
                </xsl:attribute>
                <xsl:value-of select="@name" />
            </a>
        </li>
    </xsl:template>

    <xsl:template mode="panel" match="container">
        <div class="tab-page">
            <xsl:attribute name="id">
                <xsl:text>tabs-</xsl:text>
                <xsl:number level="any" />
            </xsl:attribute>

            <div class="menu container_menu">
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
            <xsl:attribute name="class">
                <xsl:text>element_body</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="id">
                <xsl:text>element_</xsl:text>
                <xsl:number count="container" />
                <xsl:text>_</xsl:text>
                <xsl:number count="element" />
                <xsl:text>_body</xsl:text>
            </xsl:attribute>
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


    <xsl:template name="security">
        <xsl:param name="view" />
        <label>
            <xsl:text>Allow Groups or Users to </xsl:text>
            <xsl:choose>
                <xsl:when test="$view='editors'">
                    <xsl:text>Edit</xsl:text>
                </xsl:when>
                <xsl:otherwise>
                    <xsl:text>View</xsl:text>
                </xsl:otherwise>
            </xsl:choose>
            <xsl:text> the current Page</xsl:text>
        </label>
        <div class="checkbox_list">
            <ul>

                <xsl:for-each select="//data/basedata/security/groups/*">
                    <li>
                        <input type="checkbox" value="user1" class="checkbox">
                            <xsl:attribute name="id">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_group_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                            <xsl:attribute name="name">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_group_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                        </input>
                        <label>
                            <xsl:attribute name="for">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_group_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                            <xsl:text>&lt;</xsl:text>
                            <xsl:value-of select="." />
                            <xsl:text>&gt;</xsl:text>
                        </label>
                    </li>
                </xsl:for-each>
                <xsl:for-each select="//data/basedata/security/users/*">
                    <li>
                        <input type="checkbox" value="user1" class="checkbox">
                            <xsl:attribute name="id">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_user_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                            <xsl:attribute name="name">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_user_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                        </input>
                        <label>
                            <xsl:attribute name="for">
                                <xsl:text>checkbox_</xsl:text>
                                <xsl:value-of select="$view"/>
                                <xsl:text>_user_</xsl:text>
                                <xsl:value-of select="."/>
                            </xsl:attribute>
                            <xsl:value-of select="." />
                        </label>
                    </li>
                </xsl:for-each>
            </ul>
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
            <xsl:value-of select="@name" />
        </label>
        <div class="item">
            <textarea name="{$id}_{@attribute}">
                <xsl:if test="@height or $id='data_attribute'">
                    <xsl:attribute name="style">
                        <xsl:if test="@height and not(@height='')">
                            <xsl:text>height:</xsl:text>
                            <xsl:value-of select="@height" />
                            <xsl:text>px;</xsl:text>
                        </xsl:if>
                        <xsl:if test="$id='data_attribute'">
                            <xsl:text>width:</xsl:text>
                            <xsl:text>634px;</xsl:text>
                        </xsl:if>
                    </xsl:attribute>
                </xsl:if>
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
        <div>
            <xsl:attribute name="id">
                <xsl:text>element_</xsl:text>
                <xsl:number count="container" />
                <xsl:text>_</xsl:text>
                <xsl:number count="element" />
                <xsl:text>_anchor</xsl:text>
            </xsl:attribute>
            <xsl:attribute name="class">
                <xsl:text>menu element_head</xsl:text>
            </xsl:attribute>
            <div>
                <xsl:attribute name="class">
                    <xsl:text>title</xsl:text>
                </xsl:attribute>
                <xsl:value-of select="@type" />
            </div>

            <div>
                <xsl:attribute name="class">
                    <xsl:text>buttons</xsl:text>
                </xsl:attribute>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="id">
                        <xsl:text>element_</xsl:text>
                        <xsl:number count="container" />
                        <xsl:text>_</xsl:text>
                        <xsl:number count="element" />
                    </xsl:attribute>
                    <xsl:attribute name="title">
                        <xsl:text>expand</xsl:text>
                    </xsl:attribute>
                    <xsl:text>˅</xsl:text>
                </a>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                        <xsl:text>javascript:ThrowEvent('movetop','element-</xsl:text>
                        <xsl:number count="container" />-<xsl:number count="element" />
                        <xsl:text>');</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Top</xsl:text>
                </a>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                        <xsl:text>javascript:ThrowEvent('movedown','element-</xsl:text>
                        <xsl:number count="container" />-<xsl:number count="element" />
                        <xsl:text>');</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Down</xsl:text>
                </a>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                        <xsl:text>javascript:ThrowEvent('moveup','element-</xsl:text>
                        <xsl:number count="container" />-<xsl:number count="element" />
                        <xsl:text>');</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Up</xsl:text>
                </a>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                        <xsl:text>javascript:ThrowEvent('copy','element-</xsl:text>
                        <xsl:number count="container" />-<xsl:number count="element" />
                        <xsl:text>');</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Copy</xsl:text>
                </a>
                <a>
                    <xsl:attribute name="class">
                        <xsl:text>button</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="href">
                        <xsl:text>javascript:ThrowEventConfirm('remove','element-</xsl:text>
                        <xsl:number count="container" />-<xsl:number count="element" />
                        <xsl:text>','Are you sure you want to delete this element?');</xsl:text>
                    </xsl:attribute>
                    <xsl:text>X</xsl:text>
                </a>
            </div>

            <div>
                <xsl:attribute name="class">
                    <xsl:text>publish</xsl:text>
                </xsl:attribute>
                <label>
                    <xsl:attribute name="for">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementpublish</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Published</xsl:text>
                </label>
                <input>
                    <xsl:attribute name="class">
                        <xsl:text>checkbox</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="type">
                        <xsl:text>checkbox</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementpublish</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="id">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementpublish</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="value">
                        <xsl:text>Publish</xsl:text>
                    </xsl:attribute>
                    <xsl:if test="@publish='true'">
                        <xsl:attribute name="checked">
                            <xsl:text>true</xsl:text>
                        </xsl:attribute>
                    </xsl:if>
                </input>
            </div>

            <div>
                <xsl:attribute name="class">
                    <xsl:text>name</xsl:text>
                </xsl:attribute>
                <label>
                    <xsl:attribute name="for">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementtitle</xsl:text>
                    </xsl:attribute>
                    <xsl:text>Name</xsl:text>
                </label>
                <input>
                    <xsl:attribute name="type">
                        <xsl:text>text</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="name">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementtitle</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="id">
                        <xsl:text>data_element_</xsl:text>
                        <xsl:number count="container" />_<xsl:number count="element" />
                        <xsl:text>_elementtitle</xsl:text>
                    </xsl:attribute>
                    <xsl:attribute name="value">
                        <xsl:value-of select="@name" />
                    </xsl:attribute>
                </input>
            </div>
        </div>
    </xsl:template>

</xsl:stylesheet>