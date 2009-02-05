function ThrowEvent(mainevent, mainvalue)
{
	document.systemform.event_main.value = mainevent;
	document.systemform.event_mainvalue.value = mainvalue;
	document.systemform.submit();
}

function ThrowEventConfirm(mainevent, mainvalue, confirmtext)
{
	if (confirm(confirmtext))
	  ThrowEvent(mainevent, mainvalue);
}
	
function ThrowEventNew(mainevent, mainvalue, text, suggest)
{
  if (suggest == null)
    suggest = '';

  input = prompt(text, suggest);
  if (input != null)
  {	
    if (mainvalue != '')
      ThrowEvent(mainevent, mainvalue + '*' + input);
    else
      ThrowEvent(mainevent, input);
  }
}

function showAndHide(showLayerId, hideLayerId) 
{
    setDisplay(showLayerId, "block");
    setDisplay(hideLayerId, "none");
}

function setDisplay(whichElement, newValue)
{
  if (document.getElementById)
  {
    // this is the way the standards work
    var style2 = document.getElementById(whichElement).style;
    style2.display = newValue;
  }
  else if (document.all)
  {
    // this is the way old msie versions work
    var style2 = document.all[whichElement].style;
    style2.display = newValue;
  }
  else if (document.layers)
  {
    // this is the way nn4 works
    var style2 = document.layers[whichElement].style;
    style2.display = newValue;
  }
}

function open_window(my_url, name)
{
	editwin_win = window.open(my_url, name);
	editwin_win.focus();
	editwin_win.opener = self;}

function open_editwin(my_url, name, WinWidth, WinHeight)
{
	if (WinWidth == null) WinWidth = '500';
	if (WinHeight == null) WinHeight = '500';
	
	var editwin = "";    
	editwin = editwin + "width=" + WinWidth;
	editwin = editwin + ",height=" + WinHeight;
	editwin = editwin + ",resizable=yes";
	editwin = editwin + ",scrollbars=yes";
	editwin = editwin + ",menubar=no";
	editwin = editwin + ",toolbar=no";
	editwin = editwin + ",directories=no";
	editwin = editwin + ",location=no";
	editwin = editwin + ",status=no";
	editwin = editwin + ",top=20";
	editwin = editwin + ",left=250";
	editwin_win = window.open(my_url,name,editwin);
	editwin_win.focus();
	editwin_win.opener = self;
}