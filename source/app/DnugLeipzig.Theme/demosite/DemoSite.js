var DemoSite = new Object();

DemoSite.setupDemoSite = function(url)
{
	DemoSite.createEventCategory(url);
}

DemoSite.createEventCategory = function(url)
{
	new Ajax.Request(url + '?command=create-event-category',
	{
		method: 'post',
		onLoading: function()
		{
			$('create-event-category').show();
		},
		onSuccess: function(transport)
		{
			var response = transport.responseText || "No response";
			GraffitiHelpers.statusMessage('registration-status', response, true);
			
			DemoSite.configureEventsPlugin(url);
		},
		onFailure: function()
		{
			GraffitiHelpers.statusMessage('registration-status', 'Beim Verarbeiten der Anforderung ist ein Fehler aufgetreten. Die Anmeldung wurde wahrscheinlich nicht gesendet.', true);
		},
		onComplete: function()
		{
			$('create-event-category').hide();
		}
	});
}

DemoSite.configureEventsPlugin = function(url)
{
	new Ajax.Request(url + '?command=configure-events-plugin',
	{
		method: 'post',
		onLoading: function()
		{
			$('configure-events-plugin').show();
		},
		onSuccess: function(transport)
		{
			var response = transport.responseText || "No response";
			GraffitiHelpers.statusMessage('registration-status', response, true);
		},
		onFailure: function()
		{
			GraffitiHelpers.statusMessage('registration-status', 'Beim Verarbeiten der Anforderung ist ein Fehler aufgetreten. Die Anmeldung wurde wahrscheinlich nicht gesendet.', true);
		},
		onComplete: function()
		{
			$('configure-events-plugin').hide();
		}
	});
}