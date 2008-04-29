var Register = new Object();

Register.submitMessage = function(url)
{
	// Track this click in the registration funnel of Google Analytics.
	if(typeof(pageTracker) !== "undefined")
	{
		pageTracker._trackPageview("/funnel-registration/register-clicked.html")
	}
	
	GraffitiHelpers.statusMessage('registration-status', 'Sending request, please wait.', true);
	
	new Ajax.Request(url + '?command=register',
	{
		method: 'post',
		parameters: Form.serialize('registationForm'),
		onSuccess: function(transport)
		{
			var response = transport.responseText;
			GraffitiHelpers.statusMessage('registration-status', response, false);
		},
		onFailure: function()
		{
			GraffitiHelpers.statusMessage('registration-status', 'An error occured while processing your request. It is likely that the request has not been sent.', true);
		}
	});
}