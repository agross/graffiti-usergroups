var Register = new Object();

Register.submitMessage = function(url)
{
	// Track this click in the registration funnel of Google Analytics.
	if(typeof(pageTracker) !== "undefined")
	{
		pageTracker._trackPageview("/funnel-registration/register-clicked.html")
	}
	
	Form.Element.disable('register');
	$('success').hide();
	$('error').hide();
	$('working').show();
	GraffitiHelpers.statusMessage('registration-status', 'Sending request, please wait.', true);
	
	new Ajax.Request(url + '?command=register',
	{
		method: 'post',
		parameters: Form.serialize('registration'),
		onSuccess: function(transport)
		{
			var response = transport.responseText.evalJSON();
			if (response.Success)
			{
				$('success').show();
				
				GraffitiHelpers.statusMessage('registration-status', "Thank you for your registration.", false);
				if(response.WaitingListEvents.size() > 0)
				{
					GraffitiHelpers.statusMessage('registration-status', $('registration-status').innerHTML + " You are on the waiting list for some events.", false);
					response.WaitingListEvents.each(function(item) { $('event-' + item + '-waitinglist').show(); });
				}
			}
			else
			{
				$('error').show();
				
				var message = 'An error has occured. Please check the following form fields:<ul>';
				response.ValidationErrors.each(function(item) { message += '<li>' + item + '</li>'; });
				message += '</ol>';
				
				GraffitiHelpers.statusMessage('registration-status', message, false);
			}
		},
		onFailure: function()
		{
			$('error').show();
			GraffitiHelpers.statusMessage('registration-status', 'An error occured while processing your request. It is likely that the request has not been sent.', true);
		},
		onComplete: function()
		{
			$('working').hide();
			Form.Element.enable('register');
		}
	});
}