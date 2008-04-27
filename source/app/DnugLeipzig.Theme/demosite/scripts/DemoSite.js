var DemoSite = new Object();

DemoSite.setupDemoSite = function(url)
{
	Event.observe('create-event-category-success',
				  'DOMAttrModified',
				  function(){ DemoSite.runAction(url, 'configure-event-plugin'); })
		
	Event.observe('configure-event-plugin-success',
				  'DOMAttrModified',
				  function(){ DemoSite.runAction(url, 'enable-event-plugin'); })
				  
	Event.observe('enable-event-plugin-success',
				  'DOMAttrModified',
				  function(){ DemoSite.runAction(url, 'create-registration-post'); })
				  
	Event.observe('create-registration-post-success',
				  'DOMAttrModified',
				  function(){ DemoSite.runAction(url, 'create-sample-events'); })
	
	DemoSite.runAction(url, 'create-event-category');
}

DemoSite.runAction = function(url, action)
{
	new Ajax.Request(url + '?command=' + action,
	{
		method: 'post',
		onLoading: function()
		{
			$(action + '-success').hide();
			$(action + '-error').hide();
			$(action + '-todo').hide();
			$(action).show();
		},
		onSuccess: function(transport)
		{
			var response = transport.responseText;
			GraffitiHelpers.statusMessage('registration-status', response, false);
			$(action + '-success').show();
		},
		onFailure: function(transport)
		{
			var response = transport.responseText
			GraffitiHelpers.statusMessage('registration-status', 'An error occured while processing your request. ' + response, false);
			$(action + '-error').show();
		},
		onComplete: function()
		{
			$(action).hide();
		}
	});
}