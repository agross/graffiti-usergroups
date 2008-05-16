var DemoSite = new Object();

DemoSite.setupDemoSite = function(url)
{
	Form.Element.disable('start')
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
		DemoSite.runAction(url, DemoSite.actionList[index], function() { DemoSite.setupComplete(url); });
	}
	else
	{
		DemoSite.runAction(url, DemoSite.actionList[index], function() { DemoSite.runNextAction(url); });
	}
}

DemoSite.runAction = function(url, action, successAction)
{
	$(action + '-success').hide();
	$(action + '-error').hide();
	$(action + '-todo').hide();
	$(action).show();
			
	new Ajax.Request(url + '?command=' + action,
	{
		method: 'post',
		onSuccess: function(transport)
		{
			$(action + '-success').show();
			
			if(typeof(successAction) !== "undefined")
			{
				successAction();
			}
		},
		onFailure: function(transport)
		{
			var response = transport.responseText
			GraffitiHelpers.statusMessage('setup-status', 'An error occured while processing your request. ' + response, false);
			$(action + '-error').show();
		},
		onComplete: function()
		{
			$(action).hide();
		}
	});
}

DemoSite.setupComplete = function(url)
{
	new Ajax.Updater('navigation-ajax', url + '?command=load-navigation',
	{
		method: 'post',
		onFailure: function(transport)
		{
			var response = transport.responseText
			GraffitiHelpers.statusMessage('setup-status', 'An error occured while processing your request. ' + response, false);
		}
	});
	
	$('start').hide();
	
	$('navigation-ajax').show();
	$('your-turn').show();
	$('proceed').show();
}