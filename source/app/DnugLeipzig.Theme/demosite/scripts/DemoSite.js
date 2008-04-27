var DemoSite = new Object();

DemoSite.setupDemoSite = function(url)
{
	DemoSite.actionIndex = 0;
	DemoSite.runNextAction(url);
}

DemoSite.actionIndex = 0;

DemoSite.actionList = new Array('create-event-category',
								'configure-event-plugin',
								'enable-event-plugin',
								'create-registration-post',
								'create-sample-events',
								'create-talk-category',
								'configure-talk-plugin',
								'enable-talk-plugin',
								'create-sample-talks',
								'create-navigation-links');
								
DemoSite.runNextAction = function(url)
{
	var index = DemoSite.actionIndex;
	DemoSite.actionIndex++;
	
	if (DemoSite.actionIndex >= DemoSite.actionList.length)
	{
		DemoSite.runAction(url, DemoSite.actionList[index]);
	}
	else
	{
		DemoSite.runAction(url, DemoSite.actionList[index], function() { DemoSite.runNextAction(url); });
	}
}

DemoSite.runAction = function(url, action, successAction)
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
			
			if(typeof(successAction) !== "undefined")
			{
				successAction();
			}
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